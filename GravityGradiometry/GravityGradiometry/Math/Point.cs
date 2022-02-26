using System;

namespace GravityGradiometry
{
    class Point
    {
        public double X { get; set; }
        public double Z { get; set; }

        public Point(double x, double z)
        {
            X = x;
            Z = z;
        }

        public double Distance(Point p) => Math.Sqrt((X - p.X) * (X - p.X) + (Z - p.Z) * (Z - p.Z));
    }
}
