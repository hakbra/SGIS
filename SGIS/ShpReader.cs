using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System.Data;

namespace SGIS
{
    class ShpReader
    {
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
        int pos, length;
        int type;
        public double minx, miny, maxx, maxy;
        Layer layer;

        public static Layer read(string filename){
            ShpReader shpr = new ShpReader();
            return shpr.readImpl(filename);
        }
        public Layer readImpl(string filename)
        {
            string name = filename.Split('\\').Last();
            name = name.Substring(0, name.Length - 4);

            layer = new Layer(name);

            string prjName = filename.Substring(0, filename.Length - 3) + "prj";
            if (File.Exists(prjName))
                layer.Projection = PrjReader.read(prjName);

            string dbfName = filename.Substring(0, filename.Length - 3) + "dbf";
            if (File.Exists(dbfName))
                layer.DataTable = DBFReader.read(dbfName);

            FileStream f = File.Open(filename, FileMode.Open);
            br = new BinaryReaderExtension(f);
            pos = 0;
            length = (int)br.BaseStream.Length;

            readHeader();

            layer.Boundingbox = new Envelope(minx, maxx, miny, maxy);
            layer.createQuadTree();

            pos = 100;
            while (pos < length)
                readShape();

            br.Close();

            return layer;
        }

        private void readShape()
        {
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
            int[] parts = new int[numparts+1];
            for (int i = 0; i < numparts; i++)
                parts[i] = br.ReadInt32();
            parts[numparts] = numpoints;
            pos += 4 * numparts;

            ILinearRing p = null;
            List<ILinearRing> holes = new List<ILinearRing>();

            for (int i = 0; i < numparts; i++)
            {
                var points = readPolygonPart(parts[i + 1] - parts[i]);
                if (p == null)
                    p = new LinearRing(points.ToArray());
                else
                    holes.Add(new LinearRing(points.ToArray()));
            }
            return new Polygon(p, holes.ToArray());
        }
        private List<Coordinate> readPolygonPart(int num)
        {
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
