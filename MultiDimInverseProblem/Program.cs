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
                Receivers = new Receiver[3]
                { 
                    new Receiver(new Point(100, 100), new Point(110, 100)),
                    new Receiver(new Point(210, 100), new Point(220, 100)), 
                    new Receiver(new Point(150, 70), new Point(160, 70)), 
                },
                Sources = new Source[10]
                {
                    new Source(new Point(0, 0), 2.5, fem),
                    new Source(new Point(30, 0), 0.0, fem),
                    new Source(new Point(60, 0), 0.0, fem),
                    new Source(new Point(90, 0), 0.0, fem),
                    new Source(new Point(120, 0), 0.0, fem),
                    new Source(new Point(150, 0), 0.0, fem),
                    new Source(new Point(180, 0), 0.0, fem),
                    new Source(new Point(210, 0), 0.0, fem),
                    new Source(new Point(240, 0), 0.0, fem),
                    new Source(new Point(270, 0), 0.0, fem),
                }
            };

            info.PivotI = new double[10] { 5.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

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
