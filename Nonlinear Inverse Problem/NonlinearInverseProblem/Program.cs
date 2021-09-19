using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace NonlinearInverseProblem
{
    class Program
    {
        static void Main(string[] args)
        {
            double eps = 1.0e-15;
            Source s = new Source(new Vector2(0, 0), new Vector2(100, 0), 1.6);
            Receiver r1 = new Receiver(new Vector2(800, 0));
            Receiver r2 = new Receiver(new Vector2(900, 0));

            Problem.ProblemInfo info = new Problem.ProblemInfo
            {
                Source = s,
                Receivers = new List<Receiver>() { r1, r2 },
                h = 300,
                sigma1 = 1.0,
                sigma2 = 2.0,
                SolverType = SlaeSolver.SolverTypes.LOSLU
            };

            double[] TrueV = Problem.DirectProblem(info, eps);

            double I0 = 5.0;
            info.Source.I = 5.0;
            double[] V0 = Problem.DirectProblem(info, eps);

            double dI = Problem.InverseProblemLinear(info, V0, TrueV, eps, 0.05 * I0);

            double I1 = I0 + dI;
            Console.WriteLine(I1);
        }
    }
}
