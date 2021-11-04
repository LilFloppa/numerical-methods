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
                Console.Write("{0:E2}\t", s.I);

            Console.WriteLine();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var fem = LoadSolution();

            ProblemInfo info = new ProblemInfo
            {
                Receivers = new Receiver[6]
                { 
                    new Receiver(new Vector2(1100, 0), new Vector2(1110, 0)),
                    new Receiver(new Vector2(1200, 0), new Vector2(1210, 0)), 
                    new Receiver(new Vector2(1300, 0), new Vector2(1310, 0)), 
                    new Receiver(new Vector2(1400, 0), new Vector2(1410, 0)),
                    new Receiver(new Vector2(1500, 0), new Vector2(1510, 0)),
                    new Receiver(new Vector2(1600, 0), new Vector2(1610, 0)),
                },
                Sources = new Source[10]
                {
                    new Source(new Vector2(0, 0), new Vector2(10, 0), 10.0, fem),
                    new Source(new Vector2(110, 0), new Vector2(120, 0), 0.0, fem),
                    new Source(new Vector2(220, 0), new Vector2(230, 0), 0.0, fem),
                    new Source(new Vector2(330, 0), new Vector2(340, 0), 0.0, fem),
                    new Source(new Vector2(440, 0), new Vector2(450, 0), 0.0, fem),
                    new Source(new Vector2(550, 0), new Vector2(560, 0), 0.0, fem),
                    new Source(new Vector2(660, 0), new Vector2(670, 0), 0.0, fem),
                    new Source(new Vector2(770, 0), new Vector2(780, 0), 0.0, fem),
                    new Source(new Vector2(880, 0), new Vector2(890, 0), 0.0, fem),
                    new Source(new Vector2(990, 0), new Vector2(1000, 0), 0.0, fem),
                }
            };

            info.PivotI = new double[10] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            info.RealV = DirectProblem(info);
            for (int i = 0; i < info.Sources.Length; i++)
                info.Sources[i].I = info.PivotI[i];
            info.CurrentV = DirectProblem(info);

            double J = Functional(info, info.RealV);

            Console.WriteLine("Real V:");
            PrintV(info.RealV);

            bool flag = true;
            PrintI(info);
            Console.WriteLine("Current V:");
            PrintV(info.CurrentV);

            while (flag && J > 1.0e-9)
            {
                (J, flag) = InverseProblemStep(info, J);
                PrintI(info);
                Console.WriteLine("Current V:");
                PrintV(info.CurrentV);
            }
        }
    }
}
