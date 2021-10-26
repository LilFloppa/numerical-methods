using System;

namespace MathUtilities
{
    public class Double
    {
        public static readonly double Eps = 1.0e-9;
        public static bool IsEqual(double a, double b) => Math.Abs(a - b) < Eps;
    }
}
