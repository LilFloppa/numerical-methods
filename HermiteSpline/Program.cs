using HermiteSpline.Meshes;
using HermiteSpline.Spline;
using MathUtilities;
using System;

namespace HermiteSpline
{
    class Program
    {
        static (Point p, double f)[] RandomDataGenerator(int n, double x1, double x2, double y1, double y2)
        {
            (Point p, double f)[] data = new (Point p, double f)[n];
            Random random = new Random();

            for (int i = 0; i < n; i++)
                data[i] = (new Point { X = (x2 - x1) * random.NextDouble() + x1, Y = (y2 - y1) * random.NextDouble() + y1 }, random.NextDouble());

            return data;
        }

        static (Point p, double f)[] ConstDataGenerator(int n, double x1, double x2, double y1, double y2, double value)
        {
            (Point p, double f)[] data = new (Point p, double f)[n];
            Random random = new Random();

            for (int i = 0; i < n; i++)
                data[i] = (new Point { X = (x2 - x1) * random.NextDouble() + x1, Y = (y2 - y1) * random.NextDouble() + y1 }, value);

            return data;
        }

        static (Point p, double f)[] FunctionDataGenerator(int n, double x1, double x2, double y1, double y2, Func<double, double, double> f)
        {
            (Point p, double f)[] data = new (Point p, double f)[n];
            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                double x = (x2 - x1) * random.NextDouble() + x1;
                double y = (y2 - y1) * random.NextDouble() + y1;
                data[i] = (new Point { X = x, Y = y }, f(x, y));
            }

            return data;
        }

        static Point GetRandomPoint(double x1, double x2, double y1, double y2)
        {
            Random random = new Random();

            return new Point
            {
                X = (x2 - x1) * random.NextDouble() + x1,
                Y = (y2 - y1) * random.NextDouble() + y1
            };
        }

        static void Main(string[] args)
        {
            MeshBuilder builder = new MeshBuilder();

            MeshOptions options = new MeshOptions
            {
                X1 = 0.0,
                X2 = 1.0,
                XN = 2,
                Y1 = 0.0,
                Y2 = 1.0,
                YN = 2
            };

            HermiteSplineBuilder splineBuilder = new HermiteSplineBuilder();
            var points = FunctionDataGenerator(50, 0.0, 1.0, 0.0, 1.0, (double x, double y) => x);

            splineBuilder.Mesh = builder.Build(options);
            splineBuilder.Alpha = 0.0;
            splineBuilder.Beta = 0.001;
            splineBuilder.SetData(points);

            ISpline spline = splineBuilder.Build();

            for (int i = 0; i < 10; i++)
            {
                Point p = new Point { X = i * 0.1, Y = 1.0 };
                Console.WriteLine($"({p.X:0.####}, {p.Y:0.####}): {spline.GetValue(p):0.####}");
            }
        }
    }
}
