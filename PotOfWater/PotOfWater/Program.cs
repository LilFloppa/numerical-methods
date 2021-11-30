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
            ProblemInfo info)
        {
            string[] lines = File.ReadAllLines(pointsFile);
            int pointCount = int.Parse(lines[0]);
            Point[] points = new Point[pointCount];

            for (int i = 0; i < pointCount; i++)
                points[i] = Point.Parse(lines[i + 1]);

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

            lines = File.ReadAllLines(firstBoundaryFile);
            int firstBoundaryCount = int.Parse(lines[0]);
            List<Edge> firstBondary = new List<Edge>();

            for (int i = 0; i < firstBoundaryCount; i++)
            {
                Edge e = new Edge(info.BondaryBasis.Size);
                string[] tokens = lines[i + 1].Split(' ');
                e.Vertices[0] = int.Parse(tokens[0]) - 1;
                e.Vertices[info.BondaryBasis.Size - 1] = int.Parse(tokens[1]) - 1;
                e.F = info.FirstBoundaryDictionary[int.Parse(tokens[2])];

                firstBondary.Add(e);
            }

            IMeshBuilder builder = new LinearMeshBuilder();
            builder.AddPoints(points);
            builder.AddElements(elements);
            builder.AddFirstBoundary(firstBondary);
            return builder.Build();
        }

        static void GaussQuadratureTest()
        {
            double result = Quadratures.TriangleGauss18(new Point(1, 1), new Point(3, 1), new Point(2, 3), (double x, double y) => x * x * x * x * x * x * x * x);
            Console.WriteLine(result);
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
            GaussMatrixTest();
            ProblemInfo info = new ProblemInfo
            {
                Basis = new TriangleLinearLagrange(),
                BondaryBasis = new LineLinearLagrange(),
                MaterialDictionary = AreaInfo.Materials,
                FirstBoundaryDictionary = AreaInfo.FirstBoundary,
                SecondBoundaryDictionary = AreaInfo.SecondBoundary
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

            SLAEBuilder builder = new SLAEBuilder(info);
            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] q = solver.Solve(A, b);
        }
    }
}
