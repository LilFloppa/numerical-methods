using GravityGradiometry;

namespace UI.Models
{
    public class Problem
    {
        public Grid Grid { get; set; }
        public Regularization Regularization { get; set; }


        public Problem(Grid grid, Regularization regularization)
        {
            Grid = grid;
            Regularization = regularization;
        }

        public double[] Solve()
        {
            int n = Grid.Receivers.Length;
            int k = Grid.Properties.Length;

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(Grid.X, Grid.Z, Grid.Properties);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(Grid.Receivers[i]);

            double[] initialProperties = new double[k];
            for (int i = 0; i < k; i++)
                initialProperties[i] = 1.0;

            double alpha = Regularization.Alpha;
            double[] gamma = Regularization.Gamma;

            ProblemInfo info = new ProblemInfo();
            info.P = initialProperties;
            info.X = Grid.X;
            info.Z = Grid.Z;
            info.Receivers = Grid.Receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(initialProperties);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] solution = solver.Solve(A, b);

            f = F.Calculate(solution);

            return solution;
        }
    }
}
