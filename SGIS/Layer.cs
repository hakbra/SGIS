using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using GeoAPI.Geometries;
using System.Data;
using NetTopologySuite.Utilities;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SGIS
{
    public struct Style
    {
        public System.Drawing.Pen pen;
        public System.Drawing.SolidBrush brush;
        public static Style Selected = new Style()
        {
            pen = new System.Drawing.Pen(System.Drawing.Color.DarkCyan),
            brush = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan)
        };
    }
    public enum ShapeType
    {
        EMPTY,
        POINT,
        LINE,
        POLYGON,
        UNKNOWN
    }
    public class Layer : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        int maxid = -1;
        public Dictionary<int, Feature> features = new Dictionary<int, Feature>();
        public List<Feature> selected = new List<Feature>();
        public Style style;
        public bool visible;
        public ShapeType shapetype;
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(); }
        }
        public Envelope boundingbox;
        public DataTable dataTable = null;
        public QuadTree quadTree = null;

        public ShapeType convert(string s)
        {
            switch (s)
            {
                case "Polygon":
                case "MultiPolygon":
                    return ShapeType.POLYGON;
                case "LineString":
                    return ShapeType.LINE;
                case "Point":
                    return ShapeType.POINT;
            }
            return ShapeType.UNKNOWN;
        }

        public void createQuadTree()
        {
            if (boundingbox == null)
                throw new Exception("Need boundingbox to create quadTree");
            quadTree = new QuadTree(boundingbox.MinX, boundingbox.MaxX, boundingbox.MinY, boundingbox.MaxY);
            foreach (Feature f in features.Values)
                quadTree.add(f);
        }
        
        public Layer(string n)
        {
            this.name = n;
            shapetype = ShapeType.EMPTY;
            visible = true;
            style = new Style()
            {
                pen = new System.Drawing.Pen(System.Drawing.Color.Black),
                brush = new System.Drawing.SolidBrush(System.Drawing.Color.Gray)
            };
        }

        public override string ToString()
        {
            switch (shapetype)
            {
                case ShapeType.EMPTY:
                    return "E: " + name;
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
            if (shapetype == ShapeType.EMPTY)
                shapetype = convert(s.geometry.GeometryType);
            else if (shapetype != convert(s.geometry.GeometryType))
                throw new Exception("Wring shapetype in layer " + name);

            if (s.id > maxid)
                maxid = s.id + 1;
            if (s.id == -1)
                s.id = maxid++;
            features.Add(s.id, s);
            if (quadTree != null)
                quadTree.add(s);
        }

        public void delFeature(Feature f)
        {
            features.Remove(f.id);
            if (f.parent != null)
                f.parent.features.Remove(f);
        }

        public Feature getClosest(Point p, double limit)
        {
            GeometricShapeFactory gsf = new GeometricShapeFactory();
            gsf.Envelope = new Envelope(p.X - limit, p.X + limit, p.Y - limit, p.Y + limit);
            IGeometry circle = gsf.CreateCircle();
            var candidates = getWithin(circle);

            double min = 0;
            Feature minf = null;
            foreach (var f in candidates)
            {
                double dist = f.geometry.Distance(p);
                if (minf == null || dist < min)
                {
                    minf = f;
                    min = dist;
                }
            }
            return minf;
        }

        public void clearSelected()
        {
            foreach (Feature f in selected)
                f.selected = false;
            selected.Clear();
        }

        public List<Feature> getWithin(IGeometry rect)
        {
            if (quadTree != null)
                return quadTree.getWithin(rect);
            var ret = new List<Feature>();
            foreach (var pair in features)
            {
                if (pair.Value.geometry.Intersects(rect))
                    ret.Add(pair.Value);
            }
            return ret;
        }
    }
}
