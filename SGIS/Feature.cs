using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    public class Feature
    {
        public Feature(Geometry g, int i)
        {
            geometry = g;
            id = i;
            selected = false;
        }
        public Geometry geometry;
        public int id;
        public bool selected;
    }
}
