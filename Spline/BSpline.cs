﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spline
{
    /// <summary>
    /// Implementation for B-spline
    /// </summary>
    public class BSpline
    {
        private Point3d[] points;
        private KnotSet kontVector;
        private int degree;
        public BSpline(IEnumerable<IEnumerable<double>> pts, KnotSet knots, int degree)
        {
            var pointList = new List<Point3d>();

            foreach (var pt in pts)
            {
                Point3d pt3d = new Point3d(pt.ToArray());
                pointList.Add(pt3d);
            }

            this.points = pointList.ToArray();
            this.kontVector = (KnotSet)knots.Clone();
            this.degree = degree;
        }

        public double[] GetPoint(double t)
        {
            Point3d result = new Point3d(0, 0, 0);

            int n = points.Length - 1;

            for (int i = 0; i <= n; i++)
            {
                Point3d pt = points[i];
                double val = GetBasis(i, degree, this.kontVector, t);

                result = result + val * pt;
            }
            return new double[] { result.X, result.Y, result.Z };
        }

        /// <summary>
        /// Cox-de Boor recursion formula implementation. 
        /// </summary>
        private double GetBasis(int i, int j, KnotSet knots, double t)
        {
            double t_i = knots.GetIndexOf(i);
            double t_iplus1 = knots.GetIndexOf(i + 1);

            if (j == 0)
            {
                if (t_i <= t && t < t_iplus1)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }

            }
            double coef1 = GetCoef(i, j, t, knots);
            double coef2 = 1 - GetCoef(i + 1, j, t, knots);

            double firstTerm = coef1 * GetBasis(i, j - 1, knots, t);
            double secondTerm = coef2 * GetBasis(i + 1, j - 1, knots, t);

            return firstTerm + secondTerm;
        }

        /// <summary>
        /// Cox-de Boor recursion formula implementation. 
        /// </summary>
        private double GetCoef(int i, int j, double t, KnotSet knots)
        {
            if (knots.GetIndexOf(i + j) == knots.GetIndexOf(i))
            {
                return 0;
            }

            return (double)(t - knots.GetIndexOf(i)) / (double)(knots.GetIndexOf(i + j) - knots.GetIndexOf(i));
        }

        private struct Point3d
        {
            public double X { get; }
            public double Y { get; }
            public double Z { get; }

            public Point3d(double x, double y, double z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }

            public Point3d(double[] xyz)
            {
                this.X = xyz[0];
                this.Y = xyz[1];
                this.Z = xyz[2];
            }

            public static Point3d operator *(Point3d pt, double val)
            {

                return new Point3d(pt.X * val, pt.Y * val, pt.Z * val);
            }

            public static Point3d operator *(double val, Point3d pt)
            {

                return new Point3d(pt.X * val, pt.Y * val, pt.Z * val);
            }

            public static Point3d operator +(Point3d pt1, Point3d pt2)
            {

                return new Point3d(pt1.X + pt2.X, pt1.Y + pt2.Y, pt1.Z + pt2.Z);
            }

            public override string ToString()
            {
                return $"X:{X}, Y:{Y}, Z:{Z}";
            }
        }
    }
}