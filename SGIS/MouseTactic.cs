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
            SGIS.app.Focus();
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
                foreach (Feature f in l.selected)
                    f.selected = false;
                l.selected.Clear();
                if (dist < 5)
                {
                    Feature s = l.getClosest(SGIS.app.screenManager.MapScreenToReal(mouse));
                    if (s != null)
                        SGIS.app.setStatusText("Id: " + s.id);
                    s.selected = true;
                    l.selected.Add(s);
                }
                else
                {
                    var a = SGIS.app.screenManager.MapScreenToReal(mouse);
                    var b = SGIS.app.screenManager.MapScreenToReal(rightMouse);
                    List<Feature> s = l.getWithin(new Envelope(a.X, b.X, a.Y, b.Y));
                    SGIS.app.setStatusText(s.Count + " objects");
                    foreach (Feature f in s)
                    {
                        f.selected = true;
                        l.selected.Add(f);
                    }
                }
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            var newmouse = SGIS.app.getMousePos();
            if (leftMouseDown)
            {
                SGIS.app.screenManager.ScrollScreen(new NTSPoint(mouse.X - newmouse.X, mouse.Y - newmouse.Y));
                SGIS.app.screenManager.Calculate();
            }
            SGIS.app.redraw();
            mouse = newmouse;
        }

        public override void MouseWheel(MouseEventArgs e)
        {
            var oldGeoPos = SGIS.app.screenManager.MapScreenToReal(mouse);
            double scale = 1.3;
            if (e.Delta > 0)
            { // Zoom in
                if (SGIS.app.screenManager.RealRect.Width < 5)
                    return;
                SGIS.app.screenManager.RealRect.Grow(1 / scale);
            }
            else if (e.Delta < 0) // Zoom out
                SGIS.app.screenManager.RealRect.Grow(scale);
            SGIS.app.screenManager.Calculate();
            var newGeoPos = SGIS.app.screenManager.MapScreenToReal(mouse);
            SGIS.app.screenManager.ScrollReal(new NTSPoint(oldGeoPos.X - newGeoPos.X, oldGeoPos.Y - newGeoPos.Y));
            SGIS.app.screenManager.Calculate();
            SGIS.app.redraw();
        }

        public override void render(Graphics g)
        {

            Font font = new System.Drawing.Font("Helvetica", 10, FontStyle.Italic);
            Brush brush = new SolidBrush(System.Drawing.Color.Black);
            Pen pen = new Pen(Color.Black);

            var realP = SGIS.app.screenManager.MapScreenToReal(mouse);

            g.DrawString("Mpos: " + (int)mouse.X + ", " + (int)mouse.Y, font, brush, 10, 10);
            g.DrawString("Gpos: " + (int)realP.X + ", " + (int)realP.Y, font, brush, 10, 22);
            g.DrawString("Offset: " + (int)SGIS.app.screenManager.Offset.X + ", " + (int)SGIS.app.screenManager.Offset.Y, font, brush, 10, 34);
            g.DrawString("Scale: " + (int)(1 / SGIS.app.screenManager.Scale.X) + ", " + (int)(1 / SGIS.app.screenManager.Scale.Y), font, brush, 10, 46);

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
