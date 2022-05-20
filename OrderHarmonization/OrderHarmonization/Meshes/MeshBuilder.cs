using MathUtilities;
using System;
using System.Collections.Generic;

namespace OrderHarmonization.Meshes
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
            var edgeMatrix = new int[nodeCount, nodeCount];

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

                        if (edgeMatrix[a, b] == 0)
                        {
                            e.Vertices[index] = nodeCount + (f ? 1 : 0);
                            e.Vertices[index + 1] = nodeCount + (f ? 0 : 1);
                            edgeMatrix[a, b] = nodeCount;
                            nodeCount += 2;
                        }
                        else
                        {
                            int a1 = edgeMatrix[a, b];
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
        private void BulidBondary(int[,] edgeMatrix)
        {
            foreach (Edge edge in firstBoundary)
            {
                int a = edge[0];
                int b = edge[3];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                edge.Vertices[1] = edgeMatrix[a, b] + (f ? 1 : 0);
                edge.Vertices[2] = edgeMatrix[a, b] + (f ? 0 : 1);
            }

            foreach (Edge edge in secondBoundary)
            {
                int a = edge[0];
                int b = edge[3];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                edge.Vertices[1] = edgeMatrix[a, b] + (f ? 1 : 0);
                edge.Vertices[2] = edgeMatrix[a, b] + (f ? 0 : 1);
            }

            foreach (Edge edge in thirdBoundary)
            {
                int a = edge[0];
                int b = edge[3];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                edge.Vertices[1] = edgeMatrix[a, b] + (f ? 1 : 0);
                edge.Vertices[2] = edgeMatrix[a, b] + (f ? 0 : 1);
            }
        }
    }

    public class HarmonicMeshBuilder : IMeshBuilder
    {
        private struct EdgeInfo
        {
            public int Index;
            public bool BelongsToFirstOrderTriangle;
        };

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
            var edgeMatrix = new EdgeInfo[nodeCount, nodeCount];

            foreach (FiniteElement e in elements)
            {
                if (e.Order == 1)
                {
                    for (int i = 0; i < 3; i++)
                        for (int j = i + 1; j < 3; j++)
                        {
                            int a = e.Vertices[i];
                            int b = e.Vertices[j];
                            bool f = a > b;
                            if (f) (a, b) = (b, a);

                            edgeMatrix[a, b].BelongsToFirstOrderTriangle = true;
                        }
                }
            }

            foreach (FiniteElement e in elements)
            {
                if (e.Order == 2)
                {
                    int index = 3;
                    for (int i = 0; i < 3; i++)
                        for (int j = i + 1; j < 3; j++, index++)
                        {
                            int a = e.Vertices[i];
                            int b = e.Vertices[j];
                            bool f = a > b;
                            if (f) (a, b) = (b, a);

                            if (edgeMatrix[a, b].BelongsToFirstOrderTriangle)
                            {
                                continue;
                            }

                            if (edgeMatrix[a, b].Index == 0)
                            {
                                e.Vertices[index] = nodeCount;
                                edgeMatrix[a, b].Index = nodeCount;
                                nodeCount++;
                            }
                            else
                            {
                                int a1 = edgeMatrix[a, b].Index;
                                e.Vertices[index] = a1;
                            }
                        }
                }
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
        private void BulidBondary(EdgeInfo[,] edgeMatrix)
        {
            foreach (Edge edge in firstBoundary)
            {
                int a = edge[0];
                int b = edge[2];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                if (edgeMatrix[a, b].BelongsToFirstOrderTriangle)
                    continue;

                edge.Vertices[1] = edgeMatrix[a, b].Index;
                edge.Order = 2;
            }

            foreach (Edge edge in secondBoundary)
            {
                int a = edge[0];
                int b = edge[2];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                if (edgeMatrix[a, b].BelongsToFirstOrderTriangle)
                    continue;

                edge.Vertices[1] = edgeMatrix[a, b].Index;
                edge.Order = 2;
            }

            foreach (Edge edge in thirdBoundary)
            {
                int a = edge[0];
                int b = edge[2];
                bool f = a > b;
                if (f) (a, b) = (b, a);

                if (edgeMatrix[a, b].BelongsToFirstOrderTriangle)
                    continue;

                edge.Vertices[1] = edgeMatrix[a, b].Index;
                edge.Order = 2;
            }
        }
    }
}
