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
    public enum ShapeType
    {
        POINT,
        LINE,
        POLYGON
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

        public Dictionary<int, Feature> features = new Dictionary<int, Feature>();
        public List<Feature> selected = new List<Feature>();
        public System.Drawing.Color color;
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
        public void createQuadTree()
        {
            if (boundingbox == null)
                throw new Exception("Need boundingbox to create quadTree");
            quadTree = new QuadTree(boundingbox.MinX, boundingbox.MaxX, boundingbox.MinY, boundingbox.MaxY);
            foreach (Feature f in features.Values)
                quadTree.add(f);
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
            features.Add(s.id, s);
            if (quadTree != null)
                quadTree.add(s);
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
