using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using NTSPoint = NetTopologySuite.Geometries.Point;

namespace SGIS
{
    // abstract class for handling mouse input
    public abstract class MouseTactic
    {
        protected System.Drawing.Point oldMouse;
        protected System.Drawing.Point mouse;
        protected System.Drawing.Point rightMouse;
        protected System.Drawing.Point leftMouse;
        protected bool leftMouseDown = false;
        protected bool rightMouseDown = false;

        public virtual void MouseDown(System.Windows.Forms.MouseEventArgs e)
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
        public virtual void MouseUp(System.Windows.Forms.MouseEventArgs e)
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
        public virtual void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            oldMouse = mouse;
            mouse = SGIS.App.MousePosition;
            // updates coordinates on bottom line
            updateLabel();
        }
        public virtual void MouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            // saves previous real world mouse position
            var oldGeoPos = SGIS.App.ScreenManager.MapScreenToReal(mouse);
            double scale = 1.3;
            if (e.Delta > 0)
            { // Zoom in
                if (Math.Abs(SGIS.App.ScreenManager.Scale.X) < 176386645655)
                    SGIS.App.ScreenManager.RealRect.Grow(1 / scale);
            }
            else if (e.Delta < 0) // Zoom out
                SGIS.App.ScreenManager.RealRect.Grow(scale);
            SGIS.App.ScreenManager.Calculate();
            var newGeoPos = SGIS.App.ScreenManager.MapScreenToReal(mouse);
            // ensures new real world coordinates is equal to the previous
            // this gives intuitive zooming
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

    // mouse tactic used for moving the map
    // mouseUp and MouseMove is overridden, but calls base functions
    public class MoveMouseTactic : MouseTactic
    {
        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(e);

            if (e.Button == MouseButtons.Left)
            {
                SGIS.App.ScreenManager.ScrollScreen(new NTSPoint(oldMouse.X - mouse.X, oldMouse.Y - mouse.Y));
                SGIS.App.ScreenManager.Calculate();
                SGIS.App.redrawDirty();
            }
        }

        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                SGIS.App.redraw();
            }
        }
    }

    // mouse tactic for selecting features
    // render, mouseUp and MouseMove is overridden, but calls base functions
    public class SelectMouseTactic : MouseTactic
    {
        public override void MouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseMove(e);

            if (leftMouseDown)
                SGIS.App.redrawDirty();
        }
        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.MouseUp(e);

            if (e.Button == MouseButtons.Left)
            {
                Layer l = (Layer)SGIS.App.getLayerList().SelectedItem;
                if (l == null)
                    return;

                bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
                bool alt = (Keyboard.Modifiers & ModifierKeys.Alt) > 0;
                if (!ctrl && !alt)
                {
                    foreach (Feature f in l.Selected)
                        f.Selected = false;
                    l.Selected.Clear();
                }

                List<Feature> s = l.getWithin(SGIS.App.ScreenManager.MapScreenToRealGeometry(new Envelope(mouse.X, leftMouse.X, mouse.Y, leftMouse.Y)));
                
                foreach (Feature f in s)
                {
                    if (!ctrl && !alt)
                    {
                        f.Selected = true;
                        l.Selected.Add(f);
                    }
                    else if (ctrl && !alt && !f.Selected)
                    {
                        f.Selected = true;
                        l.Selected.Add(f);
                    }
                    else if (!ctrl && alt && f.Selected)
                    {
                        f.Selected = false;
                        l.Selected.Remove(f);
                    }
                }

                SGIS.App.fireSelectionChanged();
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

    // mouse tactic used for getting info of features
    // mouseUp is overridden, but calls base function
    public class InfoMouseTactic : MouseTactic
    {
        public override void MouseUp(System.Windows.Forms.MouseEventArgs e)
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
                if (s != null)
                {
                    s.Selected = true;
                    l.Selected.Add(s);

                    var menupos = SGIS.App.MapWindow.PointToScreen(mouse);
                    SGIS.App.InfoMenu.Show(menupos);
                }

                SGIS.App.fireSelectionChanged();
                SGIS.App.redraw();
            }
        }
    }
}
