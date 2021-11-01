using FEM;
using MathUtilities;
using SlaeSolver;
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

        static void Main(string[] args)
        {
            var fem = LoadSolution();

            ProblemInfo info = new ProblemInfo
            {
                Receivers = new Receiver[5]
                { 
                    new Receiver(new Vector2(600, 0)), 
                    new Receiver(new Vector2(700, 0)), 
                    new Receiver(new Vector2(800, 0)), 
                    new Receiver(new Vector2(900, 0)),
                    new Receiver(new Vector2(1000, 0)),
                },
                Sources = new Source[4]
                {
                    new Source(new Vector2(0, 0), new Vector2(30, 0), 2.5, fem),
                    new Source(new Vector2(50, 0), new Vector2(80, 0), 0.0, fem),
                    new Source(new Vector2(110, 0), new Vector2(140, 0), 0.0, fem),
                    new Source(new Vector2(180, 0), new Vector2(210, 0), 0.0, fem),
                }

            };

            info.RealV = DirectProblem(info);
            for (int i = 0; i < info.Sources.Length; i++)
                info.Sources[i].I = 5.0;
            info.CurrentV = DirectProblem(info);

            double J = Functional(info.CurrentV, info.RealV);

            bool flag = true;
            while (flag && J > 1.0e-9)
            {
                (J, flag) = InverseProblemStep(info, J);
            }
        }
    }
}
