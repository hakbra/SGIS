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
    class Render
    {
        public static void Draw(Geometry ge, Graphics gr, Color c) {
            if (ge.GeometryType == "Polygon")
                drawPolygon((Polygon)ge, gr, c);
            if (ge.GeometryType == "LineString")
                drawLine((LineString)ge, gr, c);
            if (ge.GeometryType == "Point")
                drawPoint((NTSPoint)ge, gr, c);
        }

        private static void drawPoint(NTSPoint ge, Graphics gr, Color c)
        {
            Brush b = new SolidBrush(c);
            int rad = 5;
            var mid = SGIS.app.screenManager.ScaleAndOffSet(ge);

            gr.FillEllipse(b, (int)(mid.X + rad / 2), (int)(mid.Y + rad / 2), (int)(rad*2), (int)(rad*2));
        }

        private static void drawLine(LineString ge, Graphics gr, Color c)
        {
            Pen p = new Pen(c);
            var points = ge.Coordinates;
            for (int i = 1; i < points.Count(); i++)
            {
                var a = SGIS.app.screenManager.ScaleAndOffSet(new NTSPoint(points[i - 1]));
                var b = SGIS.app.screenManager.ScaleAndOffSet(new NTSPoint(points[i]));
                gr.DrawLine(p, (int)a.X, (int)a.Y, (int)b.X, (int)b.Y);
            }
        }
        private static System.Drawing.Drawing2D.GraphicsPath CreatePath(ILineString poly)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            var points = poly.Coordinates;
            for (int i = 1; i < points.Count(); i++)
            {
                var a = SGIS.app.screenManager.ScaleAndOffSet(new NTSPoint(points[i - 1]));
                var b = SGIS.app.screenManager.ScaleAndOffSet(new NTSPoint(points[i]));
                gp.AddLine((int)a.X, (int)a.Y, (int)b.X, (int)b.Y);
            }
            return gp;
        }
        private static void drawPolygon(Polygon ge, Graphics gr, Color c)
        {

            System.Drawing.Drawing2D.GraphicsPath gp = CreatePath(ge.ExteriorRing);

            var hulls = ge.InteriorRings;
            for (int h = 0; h < hulls.Count(); h++)
            {
                gp.AddPath(CreatePath(hulls[h]), false);
            }
            gp.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;

            Brush b = new SolidBrush(c);
            gr.FillPath(b, gp);
        }
    }
}
