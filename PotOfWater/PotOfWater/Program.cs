using MathUtilities;
using PotOfWater.Meshes;
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
            ProblemInfo info)
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

            IMeshBuilder builder = new LinearMeshBuilder();
            builder.AddPoints(points);
            builder.AddElements(elements);
            builder.AddFirstBoundary(firstBondary);
            builder.AddSecondBoundary(secondBondary);
            builder.AddThirdBoundary(thirdBondary);
            return builder.Build();
        }

        static void GaussMatrixTest()
        {
            double[,] A = new double[3, 3]
            {
                { 1, 0, 2 },
                { -2, 5, 8 },
                { 5, 5, 0 }
            };

            double[] b = new double[3] { 3, 11, 10 };
            double[] q = new double[3] { 0, 0, 0 };
            Gauss.Solve(A, q, b);
        }

        static void Main(string[] args)
        {
            ProblemInfo info = new ProblemInfo
            {
                Basis = new TriangleLinearLagrange(),
                BoundaryBasis = new LineLinearLagrange(),
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
              info);

            info.Mesh = mesh;

            PortraitBuilder PB = new PortraitBuilder(info);
            Portrait p = PB.Build();

            IMatrix A = new SparseMatrix(mesh.NodeCount);
            double[] b = new double[mesh.NodeCount];
            A.SetPortrait(p);

            LSlaeBuilder builder = new LSlaeBuilder(info);
            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] q = solver.Solve(A, b);
        }
    }
}
