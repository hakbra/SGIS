using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib;

namespace SGIS
{
    public class Layer
    {
        public Dictionary<int, GeoLib.Shape> shapes = new Dictionary<int, Shape>();
        public System.Drawing.Color color;
        public bool visible;
        ShapeType shapetype;
        string name;
        public C2DRect boundingbox;
        
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
                case ShapeType.POLYLINE:
                    return "L: " + name;
                case ShapeType.POLYGON:
                    return "F: " + name;
            }
            return name;
        }
        public void addShape(Shape s)
        {
            shapes.Add(s.id, s);
        }

        public Shape getClosest(C2DPoint p)
        {
            double min = 0;
            Shape mins = null;
            foreach (Shape s in shapes.Values)
            {
                double dist = s.distanceTo(p);
                if (mins == null || dist < min)
                {
                    min = dist;
                    mins = s;
                }
            }
            return mins;
        }

        public List<Shape> getWithin(C2DRect c2DRect)
        {
            return new List<Shape>();
        }
    }
}
