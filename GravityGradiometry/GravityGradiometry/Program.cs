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

            const int xCellCount = 9;
            const int zCellCount = 6;
            const int k = xCellCount * zCellCount;
            const int n = 800;

            double[] x = new double[xCellCount + 1];
            double[] z = new double[zCellCount + 1];
            double[] p = new double[k];
            double[] gamma = new double[k];
            double alpha = 1.0e-5;

            double xbegin = 0.0;
            double xstep = 500.0;
            for (int i = 0; i < xCellCount + 1; i++)
                x[i] = xbegin + xstep * i;

            double zbegin = -1500.0;
            double zstep = 250.0;
            for (int i = 0; i < zCellCount + 1; i++)
                z[i] = zbegin + zstep * i;

            p[23] = 1.0;
            p[24] = 1.0;
            p[32] = 1.0;
            p[33] = 1.0;

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
