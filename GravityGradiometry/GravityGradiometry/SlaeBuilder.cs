namespace GravityGradiometry
{
    struct ProblemInfo
    {
        public Point[] Receivers;
        public double[] realG;
        public double[] X;
        public double[] Z;
        public double[] P;
        public double Alpha;
    }

    class SlaeBuilder
    {
        private Point[] receivers;
        private GravityCalculator calc;
        private double[] realG;
        private int n;
        private int k;
        private double alpha;

        public SlaeBuilder(ProblemInfo info)
        {
            receivers = info.Receivers;
            realG = info.realG;
            n = receivers.Length;
            k = info.P.Length;
            alpha = info.Alpha;
            calc = new GravityCalculator(info.X, info.Z, info.P);
        }

        public void Build(IMatrix A, double[] b)
        {
            for (int q = 0; q < k; q++)
            {
                for (int s = 0; s < k; s++)
                {
                    double value = 0.0;
                    for (int i = 0; i < n; i++)
                        value += calc.CalculateFromOne(receivers[i], q) * calc.CalculateFromOne(receivers[i], s);

                    A.Add(q, s, value);
                }
            }  
            
            for (int q = 0; q < k; q++)
            {
                double bvalue = 0.0;
                for (int i = 0; i < n; i++)
                    bvalue += calc.CalculateFromOne(receivers[i], q) * realG[i];
                b[q] = bvalue;

                A.Add(q, q, alpha);
            }
        }
    }
}
