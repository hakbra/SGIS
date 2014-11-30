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
    // Class with properties for rendering
    public class Style
    {
        public System.Drawing.Pen pen;
        public System.Drawing.SolidBrush brush;

        // style for selected objects
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

    // enum for supported layer types
    public enum ShapeType
    {
        EMPTY,
        POINT,
        LINE,
        POLYGON,
        UNKNOWN
    }

    // class representing one layer with many feature and one style
    public class Layer : INotifyPropertyChanged
    {
        // event fired when name is changed
        public event PropertyChangedEventHandler PropertyChanged;

        // highest feature id in layer, for assigening new id's
        private int maxid = 1;

        // list of features for O(1) access based on id
        public Dictionary<int, Feature> Features {get;private set;}

        // list of selected features
        public List<Feature> Selected { get; set; }

        // common style for all features in layer
        public Style Style;

        // boolean determining if layer is drawn on map or not
        public bool Visible {get;set;}

        public ShapeType shapetype { get; private set; }

        // boundingbox containing all features in layer
        public Envelope Boundingbox { get; set; }

        // datatable containg attributes for all features
        public DataTable DataTable { get; set; }

        // quadtree for fast spatial selection on features
        public QuadTree QuadTree = null;

        // curret projection of layer
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

        // Converts textual representation of shape type to ShapeType
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

        // adds feature to layer
        public int addFeature(Feature s)
        {
            // checks for correct shapetype
            // a layer may only contain shapes of one type
            if (shapetype == ShapeType.EMPTY)
            {
                shapetype = convert(s.Geometry.GeometryType);
                if (shapetype == ShapeType.POINT) // default to black color for points
                    Style.brush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
            }
            else if (shapetype != convert(s.Geometry.GeometryType))
                throw new Exception("Wrong shapetype in layer " + name);

            // assigns id to feature
            if (s.ID > maxid)
                maxid = s.ID + 1;
            if (s.ID == -1)
                s.ID = maxid++;
            Features.Add(s.ID, s);
            if (QuadTree != null)
                QuadTree.add(s);
            return s.ID;
        }

        // delete feature to layer
        public void delFeature(Feature s)
        {
            if (s.Selected)
                Selected.Remove(s);
            if (s.Parent != null)
                s.Parent.remove(s);
            Features.Remove(s.ID);
        }

        // returns closest feature to real-world coordinate
        // return null if no features within limit
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

        // deselects all selected features
        public void clearSelected()
        {
            foreach (Feature f in Selected)
                f.Selected = false;
            Selected.Clear();
        }

        // return list of features within or intersecting with geometry
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

        // returns attributes for feature in layer
        public DataRow getRow(Feature f)
        {
            if (DataTable == null)
                return null;
            return DataTable.Rows[f.ID - 1];
        }

        // fires PorpertyChanged event
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
