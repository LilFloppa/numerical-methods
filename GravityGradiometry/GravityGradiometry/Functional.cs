namespace GravityGradiometry
{
    interface IFunctional
    {
        public double Calculate(double[] p);
    }

    class Functional : IFunctional
    {
        private Point[] receivers;
        private int n;
        private int k;
        private double alpha;
        private double[] g;
        private double[] realG;
        private GravityCalculator calc;


        public Functional(ProblemInfo info)
        {
            receivers = info.Receivers;
            n = info.Receivers.Length;
            k = info.P.Length;
            alpha = info.Alpha;
            g = new double[n];
            realG = info.realG;
            calc = new GravityCalculator(info.X, info.Z, null);
        }


        public double Calculate(double[] p)
        {
            calc.P = p;
            double result1 = 0.0;
            double result2 = 0.0;

            for (int i = 0; i < n; i++)
                g[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < n; i++)
                result1 += (realG[i] - g[i]) * (realG[i] - g[i]);

            for (int i = 0; i < k; i++)
                result2 += p[i] * p[i];

            return result1 + alpha * result2;
        }
    }
}
