using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using System.Data;

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
        public Dictionary<int, Feature> shapes = new Dictionary<int, Feature>();
        public List<Feature> selected = new List<Feature>();
        public System.Drawing.Color color;
        public bool visible;
        ShapeType shapetype;
        string name;
        public Envelope boundingbox;
        public DataTable dataTable = null;
        public QuadTree quadTree = null;
        public void createQuadTree()
        {
            if (boundingbox == null)
                throw new Exception("Need boundingbox to create quadTree");
            quadTree = new QuadTree(boundingbox.MinX, boundingbox.MaxX, boundingbox.MinY, boundingbox.MaxY);
        }
        
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
        public void addFeature(Feature s)
        {
            shapes.Add(s.id, s);
            if (quadTree != null)
                quadTree.add(s);
        }

        public Feature getClosest(Point p)
        {
            double min = 0;
            Feature g = null;
            foreach (var pair in shapes)
            {
                double dist = pair.Value.geometry.Distance(p);
                if (g == null || dist < min)
                {
                    g = pair.Value;
                    min = dist;
                }
            }
            return g;
        }

        public List<Feature> getWithin(IGeometry rect)
        {
            if (quadTree != null)
                return quadTree.getWithin(rect);
            var ret = new List<Feature>();
            foreach (var pair in shapes)
            {
                if (pair.Value.geometry.Intersects(rect))
                    ret.Add(pair.Value);
            }
            return ret;
        }
    }
}
