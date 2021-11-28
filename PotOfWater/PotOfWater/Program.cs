using MathUtilities;
using System;

namespace PotOfWater
{
    class Program
    {
        static void GaussTest()
        {
            double result = Quadratures.TriangleGauss18(new Point(1, 1), new Point(3, 1), new Point(2, 3), (double x, double y) => x * x * x * x * x * x * x * x);
            Console.WriteLine(result);
        }

        static void Main(string[] args)
        {
           
        }
    }
}
