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
    public class Style
    {
        public System.Drawing.Pen pen;
        public System.Drawing.SolidBrush brush;
        public static Style Selected = new Style()
        {
            pen = new System.Drawing.Pen(System.Drawing.Color.DarkCyan),
            brush = new System.Drawing.SolidBrush(System.Drawing.Color.Cyan)
        };
        public Style()
        {
            pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            brush = new System.Drawing.SolidBrush(System.Drawing.Color.Gray);
        }
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
        private int maxid = 1;
        public Dictionary<int, Feature> Features {get;private set;}
        public List<Feature> Selected { get; set; }
        public Style Style;
        public bool Visible {get;set;}
        public ShapeType shapetype { get; private set; }
        public Envelope Boundingbox { get; set; }
        public DataTable DataTable { get; set; }
        public QuadTree QuadTree = null;
        public Proj4CSharp.IProjection Projection { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; NotifyPropertyChanged(); }
        }

        public Layer(string n)
        {
            name = n;
            shapetype = ShapeType.EMPTY;
            Visible = true;
            Features = new Dictionary<int, Feature>();
            Selected = new List<Feature>();
            shapetype = ShapeType.EMPTY;

            Style = new Style();
        }

        public ShapeType convert(string s)
        {
            switch (s)
            {
                case "Polygon":
                case "MultiPolygon":
                    return ShapeType.POLYGON;
                case "LineString":
                case "MultiLineString":
                    return ShapeType.LINE;
                case "Point":
                    return ShapeType.POINT;
            }
            return ShapeType.UNKNOWN;
        }

        public void calculateBoundingBox()
        {
            Envelope bb = new Envelope();
            foreach (Feature f in Features.Values)
            {
                Envelope bbg = f.Geometry.EnvelopeInternal;
                bb.ExpandToInclude(bbg);
            }
            Boundingbox = bb;
        }

        public void createQuadTree()
        {
            if (Boundingbox == null)
                throw new Exception("Need boundingbox to create quadTree");
            QuadTree = new QuadTree(Boundingbox.MinX, Boundingbox.MaxX, Boundingbox.MinY, Boundingbox.MaxY);
            foreach (Feature f in Features.Values)
                QuadTree.add(f);
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
                    return "S: " + name;
            }
            return name;
        }
        public int addFeature(Feature s)
        {
            if (shapetype == ShapeType.EMPTY)
                shapetype = convert(s.Geometry.GeometryType);
            else if (shapetype != convert(s.Geometry.GeometryType))
                throw new Exception("Wrong shapetype in layer " + name);

            if (s.ID > maxid)
                maxid = s.ID + 1;
            if (s.ID == -1)
                s.ID = maxid++;
            Features.Add(s.ID, s);
            if (QuadTree != null)
                QuadTree.add(s);
            return s.ID;
        }

        public void delFeature(Feature f)
        {
            Features.Remove(f.ID);
            if (f.Parent != null)
                f.Parent.remove(f);
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
                double dist = f.Geometry.Distance(p);
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
            foreach (Feature f in Selected)
                f.Selected = false;
            Selected.Clear();
        }

        public List<Feature> getWithin(IGeometry rect)
        {
            if (QuadTree != null)
                return QuadTree.getWithin(rect);
            var ret = new List<Feature>();
            foreach (var pair in Features)
            {
                if (pair.Value.Geometry.Intersects(rect))
                    ret.Add(pair.Value);
            }
            return ret;
        }
        public DataRow getRow(Feature f)
        {
            if (DataTable == null)
                return null;
            return DataTable.Rows[f.ID - 1];
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
