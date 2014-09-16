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
            mouse = SGIS.App.MousePosition;
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
            SGIS.App.MapWindow.Focus();
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
            mouse = SGIS.App.MousePosition;
            updateLabel();
        }
        public virtual void MouseWheel(MouseEventArgs e)
        {
            var oldGeoPos = SGIS.App.ScreenManager.MapScreenToReal(mouse);
            double scale = 1.3;
            if (e.Delta > 0)
            { // Zoom in
                if (SGIS.App.ScreenManager.RealRect.Width < 5)
                    return;
                SGIS.App.ScreenManager.RealRect.Grow(1 / scale);
            }
            else if (e.Delta < 0) // Zoom out
                SGIS.App.ScreenManager.RealRect.Grow(scale);
            SGIS.App.ScreenManager.Calculate();
            var newGeoPos = SGIS.App.ScreenManager.MapScreenToReal(mouse);
            SGIS.App.ScreenManager.ScrollReal(new NTSPoint(oldGeoPos.X - newGeoPos.X, oldGeoPos.Y - newGeoPos.Y));
            SGIS.App.ScreenManager.Calculate();
            SGIS.App.redraw();
        }
        public virtual void render(System.Drawing.Graphics g)
        {
        }

        public virtual void updateLabel()
        {
            var realP = SGIS.App.ScreenManager.MapScreenToReal(mouse);
            SGIS.App.CoordText = String.Format("Coords: [{0:F3}, {1:F3}]", realP.X, realP.Y);
        }
    }

    public class MoveMouseTactic : MouseTactic
    {
        public override void MouseMove(MouseEventArgs e)
        {
            base.MouseMove(e);

            if (leftMouseDown)
            {
                SGIS.App.ScreenManager.ScrollScreen(new NTSPoint(oldMouse.X - mouse.X, oldMouse.Y - mouse.Y));
                SGIS.App.ScreenManager.Calculate();
                SGIS.App.redraw();
            }
        }
    }

    public class SelectMouseTactic : MouseTactic
    {
        public override void MouseMove(MouseEventArgs e)
        {
            base.MouseMove(e);

            if (leftMouseDown)
                SGIS.App.redraw();
        }
        public override void MouseUp(MouseEventArgs e)
        {
            base.MouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                Layer l = (Layer)SGIS.App.getLayerList().SelectedItem;
                if (l == null)
                    return;

                foreach (Feature f in l.Selected)
                    f.Selected = false;
                l.Selected.Clear();

                List<Feature> s = l.getWithin(SGIS.App.ScreenManager.MapScreenToReal(new Envelope(mouse.X, leftMouse.X, mouse.Y, leftMouse.Y)));
                SGIS.App.StatusText = (s.Count + " objects");
                foreach (Feature f in s)
                {
                    f.Selected = true;
                    l.Selected.Add(f);
                }
            }
            SGIS.App.redraw();
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
                Layer l = (Layer)SGIS.App.getLayerList().SelectedItem;
                if (l == null)
                    return;

                foreach (Feature f in l.Selected)
                    f.Selected = false;
                l.Selected.Clear();

                Feature s = l.getClosest(SGIS.App.ScreenManager.MapScreenToReal(mouse), 5 / SGIS.App.ScreenManager.Scale.X);
                if (s == null)
                {
                    SGIS.App.StatusText = ("");
                    SGIS.App.redraw();
                    return;
                }

                s.Selected = true;
                l.Selected.Add(s);

                SGIS.App.StatusText = ("ID: " + s.ID);
                SGIS.App.redraw();

                var menupos = SGIS.App.MapWindow.PointToScreen(mouse);
                SGIS.App.InfoMenu.Show(menupos);
            }
        }
    }
}
