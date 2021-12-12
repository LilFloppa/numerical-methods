using MathUtilities;
using System;
using System.Collections.Generic;

namespace PotOfWater.Meshes
{
    public interface IMeshBuilder
    {
        public Mesh Build();
        public void AddPoints(Point[] points);
        public void AddElements(List<FiniteElement> elements);
        public void AddFirstBoundary(List<FirstBoundaryEdge> edges);
        public void AddSecondBoundary(List<SecondBoundaryEdge> edges);
        public void AddThirdBoundary(List<ThirdBoundaryEdge> edges);
    }

    public class LinearMeshBuilder : IMeshBuilder
    {
        private Point[] points;
        private List<FiniteElement> elements;
        private List<FirstBoundaryEdge> firstBoundary;
        private List<SecondBoundaryEdge> secondBoundary;
        private List<ThirdBoundaryEdge> thirdBoundary;
        public void AddPoints(Point[] points) => this.points = points;
        public void AddElements(List<FiniteElement> elements) => this.elements = elements;
        public void AddFirstBoundary(List<FirstBoundaryEdge> firstBoundary) => this.firstBoundary = firstBoundary;
        public void AddSecondBoundary(List<SecondBoundaryEdge> secondBoundary) => this.secondBoundary = secondBoundary;
        public void AddThirdBoundary(List<ThirdBoundaryEdge> thirdBoundary) => this.thirdBoundary = thirdBoundary;
        public Mesh Build()
        {
            return new Mesh
            {
                NodeCount = points.Length,
                Elements = elements,
                Points = points,
                FirstBoundary = firstBoundary,
                SecondBoundary = secondBoundary,
                ThirdBoundary = thirdBoundary
            };
        }
    }

    public class CubicMeshBuilder : IMeshBuilder
    {
        private Point[] points;
        private List<FiniteElement> elements;
        private List<FirstBoundaryEdge> firstBoundary;
        private List<SecondBoundaryEdge> secondBoundary;
        private List<ThirdBoundaryEdge> thirdBoundary;

        public void AddPoints(Point[] points) => this.points = points;
        public void AddElements(List<FiniteElement> elements) => this.elements = elements;
        public void AddFirstBoundary(List<FirstBoundaryEdge> firstBoundary) => this.firstBoundary = firstBoundary;
        public void AddSecondBoundary(List<SecondBoundaryEdge> secondBoundary) => this.secondBoundary = secondBoundary;
        public void AddThirdBoundary(List<ThirdBoundaryEdge> thirdBoundary) => this.thirdBoundary = thirdBoundary;
        public Mesh Build()
        {
            int nodeCount = points.Length;
            var edgeMatrix = new Dictionary<int, int>[nodeCount];
            for (int i = 0; i < nodeCount; i++)
                edgeMatrix[i] = new Dictionary<int, int>();

            foreach (FiniteElement e in elements)
            {
                int index = 3;
                for (int i = 0; i < 3; i++)
                    for (int j = i + 1; j < 3; j++, index += 2)
                    {
                        int a = e.Vertices[i];
                        int b = e.Vertices[j];
                        bool f = a > b;
                        if (f) (a, b) = (b, a);

                        if (!edgeMatrix[a].ContainsKey(b))
                        {
                            e.Vertices[index] = nodeCount + (f ? 1 : 0);
                            e.Vertices[index + 1] = nodeCount + (f ? 0 : 1);
                            edgeMatrix[a][b] = nodeCount;
                            nodeCount += 2;
                        }
                        else
                        {
                            int a1 = edgeMatrix[a][b];
                            e.Vertices[index] = a1 + (f ? 1 : 0);
                            e.Vertices[index + 1] = a1 + (f ? 0 : 1);
                        }
                    }
                e.Vertices[index] = nodeCount;
                nodeCount++;
            }

            BulidBondary(edgeMatrix);

            return new Mesh
            {
                NodeCount = nodeCount,
                Elements = elements,
                Points = points,
                FirstBoundary = firstBoundary,
                SecondBoundary = secondBoundary,
                ThirdBoundary = thirdBoundary
            };
        }

        private void BulidBondary(Dictionary<int, int>[] edgeMatrix)
        {
            
        }
    }
}
