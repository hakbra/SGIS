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
        protected System.Drawing.Point oldMouse;
        protected System.Drawing.Point mouse;
        protected System.Drawing.Point rightMouse;
        protected System.Drawing.Point leftMouse;
        protected bool leftMouseDown = false;
        protected bool rightMouseDown = false;

        public void MouseDown(MouseEventArgs e)
        {
            mouse = SGIS.app.getMousePos();
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
            SGIS.app.getMapWindow().Focus();
            
            this.MouseDownImpl(e);
        }
        public void MouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = false;
            }
            this.MouseUpImpl(e);
        }
        public void MouseMove(MouseEventArgs e)
        {
            oldMouse = mouse;
            mouse = SGIS.app.getMousePos();
            SGIS.app.redraw();
            this.MouseMoveImpl(e);
        }
        public void MouseWheel(MouseEventArgs e)
        {
            this.MouseWheelImpl(e);
        }
        public void render(System.Drawing.Graphics g)
        {
            var realP = SGIS.app.screenManager.MapScreenToReal(mouse);
            SGIS.app.setStatusText(String.Format("Coords: [{0:F3}, {1:F3}]", realP.X, realP.Y));

            this.renderImpl(g);
        }

        public abstract void MouseDownImpl(MouseEventArgs e);
        public abstract void MouseUpImpl(MouseEventArgs e);
        public abstract void MouseMoveImpl(MouseEventArgs e);
        public abstract void MouseWheelImpl(MouseEventArgs e);
        public abstract void renderImpl(System.Drawing.Graphics g);
    }

    public class StandardMouseTactic : MouseTactic
    {

        public override void MouseDownImpl(MouseEventArgs e) { }

        public override void MouseUpImpl(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Layer l = (Layer)SGIS.app.getLayerList().SelectedItem;
                if (l == null)
                    return;
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
                    List<Feature> s = l.getWithin(SGIS.app.screenManager.MapScreenToReal(new Envelope(mouse.X, rightMouse.X, mouse.Y, rightMouse.Y)));
                    SGIS.app.setStatusText(s.Count + " objects");
                    foreach (Feature f in s)
                    {
                        f.selected = true;
                        l.selected.Add(f);
                    }
                }
            }
        }

        public override void MouseMoveImpl(MouseEventArgs e)
        {
            if (leftMouseDown)
            {
                SGIS.app.screenManager.ScrollScreen(new NTSPoint(oldMouse.X - mouse.X, oldMouse.Y - mouse.Y));
                SGIS.app.screenManager.Calculate();
            }
        }

        public override void MouseWheelImpl(MouseEventArgs e)
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

        public override void renderImpl(Graphics g)
        {
            if (rightMouseDown)
            {
                Pen pen = new Pen(Color.Black);
                g.DrawRectangle(pen, new System.Drawing.Rectangle(
                        Math.Min(mouse.X, rightMouse.X),
                        Math.Min(mouse.Y, rightMouse.Y),
                        Math.Abs(rightMouse.X - mouse.X),
                        Math.Abs(rightMouse.Y - mouse.Y)));
            }
        }
    }
}
