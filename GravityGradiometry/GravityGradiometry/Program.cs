using System;

namespace GravityGradiometry
{

    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo ?? throw new InvalidCastException();
            culture.NumberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            const int xCount = 4;
            const int zCount = 4;
            const int k = (xCount - 1) * (zCount - 1);
            const int n = 800;

            double[] x = new double[xCount] { 2000.0, 2300.0, 2700.0, 3000.0 };
            double[] z = new double[zCount] { -1000.0, -800, -600.0, -500.0 };
            double[] p = new double[k] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };
            double[] gamma = new double[k] { 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10, 1.0e-10 };
            double alpha = 0.0;

            Point[] receivers = new Point[n];

            double beginX = -2000;
            double endX = 6000;
            double stepX = (endX - beginX) / (n - 1);

            for (int i = 0; i < n; i++)
                receivers[i] = new Point(beginX + i * stepX, 0.0);

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(x, z, p);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < k; i++)
                p[i] = 0.1;

            // for (int i = 0; i < n; i++)
            //     Console.WriteLine($"{receivers[i].X}; {realG[i]}");

            ProblemInfo info = new ProblemInfo();
            info.P = p;
            info.X = x;
            info.Z = z;
            info.Receivers = receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(p);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] newP = solver.Solve(A, b);

            f = F.Calculate(newP);
        }
    }
}
