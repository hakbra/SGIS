using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGIS
{
    /// <summary>
    /// Class which can be used to help calculate drawing offsets and scales. 
    /// </summary>
    public class ScreenManager
    {
        public class SGISEnvelope : Envelope
        {
            public SGISEnvelope(double x1, double x2, double y1, double y2) : base(x1, x2, y1, y2) { }
            public SGISEnvelope(Envelope e) : base(e) { }
            public void Grow(double factor) {
                this.Grow(factor, factor);
            }
            public void GrowWidth(double factor)
            {
                this.Grow(factor, 1);
            }
            public void GrowHeight(double factor){
                this.Grow(1, factor);
            }
            public void Grow(double xf, double yf) {
                double xinc = this.Width * xf - this.Width;
                double yinc = this.Height * yf - this.Height;

                this.ExpandBy(xinc / 2, yinc / 2);
            }
            public void Move(Point p)
            {
                this.Translate(p.X, p.Y);
            }
            public void Set(Envelope e)
            {
                Init(e);
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        public ScreenManager()
        {
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ScreenManager()
        {
        }

        /// <summary>
        /// This simply grows the real world rectangle.
        /// </summary>
        /// <param name="dFactor">Factor to zoom out by.</param>
        public void ZoomOut(double dFactor)
        {
	        RealRect.Grow(dFactor);
        }

        /// <summary>
        /// This simply moves the real world rectangle.
        /// </summary>
        /// <param name="Vector">Vector to move / scroll by</param>
        public void ScrollReal(Point Vector)
        {
	        RealRect.Move(Vector);
        }

        /// <summary>
        /// This simply moves the real world rectangle but by a scaled, screen based
        /// value e.g. 10 pixels to the right. The image is shown in the same place on
        /// the screen but the image scrolls.
        /// </summary>
        /// <param name="Vector">Vector to scroll / move by.</param>
        public void ScrollScreen(Point v)
        {
            RealRect.Move(new Point(v.X/Scale.X, v.Y/Scale.Y));
        }

        /// <summary>
        /// Calculates all the required offset and scale. The real world rect will 
        /// always all be shown but may have to occupy a sub section of the computer
        /// screen.
        /// </summary>
        public void Calculate()
        {
	        RealWindowsRect.Set(RealRect);

	        float fWinRatio = (float)WindowsRect.Width / (float)WindowsRect.Height;
	        float fRealRatio = (float)RealRect.Width / (float)RealRect.Height;
	        bool bFillX = fRealRatio >= fWinRatio;
	        if (bFillX)
	        {
		        Scale.X = (float)WindowsRect.Width / (float)RealRect.Width;
		        Scale.Y = Scale.X;
		        RealWindowsRect.GrowHeight(fRealRatio / fWinRatio);
	        }
	        else
	        {
		        Scale.Y = (float)WindowsRect.Height / (float)RealRect.Height;
		        Scale.X = Scale.Y;
		        RealWindowsRect.GrowWidth(fWinRatio / fRealRatio );
	        }

	        if (FlipX) 
                Scale.X = - Scale.X;
	        if (FlipY) 
                Scale.Y = - Scale.Y;

	        // Now find 2 points to map onto each other and the rest is easy.
	        Point ptRealCen = new Point(RealRect.Centre);
	        Point ptWinCen = new Point( WindowsRect.Centre);

	        Offset.X = ptRealCen.X - (float)ptWinCen.X / Scale.X;
	        Offset.Y = ptRealCen.Y - (float)ptWinCen.Y / Scale.Y;
        }

        /// <summary>
        /// Returns a real world point from a point on the screen.
        /// </summary>
        /// <param name="pt">Point to map</param>
        public Point MapScreenToReal(System.Drawing.Point pt)
        {
            return new Point(Offset.X + (float)pt.X / Scale.X,
					           Offset.Y + (float)pt.Y / Scale.Y);
        }
        public IGeometry MapScreenToRealGeometry(Envelope e)
        {
            var min = MapScreenToReal(new System.Drawing.Point((int)e.MinX, (int)e.MinY));
            var max = MapScreenToReal(new System.Drawing.Point((int)e.MaxX, (int)e.MaxY));
            var re = new Envelope(min.X, max.X, min.Y, max.Y);

            OgcCompliantGeometryFactory fact = new OgcCompliantGeometryFactory();
            return fact.ToGeometry(re);
        }
        public Envelope MapScreenToReal(Envelope e)
        {
            var min = MapScreenToReal(new System.Drawing.Point((int)e.MinX, (int)e.MinY));
            var max = MapScreenToReal(new System.Drawing.Point((int)e.MaxX, (int)e.MaxY));
            var re = new Envelope(min.X, max.X, min.Y, max.Y);
            return re;
        }

        public void ZoomTo(Point p)
        {
            Point diff = new Point(p.X - RealRect.Centre.X, p.Y - RealRect.Centre.Y);
            RealRect.Move(diff);
        }

        /// <summary>
        /// Returns a point on the screen from a real world point.
        /// </summary>
        /// <param name="pt">Point to map.</param>
        public Point MapRealToScreen(Point pt)
        {
	        return new Point(   (int)((pt.X - Offset.X) * Scale.X),
				              (int)((pt.Y - Offset.Y) * Scale.Y)   );

        }
        public System.Drawing.Rectangle MapRealToScreen(Envelope e)
        {
            var min = MapRealToScreen(new Point(e.MinX,e.MinY));
            var max = MapRealToScreen(new Point(e.MaxX,e.MaxY));
            return new System.Drawing.Rectangle((int)min.X, (int)max.Y, (int)(max.X - min.X), (int)(min.Y - max.Y));
        }

        public Point ScaleAndOffSet(Point pt)
        {
            Point p = new Point(0, 0);
            p.X = pt.X - Offset.X;
            p.Y = pt.Y - Offset.Y;
            p.X *= Scale.X;
            p.Y *= Scale.Y;
            return p;
        }

        /// <summary>
        /// The windows rectangle that we are trying to project this onto.
        /// </summary>
        public SGISEnvelope WindowsRect = new SGISEnvelope(0, 0, 0, 0);

        /// <summary>
        /// The real world rectangle that we are viewing.
        /// </summary>
        public SGISEnvelope RealRect = new SGISEnvelope(0, 0, 0, 0);

        /// <summary>
        /// /// The resulting real world rectangle that the screen represents (note that this is the same as the above
        /// so long as the aspect ration is maintained).
        /// </summary>
        public SGISEnvelope RealWindowsRect = new SGISEnvelope(0, 0, 0, 0);

        /// <summary>
        /// True if the Y dimension is to be flipped because e.g. because of Windows.
        /// </summary>
        public bool FlipY = true;
        /// <summary>
        /// True if the Y dimension is to be flipped.
        /// </summary>
        public bool FlipX = false;
        /// <summary>
        /// The offest.
        /// </summary>
        public Point Offset = new Point(0, 0);

        /// <summary>
        /// The scale.
        /// </summary>
        public Point Scale = new Point(0, 0);
    }
}
