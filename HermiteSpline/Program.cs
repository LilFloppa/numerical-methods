using HermiteSpline.Meshes;
using HermiteSpline.Spline;
using MathUtilities;
using System;
using System.IO;

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

        static (Point p, double f)[] GetRegularData()
        {
            return new (Point p, double f)[]
            {
                (new Point { X = 0.1, Y = 0.0 }, 0.9),
                (new Point { X = 0.3, Y = 0.0 }, 0.3),
                (new Point { X = 0.5, Y = 0.0 }, 0.5),
                (new Point { X = 0.7, Y = 0.0 }, 0.7),
                (new Point { X = 0.9, Y = 0.0 }, 0.9),
            };
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
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo ?? throw new InvalidCastException();
            culture.NumberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

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
            var points = FunctionDataGenerator(12, 0.0, 1.0, 0.0, 1.0, (double x, double y) => x);
            //var points = GetRegularData();
            //var points = RandomDataGenerator(50, 0.0, 1.0, 0.0, 0.0);

            File.Delete("C:/repos/data/data.txt");
            using (var writer = new StreamWriter(File.OpenWrite("C:/repos/data/data.txt")))
            {
                foreach (var p in points)
                {
                    Console.WriteLine($"{p.p.X} {p.f}");
                    writer.WriteLine($"{p.p.X} {p.f}");
                }
            }

            splineBuilder.Mesh = builder.Build(options);
            splineBuilder.Alpha = 0.001;
            splineBuilder.Beta = 0.001;
            splineBuilder.SetData(points);

            ISpline spline = splineBuilder.Build();
            File.Delete("C:/repos/data/spline.txt");
            using (var writer = new StreamWriter(File.OpenWrite("C:/repos/data/spline.txt")))
            {
                for (int i = 0; i < 1000; i++)
                {
                    Point p = new Point { X = i * 0.001, Y = 0.1 };
                    Console.WriteLine($"{p.X}, {spline.GetValue(p)}");
                    writer.WriteLine($"{p.X} {spline.GetValue(p)}");
                }
            }
        }
    }
}
