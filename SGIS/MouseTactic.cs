using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NTSPoint = NetTopologySuite.Geometries.Point;

namespace SGIS
{
    public abstract class MouseTactic
    {
        public static SGIS sgis;
        public abstract void MouseDown(MouseEventArgs e);
        public abstract void MouseUp(MouseEventArgs e);
        public abstract void MouseMove(MouseEventArgs e);
        public abstract void MouseWheel(MouseEventArgs e);
        public abstract void render(System.Drawing.Graphics g);
    }

    public class StandardMouseTactic : MouseTactic
    {

        System.Drawing.Point mouse;
        System.Drawing.Point rightMouse;
        System.Drawing.Point leftMouse;
        bool leftMouseDown = false;
        bool rightMouseDown = false;

        public override void MouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = true;
                leftMouse = mouse;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = true;
                rightMouse = mouse;
            }
        }

        public override void MouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = false;
                if (LayerControl.current == null)
                    return;
                Layer l = LayerControl.current.layer;
                double dist = Math.Abs(mouse.X - rightMouse.X) + Math.Abs(mouse.Y - rightMouse.Y);
                if (dist < 5)
                {
                    int s = l.getClosest(sgis.screenManager.MapScreenToReal(mouse));
                    if (s > 0)
                        sgis.setStatusText("Id: " + s);
                }
                else
                {
                    List<int> s = l.getWithin(new Envelope(mouse.X, rightMouse.X, mouse.Y, rightMouse.Y));
                    sgis.setStatusText(s.Count + " objects");
                }
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            var newmouse = sgis.getMousePos();
            if (leftMouseDown)
            {
                sgis.screenManager.ScrollScreen(new NTSPoint(mouse.X - newmouse.X, mouse.Y - newmouse.Y));
                sgis.screenManager.Calculate();
            }
            sgis.redraw();
            mouse = newmouse;
        }

        public override void MouseWheel(MouseEventArgs e)
        {
            var oldGeoPos = sgis.screenManager.MapScreenToReal(mouse);
            double scale = 1.3;
            if (e.Delta > 0)
            { // Zoom in
                sgis.screenManager.RealRect.Grow(1 / scale);
            }
            else if (e.Delta < 0) // Zoom out
                sgis.screenManager.RealRect.Grow(scale);
            sgis.screenManager.Calculate();
            var newGeoPos = sgis.screenManager.MapScreenToReal(mouse);
            sgis.screenManager.ScrollReal(new NTSPoint(oldGeoPos.X - newGeoPos.X, oldGeoPos.Y - newGeoPos.Y));
            sgis.screenManager.Calculate();
            sgis.redraw();
        }

        public override void render(Graphics g)
        {

            Font font = new System.Drawing.Font("Helvetica", 10, FontStyle.Italic);
            Brush brush = new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(Color.Black);

            var realP = sgis.screenManager.MapScreenToReal(mouse);

            g.DrawString("Mpos: " + (int)mouse.X + ", " + (int)mouse.Y, font, brush, 10, 10);
            g.DrawString("Gpos: " + (int)realP.X + ", " + (int)realP.Y, font, brush, 10, 22);
            g.DrawString("Offset: " + (int)sgis.screenManager.Offset.X + ", " + (int)sgis.screenManager.Offset.Y, font, brush, 10, 34);
            g.DrawString("Scale: " + (int)(1 / sgis.screenManager.Scale.X) + ", " + (int)(1 / sgis.screenManager.Scale.Y), font, brush, 10, 46);

            if (rightMouseDown)
            {
                g.DrawRectangle(pen, new System.Drawing.Rectangle(
                        Math.Min(mouse.X, rightMouse.X),
                        Math.Min(mouse.Y, rightMouse.Y),
                        Math.Abs(rightMouse.X - mouse.X),
                        Math.Abs(rightMouse.Y - mouse.Y)));
            }
        }
    }
}
