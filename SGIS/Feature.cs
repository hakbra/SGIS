using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    public class Feature
    {
        public Feature(IGeometry g, int i)
        {
            Geometry = g;
            ID = i;
            Selected = false;
        }
        public Feature(IGeometry g)
        {
            Geometry = g;
            ID = -1;
            Selected = false;
        }

        // NetToplogySuite geometry
        public IGeometry Geometry;

        // internal id in layer
        public int ID;

        // containing quadtree
        public QuadTree Parent;
        public bool Selected;
    }
}
