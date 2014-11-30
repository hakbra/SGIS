using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTSPoint = NetTopologySuite.Geometries.Point;
namespace SGIS
{
    // class for rendering map given scale and offset
    class Render
    {
        NTSPoint offset;
        NTSPoint scale;

        // constructor clones objects so they are not changed in other threads
        public Render(NTSPoint s, NTSPoint o)
        {
            offset = (NTSPoint) o.Clone();
            scale = (NTSPoint)s.Clone();
        }

        // transforms real world point to screen point
        public NTSPoint ScaleAndOffSet(NTSPoint pt)
        {
            NTSPoint p = new NTSPoint(0, 0);
            p.X = pt.X - offset.X;
            p.Y = pt.Y - offset.Y;
            p.X *= scale.X;
            p.Y *= scale.Y;
            return p;
        }

        // transforms real world point to screen point
        public NTSPoint MapRealToScreen(NTSPoint pt)
        {
            return new NTSPoint(((pt.X - offset.X) * scale.X),
                                ((pt.Y - offset.Y) * scale.Y));

        }

        // transforms real world rect to screen rect
        public System.Drawing.Rectangle MapRealToScreen(Envelope e)
        {
            var min = MapRealToScreen(new NTSPoint(e.MinX, e.MinY));
            var max = MapRealToScreen(new NTSPoint(e.MaxX, e.MaxY));
            return new System.Drawing.Rectangle((int)min.X, (int)max.Y, (int)(max.X - min.X), (int)(min.Y - max.Y));
        }

        // drawm wms-map
        public void Draw(Photo p, Graphics gr)
        {
            Rectangle screenRect = MapRealToScreen(p.Bounds);
            gr.DrawImage(p.Pic, screenRect);
        }

        // draws arbitrary geometry
        public  void Draw(IGeometry ge, Graphics gr, Style c) {
            if (ge.GeometryType == "Polygon")
                drawPolygon((Polygon)ge, gr, c);
            if (ge.GeometryType == "MultiPolygon" || ge.GeometryType == "MultiLineString")
                foreach(IGeometry g in ((IGeometryCollection)ge).Geometries)
                    Draw(g, gr, c);
            if (ge.GeometryType == "LineString")
                drawLine((LineString)ge, gr, c);
            if (ge.GeometryType == "Point")
                drawPoint((NTSPoint)ge, gr, c);
        }

        private  void drawPoint(NTSPoint ge, Graphics gr, Style c)
        {
            float rad = Math.Max(1, c.pen.Width * 5 * (float)scale.X);
            var mid = ScaleAndOffSet(ge);

            gr.FillEllipse(c.brush, (int)(mid.X - rad), (int)(mid.Y - rad), (int)(rad*2), (int)(rad*2));
        }

        private  void drawLine(LineString ge, Graphics gr, Style c)
        {
            var points = ge.Coordinates;
            for (int i = 1; i < points.Count(); i++)
            {
                var a = ScaleAndOffSet(new NTSPoint(points[i - 1]));
                var b = ScaleAndOffSet(new NTSPoint(points[i]));
                gr.DrawLine(c.pen, (int)a.X, (int)a.Y, (int)b.X, (int)b.Y);
            }
        }

        // creates line path for polygon
        private  System.Drawing.Drawing2D.GraphicsPath CreatePath(ILineString poly)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            var points = poly.Coordinates;
            for (int i = 1; i < points.Count(); i++)
            {
                var a = ScaleAndOffSet(new NTSPoint(points[i - 1]));
                var b = ScaleAndOffSet(new NTSPoint(points[i]));
                gp.AddLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y);
            }
            return gp;
        }

        // draws polygon and then path around polygon
        private  void drawPolygon(Polygon ge, Graphics gr, Style c)
        {

            System.Drawing.Drawing2D.GraphicsPath gp = CreatePath(ge.ExteriorRing);

            var hulls = ge.InteriorRings;
            for (int h = 0; h < hulls.Count(); h++)
                gp.AddPath(CreatePath(hulls[h]), false);

            gp.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
            gr.FillPath(c.brush, gp);

            if (c.pen.Width > 0)
                gr.DrawPath(c.pen, gp);
        }
    }
}
