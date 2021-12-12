using MathUtilities;
using PotOfWater.Meshes;
using System;
using System.Collections.Generic;
using System.IO;

namespace PotOfWater
{
    class Program
    {
        static Mesh LoadMesh(
            string pointsFile,
            string elementsFile,
            string firstBoundaryFile,
            string secondBoundaryFile,
            string thirdBoundaryFile,
            ProblemInfo info,
            IMeshBuilder builder)
        {
            // Points Parsing --------------------------------------------------------
            string[] lines = File.ReadAllLines(pointsFile);
            int pointCount = int.Parse(lines[0]);
            Point[] points = new Point[pointCount];

            for (int i = 0; i < pointCount; i++)
                points[i] = Point.Parse(lines[i + 1]);
            // -----------------------------------------------------------------------

            // Elements Parsing ------------------------------------------------------
            lines = File.ReadAllLines(elementsFile);
            int elementsCount = int.Parse(lines[0]);
            List<FiniteElement> elements = new List<FiniteElement>();

            for (int i = 0; i < elementsCount; i++)
            {
                FiniteElement e = new FiniteElement(info.Basis.Size);
                string[] tokens = lines[i + 1].Split(' ');
                e.Vertices[0] = int.Parse(tokens[0]) - 1;
                e.Vertices[1] = int.Parse(tokens[1]) - 1;
                e.Vertices[2] = int.Parse(tokens[2]) - 1;
                e.Material = info.MaterialDictionary[int.Parse(tokens[3])];

                elements.Add(e);
            }
            // -----------------------------------------------------------------------

            // First Boundary Parsing ------------------------------------------------
            lines = File.ReadAllLines(firstBoundaryFile);
            int firstBoundaryCount = int.Parse(lines[0]);
            List<FirstBoundaryEdge> firstBondary = new List<FirstBoundaryEdge>();

            for (int i = 0; i < firstBoundaryCount; i++)
            {
                FirstBoundaryEdge e = new FirstBoundaryEdge(info.BoundaryBasis.Size);
                string[] tokens = lines[i + 1].Split(' ');
                e.Vertices[0] = int.Parse(tokens[0]) - 1;
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]) - 1;
                e.F = info.FirstBoundaryDictionary[int.Parse(tokens[2])];

                firstBondary.Add(e);
            }
            // -----------------------------------------------------------------------

            // Second Boundary Parsing -----------------------------------------------
            lines = File.ReadAllLines(secondBoundaryFile);
            int secondBoundaryCount = int.Parse(lines[0]);
            List<SecondBoundaryEdge> secondBondary = new List<SecondBoundaryEdge>();

