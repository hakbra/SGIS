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

        public virtual void MouseDown(MouseEventArgs e)
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
        }
        public virtual void MouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftMouseDown = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseDown = false;
            }
        }
        public virtual void MouseMove(MouseEventArgs e)
        {
            oldMouse = mouse;
            mouse = SGIS.app.getMousePos();
            SGIS.app.redraw();
        }
        public virtual void MouseWheel(MouseEventArgs e)
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
        public virtual void render(System.Drawing.Graphics g)
        {
            var realP = SGIS.app.screenManager.MapScreenToReal(mouse);
            SGIS.app.setCoordText(String.Format("Coords: [{0:F3}, {1:F3}]", realP.X, realP.Y));
        }
    }

    public class MoveMouseTactic : MouseTactic
    {
        public override void MouseMove(MouseEventArgs e)
        {
            base.MouseMove(e);

            if (leftMouseDown)
            {
                SGIS.app.screenManager.ScrollScreen(new NTSPoint(oldMouse.X - mouse.X, oldMouse.Y - mouse.Y));
                SGIS.app.screenManager.Calculate();
            }
        }
    }

    public class SelectMouseTactic : MouseTactic
    {
        public override void MouseUp(MouseEventArgs e)
        {
            base.MouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                Layer l = (Layer)SGIS.app.getLayerList().SelectedItem;
                if (l == null)
                    return;

                foreach (Feature f in l.selected)
                    f.selected = false;
                l.selected.Clear();

                List<Feature> s = l.getWithin(SGIS.app.screenManager.MapScreenToReal(new Envelope(mouse.X, leftMouse.X, mouse.Y, leftMouse.Y)));
                SGIS.app.setStatusText(s.Count + " objects");
                foreach (Feature f in s)
                {
                    f.selected = true;
                    l.selected.Add(f);
                }
            }
        }

        public override void render(Graphics g)
        {
            base.render(g);

            if (leftMouseDown)
            {
                Pen pen = new Pen(Color.Black);
                g.DrawRectangle(pen, new System.Drawing.Rectangle(
                        Math.Min(mouse.X, leftMouse.X),
                        Math.Min(mouse.Y, leftMouse.Y),
                        Math.Abs(leftMouse.X - mouse.X),
                        Math.Abs(leftMouse.Y - mouse.Y)));
            }
        }
    }

    public class InfoMouseTactic : MouseTactic
    {
        public override void MouseUp(MouseEventArgs e)
        {
            base.MouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                Layer l = (Layer)SGIS.app.getLayerList().SelectedItem;
                if (l == null)
                    return;

                foreach (Feature f in l.selected)
                    f.selected = false;
                l.selected.Clear();

                Feature s = l.getClosest(SGIS.app.screenManager.MapScreenToReal(mouse), 5 / SGIS.app.screenManager.Scale.X);
                if (s == null)
                {
                    SGIS.app.setStatusText("");
                    return;
                }

                s.selected = true;
                l.selected.Add(s);

                SGIS.app.setStatusText("ID: " + s.id);
                SGIS.app.redraw();

                var menupos = SGIS.app.getMapWindow().PointToScreen(mouse);
                SGIS.app.getInfoMenu().Show(menupos);
            }
        }
    }
}
