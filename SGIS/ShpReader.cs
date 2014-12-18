using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System.Data;
using System.Windows.Forms;

namespace SGIS
{
    // class for reading .shp files
    class ShpReader
    {
        // extension of binary reader to read BigEndian integers
        private class BinaryReaderExtension : BinaryReader
        {
            private byte[] a32 = new byte[4];
            public BinaryReaderExtension(System.IO.Stream stream) : base(stream) { }
            public int ReadInt32BigEndian()
            {
                a32 = base.ReadBytes(4);
                Array.Reverse(a32);
                return BitConverter.ToInt32(a32, 0);
            }
        }
        BinaryReaderExtension br;
        // position and length of read
        int pos, length;
        // layer shape type
        int type;
        // layer bounds
        public double minx, miny, maxx, maxy;
        Layer layer;

        // static convenience function
        public static Layer read(string filename){
            ShpReader shpr = new ShpReader();
            return shpr.readImpl(filename);
        }
        public Layer readImpl(string filename)
        {
            // split filename in path and filename
            string[] split = filename.Split('\\');
            string name = split.Last();
            // remove extension
            name = name.Substring(0, name.Length - 4);
            string path = filename.Substring(0, filename.Length - name.Length - 4);

            layer = new Layer(name);

            // read .prj-file with same name or with name 'default.prj'
            string prjName = filename.Substring(0, filename.Length - 3) + "prj";
            if (File.Exists(prjName))
                layer.Projection = PrjReader.read(prjName);
            else if (File.Exists(path + "default.prj"))
                layer.Projection = PrjReader.read(path + "default.prj");
            else
            {
                // warning message if missing prj file
                string msg = "No projection da found. Assuming " + SGIS.App.getSrsName();
                MessageBox.Show(msg, "Missing spatial reference system: " + name, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // try reading attribute file with same name
            string dbfName = filename.Substring(0, filename.Length - 3) + "dbf";
            if (File.Exists(dbfName))
                layer.DataTable = DbfReader.read(dbfName);

            // create stream from file
            FileStream f = File.Open(filename, FileMode.Open);
            br = new BinaryReaderExtension(f);
            pos = 0;
            length = (int)br.BaseStream.Length;

            readHeader();

            // set bounding box from values in header
            layer.Boundingbox = new Envelope(minx, maxx, miny, maxy);
            layer.createQuadTree();

            // content starts at pos 100
            pos = 100;

            // read all shapes until end
            while (pos < length)
                readShape();

            br.Close();

            // features now converted to application srs
            layer.Projection = SGIS.App.SRS;
            return layer;
        }

        private void readShape()
        {
            // read shape id, data length and shape type
            int id = br.ReadInt32BigEndian();
            int length = br.ReadInt32BigEndian();
            int stype = br.ReadInt32();
            pos += 12;

            if (stype == 0) // Null
                return;
            Geometry g = null;
            if (stype == 1) // Point
                g = readPoint();
            if (stype == 3) // Polyline
                g = readPolyline();
            if (stype == 5) // Polygon
                g = readPolygon();

            // add shape to layer
            if (g != null)
            {
                Feature f = new Feature(g, id);
                layer.addFeature(f);
            }
        }

        private LineString readPolyline()
        {
            br.BaseStream.Seek(4*8, SeekOrigin.Current); // Skip bounding box
            int numparts = br.ReadInt32();
            int numpoints = br.ReadInt32();
            if (numparts > 1)
                throw new Exception("Uh oh");
            pos += 4 * 8 + 8;
            int[] parts = new int[numparts];
            for (int i = 0; i < numparts; i++)
                parts[i] = br.ReadInt32();
            pos += 4 * numparts;
            List<Coordinate> points = new List<Coordinate>();
            for (int i = 0; i < numpoints; i++)
                points.Add(readCoordinate());

            return new LineString(points.ToArray());
        }
        private Polygon readPolygon()
        {
            br.BaseStream.Seek(4 * 8, SeekOrigin.Current); // Skip bounding box
            int numparts = br.ReadInt32();
            int numpoints = br.ReadInt32();
            pos += 4 * 8 + 8;
            // parts[] contain accumulated number of points in each polygon part
            int[] parts = new int[numparts+1];
            for (int i = 0; i < numparts; i++)
                parts[i] = br.ReadInt32();
            parts[numparts] = numpoints;
            pos += 4 * numparts;

            // polygon to be read
            ILinearRing p = null;
            // potential holes in polygon
            List<ILinearRing> holes = new List<ILinearRing>();

            for (int i = 0; i < numparts; i++)
            {
                // read points
                var points = readPolygonPart(parts[i + 1] - parts[i]);
                if (p == null) // first polygon is boundary
                    p = new LinearRing(points.ToArray());
                else // subsequent polygons are holes
                    holes.Add(new LinearRing(points.ToArray()));
            }
            return new Polygon(p, holes.ToArray());
        }
        private List<Coordinate> readPolygonPart(int num)
        {
            // read polygon consisting of num points
            List<Coordinate> points = new List<Coordinate>();
            for (; num > 0; num--)
                points.Add(readCoordinate());
            return points;
        }

        private Point readPoint() {
            var c = readCoordinate();
            Point p = new Point(c);
            return p;
        }

        private Coordinate readCoordinate()
        {
            Coordinate p = new Coordinate();
            p.X = br.ReadDouble();
            p.Y = br.ReadDouble();
            p = transformCoordinate(p);
            pos += 16;
            return p;
        }

        private Coordinate transformCoordinate(Coordinate c)
        {
            // if layer and application have different projections
            if (SGIS.App.SRS != null && layer.Projection != null && SGIS.App.SRS != layer.Projection)
            {
                double[] x = new double[1] { c.X };
                double[] y = new double[1] { c.Y };
                double[] z = new double[1] { 0 };

                layer.Projection.Transform(SGIS.App.SRS, 1, 1, x, y, z);
                c.X = x[0];
                c.Y = y[0];
            }
            return c;
        }

        // read .shp header
        private void readHeader()
        {
            br.BaseStream.Seek(32, SeekOrigin.Begin);
            type = br.ReadInt32();
            var min = readCoordinate();
            var max = readCoordinate();
            minx = min.X;
            miny = min.Y;
            maxx = max.X;
            maxy = max.Y;
            br.BaseStream.Seek(100, SeekOrigin.Begin);
        }
    }
}
