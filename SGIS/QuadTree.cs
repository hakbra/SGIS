using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    // class representing one quad-tree node, with potential children
    public class QuadTree
    {
        // bounding box and bounding geometry of node
        private Envelope boundary;
        private IGeometry gboundary;

        // children of node, may be null
        private QuadTree[] children = new QuadTree[4];
        private bool hasChildren = false;

        // list of children in node
        private List<Feature> features = new List<Feature>();

        // creates new node with given boundary
        public QuadTree(double minx, double maxx, double miny, double maxy)
        {
            boundary = new Envelope(minx, maxx, miny, maxy);
            
            OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
            gboundary = fact.ToGeometry(boundary);
        }

        // adds feature to node
        public void add(Feature f)
        {
            // if node is leaf node and has less than 20 features
            if (!hasChildren && features.Count < 20)
            {
                features.Add(f);
                f.Parent = this;
                return;
            }

            // if node is leaf node and has more than 19 children
            if (!hasChildren)
                split();    // split node and divide features

            foreach (QuadTree qt in children)
            {
                // if child completely contains feature
                if (qt.gboundary.Contains(f.Geometry))
                {
                    qt.add(f);
                    return;
                }
            }
            // if feature spans multiple children
            features.Add(f);
            f.Parent = this;
        }

        public void split()
        {
            // creates child nodes
            hasChildren = true;
            children[0] = new QuadTree(boundary.MinX, boundary.MinX + boundary.Width / 2, boundary.MinY, boundary.MinY + boundary.Height / 2);
            children[1] = new QuadTree(boundary.MinX, boundary.MinX + boundary.Width / 2, boundary.MinY + boundary.Height / 2, boundary.MaxY);
            children[2] = new QuadTree(boundary.MinX + boundary.Width / 2, boundary.MaxX, boundary.MinY, boundary.MinY + boundary.Height / 2);
            children[3] = new QuadTree(boundary.MinX + boundary.Width / 2, boundary.MaxX, boundary.MinY + boundary.Height / 2, boundary.MaxY);


            // divide features between child nodes
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

        // recursively render quadtree, only used for debugging purposes
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

        // get features within geometry
        public List<Feature> getWithin(IGeometry rect)
        {
            // if node is completely inside geometry, return all features
            if (rect.Contains(gboundary))
                return getAll();
            // if node is completely outside geometry, return no features
            if (!rect.Intersects(gboundary))
                return new List<Feature>();

            // create list for returning features
            List<Feature> retfeatures = new List<Feature>();

            // recursively add features from children
            if (hasChildren)
                foreach (QuadTree qt in children)
                    retfeatures.AddRange(qt.getWithin(rect));

            // check each feature in current node
            foreach (Feature f in features)
                if (rect.Intersects(f.Geometry))
                    retfeatures.Add(f);
            return retfeatures;
        }

        // gets list of all features in current and all child nodes
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
