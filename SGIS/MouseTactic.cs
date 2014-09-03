using GeoLib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGIS
{
    public abstract class MouseTactic
    {
        public static SGIS sgis;
        public abstract void MouseDown(MouseEventArgs e);
        public abstract void MouseUp(MouseEventArgs e);
        public abstract void MouseMove(MouseEventArgs e);
        public abstract void MouseWheel(MouseEventArgs e);
        public abstract void render(Graphics g);
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
                if (new GeoLib.Point(mouse).distanceTo(new GeoLib.Point(rightMouse)) < 5)
                {
                    Shape s = l.getClosest(sgis.screen.MapScreenToReal(mouse));
                    if (s != null)
                        sgis.setStatusText("Id: " + s.id);
                }
                else
                {
                    List<Shape> s = l.getWithin(new C2DRect(new GeoLib.Point(mouse), new GeoLib.Point(rightMouse)));
                    sgis.setStatusText(s.Count + " objects");
                }
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            var newmouse = sgis.getMousePos();
            if (leftMouseDown)
            {
                sgis.screen.ScrollScreen(new C2DVector(mouse.X - newmouse.X, mouse.Y - newmouse.Y));
                sgis.screen.Calculate();
            }
            sgis.redraw();
            mouse = newmouse;
        }

        public override void MouseWheel(MouseEventArgs e)
        {
            var oldGeoPos = sgis.screen.MapScreenToReal(mouse);
            double scale = 1.3;
            if (e.Delta > 0)
            { // Zoom in
                if (sgis.screen.Scale.x > 50)
                    return;
                sgis.screen.RealRect.Grow(1 / scale);
            }
            else if (e.Delta < 0) // Zoom out
                sgis.screen.RealRect.Grow(scale);
            sgis.screen.Calculate();
            var newGeoPos = sgis.screen.MapScreenToReal(mouse);
            sgis.screen.ScrollReal(newGeoPos.MakeVector(oldGeoPos));
            sgis.screen.Calculate();
            sgis.redraw();
        }

        public override void render(Graphics g)
        {

            Font font = new System.Drawing.Font("Helvetica", 10, FontStyle.Italic);
            Brush brush = new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(Color.Black);

            var realP = sgis.screen.MapScreenToReal(mouse);

            g.DrawString("Mpos: " + (int)mouse.X + ", " + (int)mouse.Y, font, brush, 10, 10);
            g.DrawString("Gpos: " + (int)realP.x + ", " + (int)realP.y, font, brush, 10, 22);
            g.DrawString("Offset: " + (int)sgis.screen.Offset.x + ", " + (int)sgis.screen.Offset.y, font, brush, 10, 34);
            g.DrawString("Scale: " + (int)(1 / sgis.screen.Scale.x) + ", " + (int)(1 / sgis.screen.Scale.y), font, brush, 10, 46);

            if (rightMouseDown)
            {
                g.DrawRectangle(pen, new Rectangle(
                        Math.Min(mouse.X, rightMouse.X),
                        Math.Min(mouse.Y, rightMouse.Y),
                        Math.Abs(rightMouse.X - mouse.X),
                        Math.Abs(rightMouse.Y - mouse.Y)));
            }
        }
    }
}
