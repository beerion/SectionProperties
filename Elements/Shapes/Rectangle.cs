﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ExcelExpress.ComplexShape.SectionProperties
{
    public partial class SectionElements
    {
        public class Rectangle : Shape
        {
            public override IsoMaterial Material { get; set; }
            public override double theta { get; set; }
            public override bool mirrorX { get; set; } = false;
            public override bool mirrorY { get; set; } = false;

            public override string point { get; set; }
            public override double xp { get; set; }
            public override double yp { get; set; }

            public static List<string> _pointlist = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "cg" };
            public override List<string> ShapePointList { get { return _pointlist; } }

            [Dimension("Width")]
            public double b     { get; set; }
            [Dimension("Thickness")]
            public double t     { get; set; }
            
            
            public override void Draw(ref Bitmap bitmap, PlotProperties plotprops)
            {
                double theta_degree = theta * 180 / Math.PI;
                double SF = ImageUtil.CalculateScaleFactor(bitmap, plotprops);

                float _b = (float)(b * SF);
                float _t = (float)(t * SF);

                int wdth = bitmap.Width;
                int hght = bitmap.Height;

                Graphics g = Graphics.FromImage(bitmap);

                GraphicsPath path = new GraphicsPath();
                path.AddLine(0, 0, 0, _t);
                path.AddLine(0, _t, _b, _t);
                path.AddLine(_b, _t, _b, 0);
                path.AddLine(_b, 0, 0, 0);
                path.CloseFigure();

                PointF pnt = CreateImagePoint(SF);

                Coordinate xy_min = ImageUtil.CalculateImageXYmin(bitmap, plotprops);

                double Xdel = xp - xy_min.x;
                double Ydel = yp - xy_min.y;

                Matrix mtrx = ImageUtil.ImageTransformationMatrix(bitmap, pnt, theta_degree, Xdel, Ydel, SF, mirrorX, mirrorY);
                path.Transform(mtrx);

                Color _fillColor = Material.ConvertFillColor();

                Pen _pen = new Pen(borderColor, borderThickness);
                _pen.Alignment = PenAlignment.Inset;
                g.FillPath(new SolidBrush(_fillColor), path);
                g.DrawPath(_pen, path);

                _pen.Dispose();
                g.Dispose();
            }

            

            protected override SecProp ShapeSecProp()
            {
                SecProp sp = new SecProp();

                double E = Material.E;

                sp.EA = E * b * t;
                sp.EIxx = E * 1.0 / 12.0 * b * Math.Pow(t, 3);
                sp.EIyy = E * 1.0 / 12.0 * t * Math.Pow(b, 3);
                sp.EIxy = 0;
                sp.Xcg = b / 2.0;
                sp.Ycg = t / 2.0;

                return sp;

            }           


            protected override Coordinate LocalPointCoordinate(string PointID)
            {
                //Returns the coordinate of a defined point 
                //assumes zero rotation and translation
                Coordinate point1_sh = new Coordinate();

                switch (PointID)
                {
                    case "a":
                        point1_sh = ConvertXYtoCoordinate(0, 0);
                        break;
                    case "b":
                        point1_sh = ConvertXYtoCoordinate(0, t/2);
                        break;
                    case "c":
                        point1_sh = ConvertXYtoCoordinate(0, t);
                        break;
                    case "d":
                        point1_sh = ConvertXYtoCoordinate(b / 2, t);
                        break;
                    case "e":
                        point1_sh = ConvertXYtoCoordinate(b, t);
                        break;
                    case "f":
                        point1_sh = ConvertXYtoCoordinate(b, t/2);
                        break;
                    case "g":
                        point1_sh = ConvertXYtoCoordinate(b, 0);
                        break;
                    case "h":
                        point1_sh = ConvertXYtoCoordinate(b / 2, 0);
                        break;
                    case "cg":
                        point1_sh = ConvertXYtoCoordinate(b / 2, t / 2);
                        break;
                }

                return point1_sh;

            }

            

        }

        
    }
}
