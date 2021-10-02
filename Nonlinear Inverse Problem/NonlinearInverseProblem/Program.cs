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
            Random random = new Random();
            double trueI = 1.66;
            double iEps = 0.1;
            double eps = 1.0e-15;
            Source s = new Source(new Vector2(0, 0), new Vector2(100, 0), trueI);
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
            TrueV[0] += TrueV[0] * (0.01 + 0.9 * random.NextDouble());

            double I0 = 5.0;
            info.Source.I = I0;
            double[] V0 = Problem.DirectProblem(info, eps);

            double J = Problem.Functional(V0, TrueV);
            using (var writer = new StreamWriter(File.OpenWrite("I.txt")))
            {
                writer.WriteLine(I0);
                while (Math.Abs(I0 - trueI) > iEps)
                {
                    double dI = Problem.InverseProblemLinear(info, V0, TrueV, eps, 0.05 * I0);

                    double b = 1.0;
                    double Ii = I0 + b * dI;

                    info.Source.I = Ii;
                    V0 = Problem.DirectProblem(info, eps);

                    Console.WriteLine("Вычисление нового I");
                    int k = 0;
                    while (b > 1.0e-5 && Problem.Functional(V0, TrueV) >= J)
                    {
                        Console.WriteLine($"Итерация: { k++ }, Ii: { Ii } ");
                        b /= 2;
                        Ii = I0 + b * dI;
                        info.Source.I = dI;
                        V0 = Problem.DirectProblem(info, eps);
                    }

                    if (b <= 1.0e-5)
                    {
                        Console.WriteLine($"b is too small. Last I: { I0 } ");
                        return;
                    }
                    else
                    {
                        Console.WriteLine($"New I: { Ii }");
                        I0 = Ii;
                        writer.WriteLine(I0);
                    }
                }
            }
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


            double[] TrueV = Problem.DirectProblem(info, eps);

            double sigma0 = 4.0;


            Console.WriteLine($"new sigma { sigma0 }");
            info.sigma1 = sigma0;
            double[] V0 = Problem.DirectProblem(info, eps);

            double J = Problem.Functional(V0, TrueV);
            using (var writer = new StreamWriter(File.OpenWrite("sigma.txt")))
            {
                writer.WriteLine(sigma0);
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
            LinearProblem();
        }
    }
}
