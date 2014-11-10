using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGIS
{
    class Photo
    {
        public Photo(System.Drawing.Image i, ScreenManager.SGISEnvelope b)
        {
            Pic = i;
            Bounds = new ScreenManager.SGISEnvelope(b);
            
            OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
            Geometry = fact.ToGeometry(b);
        }
        public ScreenManager.SGISEnvelope Bounds;
        public System.Drawing.Image Pic;
        public GeoAPI.Geometries.IGeometry Geometry;

        public void Draw(System.Drawing.Graphics mapGraphics)
        {
           Rectangle screenRect = SGIS.App.ScreenManager.MapRealToScreen(Bounds);
           mapGraphics.DrawImage(Pic, screenRect);
        }
    }
}