            for (int i = 0; i < secondBoundaryCount; i++)
            {
                SecondBoundaryEdge e = new SecondBoundaryEdge(info.BoundaryBasis.Size);
                string[] tokens = lines[i + 1].Split(' ');
                e.Vertices[0] = int.Parse(tokens[0]) - 1;
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]) - 1;
                e.Theta = info.SecondBoundaryDictionary[int.Parse(tokens[2])];

                secondBondary.Add(e);
            }
            // -----------------------------------------------------------------------

            // Third Boundary Parsing ------------------------------------------------
            lines = File.ReadAllLines(thirdBoundaryFile);
            int thirdBoundaryCount = int.Parse(lines[0]);
            List<ThirdBoundaryEdge> thirdBondary = new List<ThirdBoundaryEdge>();

            for (int i = 0; i < thirdBoundaryCount; i++)
            {
                ThirdBoundaryEdge e = new ThirdBoundaryEdge(info.BoundaryBasis.Size);
                string[] tokens = lines[i + 1].Split(' ');
                e.Vertices[0] = int.Parse(tokens[0]) - 1;
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]) - 1;

                int boundaryNo = int.Parse(tokens[2]);
                e.UBeta = info.ThirdBoundaryDictionary[boundaryNo].ubeta;
                e.Beta = info.ThirdBoundaryDictionary[boundaryNo].beta;

                thirdBondary.Add(e);
            }
            // -----------------------------------------------------------------------

            builder.AddPoints(points);
            builder.AddElements(elements);
            builder.AddFirstBoundary(firstBondary);
            builder.AddSecondBoundary(secondBondary);
            builder.AddThirdBoundary(thirdBondary);
            return builder.Build();
        }


        static void ClearMatrix(IMatrix A, double[] b)
        {
            Array.Fill(b, 0.0);
            Array.Fill(A.AL, 0.0);
            Array.Fill(A.AU, 0.0);
            Array.Fill(A.DI, 0.0);
        }

        static void TimeProblem(ProblemInfo info, IMatrix A, double[] b)
        {
            ISolver solver = new LOSLU();
            TimeSlaeBuilder tsb = new TimeSlaeBuilder(info);

            double[] t = info.TimeMesh;
            double[][] Q = new double[info.TimeMesh.Length][];
            Q[0] = new double[A.N];

            tsb.Layer = new TwoLayer();
            tsb.Layer.SetQ(new double[1][] { Q[0] });
            tsb.Layer.SetT(new double[] { t[0], t[1] });
            tsb.CurrentT = info.TimeMesh[1];
            tsb.Build(A, b);
            Q[1] = solver.Solve(A, b);
            ClearMatrix(A, b);

            Q[1][A.N - 1] = 0.225;

            tsb.Layer = new ThreeLayer();
            tsb.Layer.SetQ(new double[2][] { Q[0], Q[1] });
            tsb.Layer.SetT(new double[3] { t[0], t[1], t[2] });
            tsb.CurrentT = t[2];
            tsb.Build(A, b);
            Q[2] = solver.Solve(A, b);
            ClearMatrix(A, b);

            tsb.Layer = new FourLayer();
            tsb.Layer.SetQ(new double[3][] { Q[0], Q[1], Q[2] });
            tsb.Layer.SetT(new double[4] { t[0], t[1], t[2], t[3] });
            tsb.CurrentT = t[3];
            tsb.Build(A, b);
            Q[3] = solver.Solve(A, b);
            ClearMatrix(A, b);

            for (int i = 4; i < info.TimeMesh.Length; i++)
            {
                tsb.Layer.SetQ(new double[3][] { Q[i - 3], Q[i - 2], Q[i - 1] });
                tsb.Layer.SetT(new double[4] { t[i - 3], t[i - 2], t[i - 1], t[i] });
                tsb.CurrentT = t[i];
                tsb.Build(A, b);
                Q[i] = solver.Solve(A, b);
                ClearMatrix(A, b);
            }    
        }

        static void Problem(ProblemInfo info, IMatrix A, double[] b)
        {
            LSlaeBuilder builder = new LSlaeBuilder(info);
            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] q = solver.Solve(A, b);
        }
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo ?? throw new InvalidCastException();
            culture.NumberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            ProblemInfo info = new ProblemInfo
            {
                Basis = new TriangleCubicLagrange(),
                BoundaryBasis = new LineCubicLagrange(),
                MaterialDictionary = AreaInfo.Materials,
                FirstBoundaryDictionary = AreaInfo.FirstBoundary,
                SecondBoundaryDictionary = AreaInfo.SecondBoundary,
                ThirdBoundaryDictionary = AreaInfo.ThirdBoundary
            };

            Mesh mesh = LoadMesh(
              @"C:\repos\numerical-methods\PotOfWater\PotOfWater\Input\points.txt",
              @"C:\repos\numerical-methods\PotOfWater\PotOfWater\Input\triangles.txt",
              @"C:\repos\numerical-methods\PotOfWater\PotOfWater\Input\boundary1.txt",
              @"C:\repos\numerical-methods\PotOfWater\PotOfWater\Input\boundary2.txt",
              @"C:\repos\numerical-methods\PotOfWater\PotOfWater\Input\boundary3.txt",
              info,
              new CubicMeshBuilder());

            info.Mesh = mesh;
            info.TimeMesh = new double[4] { 0.0, 0.1, 0.3, 0.7 };

            PortraitBuilder PB = new PortraitBuilder(info);
            Portrait p = PB.Build();

            IMatrix A = new SparseMatrix(mesh.NodeCount);
            double[] b = new double[mesh.NodeCount];
            A.SetPortrait(p);

            // TimeProblem(info, A, b);

            CubicSlaeBuilder sb = new CubicSlaeBuilder(info);
            sb.Build(A, b);

            ISolver solver = new LOSLU();
            var q = solver.Solve(A, b);

            Solution solution = new Solution(q, mesh);
            double res = solution.GetValue(0.5, 0.5);
        }
    }
}
