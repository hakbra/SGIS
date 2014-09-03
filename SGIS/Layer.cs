using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;

namespace SGIS
{
    public enum ShapeType
    {
        POINT,
        LINE,
        POLYGON
    }
    public class Layer
    {
        public Dictionary<int, Geometry> shapes = new Dictionary<int, Geometry>();
        public System.Drawing.Color color;
        public bool visible;
        ShapeType shapetype;
        string name;
        public Envelope boundingbox;
        
        public Layer(string n, ShapeType st)
        {
            this.name = n;
            shapetype = st;
            visible = true;
            color = System.Drawing.Color.Black;
        }

        public override string ToString()
        {
            switch (shapetype)
            {
                case ShapeType.POINT:
                    return "P: " + name;
                case ShapeType.LINE:
                    return "L: " + name;
                case ShapeType.POLYGON:
                    return "F: " + name;
            }
            return name;
        }
        public void addShape(int id, Geometry s)
        {
            shapes.Add(id, s);
        }

        public int getClosest(Point p)
        {
            double min = 0;
            int minid = -1;
            foreach (var pair in shapes)
            {
                double dist = pair.Value.Distance(p);
                if (minid == -1 || dist < min)
                {
                    minid = pair.Key;
                    min = dist;
                }
            }
            return minid;
        }

        public List<int> getWithin(Envelope rect)
        {
            return new List<int>();
        }
    }
}
