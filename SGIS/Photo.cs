using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    // class representing one background map loaded from wms-server
    class Photo
    {
        public Photo(System.Drawing.Image i, ScreenManager.SGISEnvelope b)
        {
            Pic = i;
            Bounds = new ScreenManager.SGISEnvelope(b);
            
            OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
            Geometry = fact.ToGeometry(b);
        }

        // real world bounding coordinates of map
        public ScreenManager.SGISEnvelope Bounds;

        // bitmap representation of map
        public System.Drawing.Image Pic;

        // geometry representing map, used for intersection tests
        public GeoAPI.Geometries.IGeometry Geometry;

        // draws map to graphics
        public void Draw(System.Drawing.Graphics mapGraphics)
        {
           Rectangle screenRect = SGIS.App.ScreenManager.MapRealToScreen(Bounds);
           mapGraphics.DrawImage(Pic, screenRect);
        }
    }
}
