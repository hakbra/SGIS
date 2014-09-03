using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoLib
{
    public enum ShapeType
    {
        POINT,
        LINE,
        POLYGON,
        POLYLINE
    }
    public interface ShapeInterface
    {
        void Draw(CGeoDraw render, Graphics g, Color c);
        ShapeType getType();
        C2DRect boundingBox();
        double distanceTo(C2DPoint p);
    }

    public class Point : C2DPoint, ShapeInterface
    {
        public Point(double dx, double dy) : base(dx, dy) { }
        public Point(System.Drawing.Point p) : base(p.X, p.Y) { }

        public Point() : base() { }
        public void Draw(CGeoDraw render, Graphics g, Color c)
        {
            Brush b = new SolidBrush(c);
            render.Draw(this, g, b);
        }
        public ShapeType getType() { return ShapeType.POINT; }
        public C2DRect boundingBox() { return new C2DRect(this); }
        public double distanceTo(C2DPoint p) { return Distance(p); }
    }

    public class Line : C2DLine, ShapeInterface
    {
        public Line(C2DPoint a, C2DPoint b) : base(a, b) { }
        public void Draw(CGeoDraw render, Graphics g, Color c)
        {
            Pen p = new Pen(c);
            render.Draw(this, g, p);
        }
        public ShapeType getType() { return ShapeType.LINE; }
        public C2DRect boundingBox() { C2DRect bb = new C2DRect(); GetBoundingRect(bb); return bb; }
        public double distanceTo(C2DPoint p) { return Distance(p); }
    }

    public class Polygon : C2DHoledPolygon, ShapeInterface
    {
        public Polygon(params C2DPoint[] points) : base(new C2DPolygon(points.ToList<C2DPoint>(), false)) { }
        public void Draw(CGeoDraw render, Graphics g, Color c)
        {
            Brush b = new SolidBrush(c);
            render.DrawFilled(this, g, b);
        }
        public ShapeType getType() { return ShapeType.POLYGON; }
        public C2DRect boundingBox() { C2DRect bb = new C2DRect(); GetBoundingRect(bb); return bb; }
        public double distanceTo(C2DPoint p) { return Distance(p); }
    }

    public class PolyLine : ShapeInterface
    {
        List<Line> lines = new List<Line>();
        public PolyLine(params Point[] points)
        {
            for (int i = 1; i < points.Count(); i++ )
                lines.Add(new Line(points[i - 1], points[i]));
        }
        public void Draw(CGeoDraw render, Graphics g, Color c)
        {
            foreach (Line l in lines)
                l.Draw(render, g, c);
        }
        public ShapeType getType() { return ShapeType.POLYLINE; }
        public C2DRect boundingBox()
        {
            C2DRect boundingbox = null;
            foreach (Line s in lines)
            {
                if (boundingbox == null)
                    boundingbox = s.boundingBox();
                else
                    boundingbox.ExpandToInclude(s.boundingBox());
            }
            return boundingbox;
        }
        public double distanceTo(C2DPoint p)
        {
            Double dist = Double.MaxValue;
            foreach (Line l in lines)
            {
                double d = l.Distance(p);
                if (d < dist)
                    dist = d;
            }
            return dist;
        }
    }

    public class Shape : ShapeInterface
    {
        ShapeInterface shape;
        public int id;

        public Shape(ShapeInterface s, int i)
        {
            shape = s;
            id = i;
        }
        public void Draw(CGeoDraw render, Graphics g, Color c){shape.Draw(render, g, c);}
        public ShapeType getType(){return shape.getType();}
        public C2DRect boundingBox() { return shape.boundingBox(); }
        public double distanceTo(C2DPoint p) { return shape.distanceTo(p); }
    }
}
