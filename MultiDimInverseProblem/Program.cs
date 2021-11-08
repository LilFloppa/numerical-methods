using FEM;
using MathUtilities;
using SlaeSolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using static MultiDimInverseProblem.Problem;

namespace MultiDimInverseProblem
{
    class Program
    {
        static FEMrz LoadSolution()
        {
            var elements = JsonSerializer.Deserialize<List<FiniteElement>>(File.ReadAllText("C:/repos/Elements.txt"));
            var points = JsonSerializer.Deserialize<Point[]>(File.ReadAllText("C:/repos/Points.txt"));
            var weights = JsonSerializer.Deserialize<double[]>(File.ReadAllText("C:/repos/Weights.txt"));

            FEMrz fem = new FEMrz(new FEMProblemInfo { Points = points, Mesh = new Mesh { Elements = elements } });
            fem.Weights = weights;

            return fem;
        }

        static void GenerateSolution()
        {
            FEMrz fem = Problem.FEM(SolverTypes.LOSLU, 1.0, 1.0, 2.0, 300.0, 1.0e-14);
            File.WriteAllText("C:/repos/Weights.txt", JsonSerializer.Serialize(fem.Weights));
            File.WriteAllText("C:/repos/Mesh.txt", JsonSerializer.Serialize(fem.Info.Mesh.Elements));
            File.WriteAllText("C:/repos/Points.txt", JsonSerializer.Serialize(fem.Info.Points));
        }

        static void PrintV(double[] V)
        {
            foreach (var v in V)
                Console.Write("{0:E2}\t", v);

            Console.WriteLine();
            Console.WriteLine();
        }

        static void PrintI(ProblemInfo info)
        {
            foreach (var s in info.Sources)
                Console.Write("{0:F4}  ", s.I);

            Console.WriteLine();
            Console.WriteLine();
        }

        static void SolveReverseProblem(ProblemInfo info)
        {
            double J = Functional(info, info.RealV);

            // Console.WriteLine("Real V:");
            // PrintV(info.RealV);

            bool flag = true;
            // PrintI(info);
            // Console.WriteLine("Current V:");
            // PrintV(info.CurrentV);

            while (flag && J > 8.0e-7)
            {
                (J, flag) = InverseProblemStep(info, J);
                // PrintI(info);
                // Console.WriteLine("Current V:");
                // PrintV(info.CurrentV);
            }

            Console.WriteLine($"Alpha = {info.Alpha}");
            PrintI(info);
        }

        static ProblemInfo BulidProblemInfo(FEMrz fem, double[] I0, double alpha)
        {
            ProblemInfo info = new ProblemInfo
            {
                Receivers = new Receiver[4]
                {
                    new Receiver(new Point(10, 15), new Point(20, 25)),
                    new Receiver(new Point(25, 35), new Point(35, 45)),
                    new Receiver(new Point(30, -5), new Point(40, 5)),
                    new Receiver(new Point(10, 15), new Point(20, 5)),
                },
                Sources = new Source[10]
                {
                    new Source(new Point(0, 10), 0.0, fem),
                    new Source(new Point(0, 30), 10.0, fem),
                    new Source(new Point(0, 50), 0.0, fem),
                    new Source(new Point(0, 70), 0.0, fem),
                    new Source(new Point(0, 90), 0.0, fem),
                    new Source(new Point(0, 110), 0.0, fem),
                    new Source(new Point(0, 130), 0.0, fem),
                    new Source(new Point(0, 150), 0.0, fem),
                    new Source(new Point(0, 170), 0.0, fem),
                    new Source(new Point(0, 190), 0.0, fem),
                },
                Alpha = alpha
            };

            info.RealV = DirectProblem(info);
            for (int i = 0; i < info.Sources.Length; i++)
                info.Sources[i].I = I0[i];
            info.CurrentV = DirectProblem(info);

            return info;
        }

        static void Main(string[] args)
        {
            var fem = LoadSolution();
            double[] I0 = new double[10] { 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0 };

            for (double alpha = 1.0e-14; alpha < 1.0; alpha *= 10)
            {
                var info = BulidProblemInfo(fem, I0, alpha);
                SolveReverseProblem(info);
            }           
        }
    }
}
