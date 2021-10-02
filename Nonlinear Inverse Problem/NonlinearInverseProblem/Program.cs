using System;
using System.Collections.Generic;
using System.Numerics;
using System.IO;

namespace NonlinearInverseProblem
{
    class Program
    {
        static void LinearProblem()
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

        static void NonlinearProblem()
        {
            double eps = 1.0e-15;
            double sigmaEps = 0.001;
            Source s = new Source(new Vector2(0, 0), new Vector2(100, 0), 5.0);
            Receiver r1 = new Receiver(new Vector2(800, 0));
            Receiver r2 = new Receiver(new Vector2(900, 0));

            double trueSigma = 0.1;

            Problem.ProblemInfo info = new Problem.ProblemInfo
            {
                Source = s,
                Receivers = new List<Receiver>() { r1, r2 },
                h = 300,
                sigma1 = trueSigma,
                sigma2 = 0.2,
                SolverType = SlaeSolver.SolverTypes.LOSLU
            };

            using (var writer = new StreamWriter(File.OpenWrite("sigma.txt")))
            {
                double[] TrueV = Problem.DirectProblem(info, eps);

                double sigma0 = 4.0;

                writer.WriteLine(sigma0);
                Console.WriteLine($"new sigma { sigma0 }");
                info.sigma1 = sigma0;
                double[] V0 = Problem.DirectProblem(info, eps);

                double J = Problem.Functional(V0, TrueV);

                while (Math.Abs(sigma0 - trueSigma) > sigmaEps)
                {
                    double ds = Problem.InverseProblem(info, V0, TrueV, eps, 0.05 * sigma0);

                    double b = 1.0;
                    double sigmaI = sigma0 + b * ds;

                    info.sigma1 = sigmaI;
                    V0 = Problem.DirectProblem(info, eps);

                    Console.WriteLine("Вычисление новой sigma");
                    int k = 0;
                    while (b > 1.0e-5 && Problem.Functional(V0, TrueV) >= J)
                    {
                        Console.WriteLine($"Итерация: { k++ }, sigmaI: { sigmaI } ");
                        b /= 2;
                        sigmaI = sigma0 + b * ds;
                        info.sigma1 = sigmaI;
                        V0 = Problem.DirectProblem(info, eps);
                    }

                    if (b <= 1.0e-5)
                    {
                        Console.WriteLine($"b is too small. Last sigma: { sigma0 } ");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"New sigma: { sigmaI }");
                        sigma0 = sigmaI;
                        writer.WriteLine(sigma0);
                    }
                }
            }
        }

        static void Main(string[] args)
        {
            NonlinearProblem();
        }
    }
}
