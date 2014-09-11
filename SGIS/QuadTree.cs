using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    public class QuadTree
    {
        private Envelope boundary;
        private IGeometry gboundary;
        private QuadTree[] children = new QuadTree[4];
        private bool hasChildren = false;
        private List<Feature> features = new List<Feature>();

        public QuadTree(double minx, double maxx, double miny, double maxy)
        {
            boundary = new Envelope(minx, maxx, miny, maxy);
            
            OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
            gboundary = fact.ToGeometry(boundary);
        }

        public void add(Feature f)
        {
            if (!hasChildren && features.Count < 20)
            {
                features.Add(f);
                f.Parent = this;
                return;
            }

            if (!hasChildren)
                split();

            foreach (QuadTree qt in children)
            {
                if (qt.gboundary.Contains(f.Geometry))
                {
                    qt.add(f);
                    return;
                }
            }
            features.Add(f);
            f.Parent = this;
        }

        public void split()
        {
            hasChildren = true;
            children[0] = new QuadTree(boundary.MinX, boundary.MinX + boundary.Width / 2, boundary.MinY, boundary.MinY + boundary.Height / 2);
            children[1] = new QuadTree(boundary.MinX, boundary.MinX + boundary.Width / 2, boundary.MinY + boundary.Height / 2, boundary.MaxY);
            children[2] = new QuadTree(boundary.MinX + boundary.Width / 2, boundary.MaxX, boundary.MinY, boundary.MinY + boundary.Height / 2);
            children[3] = new QuadTree(boundary.MinX + boundary.Width / 2, boundary.MaxX, boundary.MinY + boundary.Height / 2, boundary.MaxY);

            List<Feature> newfeatures = new List<Feature>();
            foreach (Feature f in features)
            { 
                bool added = false;
                foreach (QuadTree qt in children) {
                    if (qt.gboundary.Contains(f.Geometry))
                    {
                        qt.add(f);
                        added = true;
                        break;
                    }
                }
                if (!added)
                   newfeatures.Add(f);
            }
            features = newfeatures;
        }

        public void render(System.Drawing.Graphics g)
        {
            System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.Black);

            var min = SGIS.App.ScreenManager.MapRealToScreen(new Point(boundary.MinX, boundary.MinY));
            var max = SGIS.App.ScreenManager.MapRealToScreen(new Point(boundary.MaxX, boundary.MaxY));

            g.DrawRectangle(p, (float)min.X, (float)max.Y, (float)(max.X - min.X), (float)(min.Y - max.Y));

            if (hasChildren)
                foreach (QuadTree qt in children)
                    qt.render(g);
        }

        public List<Feature> getWithin(IGeometry rect)
        {
            if (rect.Contains(gboundary))
                return getAll();
            if (!rect.Intersects(gboundary))
                return new List<Feature>();

            List<Feature> retfeatures = new List<Feature>();
            if (hasChildren)
                foreach (QuadTree qt in children)
                    retfeatures.AddRange(qt.getWithin(rect));
            foreach (Feature f in features)
                if (rect.Intersects(f.Geometry))
                    retfeatures.Add(f);
            return retfeatures;
        }

        public List<Feature> getAll()
        {
            List<Feature> retfeatures = features.ToList();
            if (hasChildren)
                foreach (QuadTree qt in children)
                    retfeatures.AddRange(qt.getAll());
            return retfeatures;
        }

        public void remove(Feature f)
        {
            features.Remove(f);
        }
    }
}
