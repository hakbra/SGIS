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
        public static ScreenManager sm;
        public static void Draw(Geometry ge, Graphics gr, Color c) {
            if (ge.GeometryType == "Polygon")
                drawPolygon((Polygon)ge, gr, c);
            if (ge.GeometryType == "Line")
                drawLine((LineString)ge, gr, c);
            if (ge.GeometryType == "Point")
                drawPoint((NTSPoint)ge, gr, c);
        }

        private static void drawPoint(NTSPoint ge, Graphics gr, Color c)
        {
            Brush b = new SolidBrush(c);
            int rad = 5;
            NTSPoint mid = new NTSPoint(ge.X, ge.Y);
            sm.ScaleAndOffSet(mid);

            gr.FillEllipse(b, (int)(mid.X + rad / 2), (int)(mid.Y + rad / 2), (int)(rad*2), (int)(rad*2));
        }

        private static void drawLine(LineString ge, Graphics gr, Color c)
        {
            throw new NotImplementedException();
        }

        private static void drawPolygon(Polygon ge, Graphics gr, Color c)
        {
            throw new NotImplementedException();
        }
    }
}
