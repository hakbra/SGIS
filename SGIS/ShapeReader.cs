using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib;

namespace SGIS
{
    class ShapeReader
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

        public Layer read(string filename)
        {
            FileStream f = File.Open(filename, FileMode.Open);
            br = new BinaryReaderExtension(f);
            pos = 0;
            length = (int)br.BaseStream.Length;

            readHeader();

            string name = filename.Split('\\').Last();

            if (type == 1)
                layer = new Layer(name, ShapeType.POINT);
            else if (type == 3)
                layer = new Layer(name, ShapeType.POLYLINE);
            else if (type == 5)
                layer = new Layer(name, ShapeType.POLYGON);
            else
                throw new Exception("Uh oh");

            pos = 100;
            while (pos < length)
                readShape();

            br.Close();

            layer.boundingbox = new C2DRect(minx, maxy, maxx, miny);
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
            ShapeInterface s = null;
            if (stype == 1) // Point
                s = readPoint();
            if (stype == 3) // Polyline
                s = readPolyline();
            if (stype == 5) // Polygon
                s = readPolygon();

            if (s != null)
                layer.addShape(new Shape(s, id));
        }

        private PolyLine readPolyline()
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
            List<Point> points = new List<Point>();
            for (int i = 0; i < numpoints; i++)
                points.Add(readPoint());
            
            return new PolyLine(points.ToArray<Point>());
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
            Polygon p = null;
            for (int i = 0; i < numparts; i++)
            {
                var points = readPolygonPart(parts[i + 1] - parts[i]);
                if (p == null)
                    p = new Polygon(points.ToArray());
                else
                    p.AddHole(new C2DPolygon(points, false));
            }
            return p;
        }
        private List<C2DPoint> readPolygonPart(int num)
        {
            List<C2DPoint> points = new List<C2DPoint>();
            for (; num > 0; num--)
                points.Add(readPoint());
            return points;
        }

        private Point readPoint()
        {
            Point p = new Point();
            p.x = br.ReadDouble();
            p.y = br.ReadDouble();
            pos += 16;
            return p;
        }

        private void readHeader()
        {
            br.BaseStream.Seek(32, SeekOrigin.Begin);
            type = br.ReadInt32();
            minx = br.ReadDouble();
            miny = br.ReadDouble();
            maxx = br.ReadDouble();
            maxy = br.ReadDouble();
            br.BaseStream.Seek(100, SeekOrigin.Begin);
        }
    }
}
