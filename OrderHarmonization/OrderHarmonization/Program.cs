using MathUtilities;
using OrderHarmonization.Meshes;
using System;
using System.Collections.Generic;
using System.IO;

namespace OrderHarmonization
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
                e.Vertices[0] = int.Parse(tokens[0]);
                e.Vertices[1] = int.Parse(tokens[1]);
                e.Vertices[2] = int.Parse(tokens[2]);
                e.Material = info.MaterialDictionary[int.Parse(tokens[3])];
                e.Order = int.Parse(tokens[4]);

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
                e.Vertices[0] = int.Parse(tokens[0]);
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]);
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
                e.Vertices[0] = int.Parse(tokens[0]);
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]);
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
                e.Vertices[0] = int.Parse(tokens[0]);
                e.Vertices[info.BoundaryBasis.Size - 1] = int.Parse(tokens[1]);

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

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo culture = System.Threading.Thread.CurrentThread.CurrentCulture.Clone() as System.Globalization.CultureInfo ?? throw new InvalidCastException();
            culture.NumberFormat = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            ProblemInfo info = new ProblemInfo
            {
                Basis = new TriangleCubicHierarchical(),
                BoundaryBasis = new LineCubicHierarchical(),
                BoundaryBasisFirstOrder = new LineLinearHierarchical(),
                BoundaryBasisThirdOrder = new LineCubicHierarchical(),
                MaterialDictionary = AreaInfo.Materials,
                FirstBoundaryDictionary = AreaInfo.FirstBoundary,
                SecondBoundaryDictionary = AreaInfo.SecondBoundary,
                ThirdBoundaryDictionary = AreaInfo.ThirdBoundary
            };

            //ProblemInfo info = new ProblemInfo
            //{
            //    Basis = new TriangleLinearHierarchical(),
            //    BoundaryBasis = new LineLinearHierarchical(),
            //    BoundaryBasisFirstOrder = new LineLinearHierarchical(),
            //    BoundaryBasisThirdOrder = new LineCubicHierarchical(),
            //    MaterialDictionary = AreaInfo.Materials,
            //    FirstBoundaryDictionary = AreaInfo.FirstBoundary,
            //    SecondBoundaryDictionary = AreaInfo.SecondBoundary,
            //    ThirdBoundaryDictionary = AreaInfo.ThirdBoundary
            //};

            var meshBuilder = new HarmonicMeshBuilder();

            Mesh mesh = LoadMesh(
              @"C:\repos\numerical-methods\OrderHarmonization\OrderHarmonization\Input\points.txt",
              @"C:\repos\numerical-methods\OrderHarmonization\OrderHarmonization\Input\triangles.txt",
              @"C:\repos\numerical-methods\OrderHarmonization\OrderHarmonization\Input\boundary1.txt",
              @"C:\repos\numerical-methods\OrderHarmonization\OrderHarmonization\Input\boundary2.txt",
              @"C:\repos\numerical-methods\OrderHarmonization\OrderHarmonization\Input\boundary3.txt",
              info,
              meshBuilder);

            info.Mesh = mesh;

            PortraitBuilder PB = new PortraitBuilder(info);
            Portrait p = PB.Build();

            IMatrix A = new SparseMatrix(mesh.NodeCount);
            double[] b = new double[mesh.NodeCount];
            A.SetPortrait(p);

            HarmonicSlaeBuilder builder = new HarmonicSlaeBuilder(info);
            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] q = solver.Solve(A, b);

            Solution s = new Solution(q, mesh);

            double value = s.GetValue(new Point(1.5, 0.3));
            double value1 = s.GetValue(new Point(0.5, 1.2));
            Console.WriteLine(value);
            Console.WriteLine(value1);
        }
    }
}
