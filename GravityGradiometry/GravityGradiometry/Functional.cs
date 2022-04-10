namespace GravityGradiometry
{
    public interface IFunctional
    {
        public double Calculate(double[] p);
    }

    public class Functional : IFunctional
    {
        private Point[] receivers;
        private int n;
        private int K;
        private int xCellCount;
        private int zCellCount;
        private double alpha;
        private double[] gamma;
        private double[] g;
        private double[] realG;
        private GravityCalculator calc;


        public Functional(ProblemInfo info)
        {
            receivers = info.Receivers;
            n = info.Receivers.Length;
            K = info.P.Length;
            xCellCount = info.X.Length - 1;
            zCellCount = info.Z.Length - 1;
            alpha = info.Alpha;
            gamma = info.Gamma;
            g = new double[n];
            realG = info.realG;
            calc = new GravityCalculator(info.X, info.Z, null);
        }

        public double Calculate(double[] p)
        {
            calc.P = p;
            double result1 = 0.0;
            double result2 = 0.0;
            double result3 = 0.0;

            for (int i = 0; i < n; i++)
                g[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < n; i++)
                result1 += (realG[i] - g[i]) * (realG[i] - g[i]);

            for (int k = 0; k < K; k++)
            {
                result2 += p[k] * p[k];

                double value = 0.0;
                foreach (var m in Mesh.GetAdjacentCells(k, xCellCount, zCellCount))
                    value += (p[k] - p[m]) * (p[k] - p[m]);

                result3 += value * gamma[k];
            }

            return result1 + alpha * result2 + result3;
        }
    }
}
