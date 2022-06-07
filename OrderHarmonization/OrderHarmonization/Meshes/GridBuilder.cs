using MathUtilities;
using System;
using System.Collections.Generic;

namespace OrderHarmonization.Meshes
{
    public struct Triangle
    {
        public int V1, V2, V3;
        public int MaterialNo;
        public int Order;
    }

    public struct GridEdge
    {
        public int V1, V2;
        public int No;
    }

    public enum BoundaryType
    {
        First,
        Second,
        Third,
    }

    public struct GridBoundary
    {
        public BoundaryType Type;
        public int FuncNo;
    }


    public struct Interval
    {
        public double Begin;
        public double End;
        public int NodeCount;
        public int Order;
        public int Mat;
    }

    public struct GridInfo
    {
        public double XBegin, XEnd;
        public double YBegin, YEnd;
        public List<Interval> XIntervals;
        public List<Interval> YIntervals;
        public GridBoundary LeftBoundary { get; set; }
        public GridBoundary TopBoundary { get; set; }
        public GridBoundary RightBoundary { get; set; }
        public GridBoundary BottomBoundary { get; set; }
    }

    public class GridBuilder
    {
        public GridInfo Info { get; set; }
        public Point[] Points { get; set; }
        public List<Triangle> Elements { get; set; } = new();
        public List<GridEdge> FirstBoundary { get; set; } = new();
        public List<GridEdge> SecondBoundary { get; set; } = new();
        public List<GridEdge> ThirdBoundary { get; set; } = new();

        private Dictionary<BoundaryType, Action<GridEdge>> BoundaryTypeMap = new();
        private int xNodeCount = 0;
        private int yNodeCount = 0;

        public GridBuilder(GridInfo info)
        {
            Info = info;

            xNodeCount = NodeCount(info.XIntervals);
            yNodeCount = NodeCount(info.YIntervals);
            int pointCount = xNodeCount * yNodeCount;
            Points = new Point[pointCount];
            Elements = new();

            BoundaryTypeMap.Add(BoundaryType.First, (e) => FirstBoundary.Add(e));
            BoundaryTypeMap.Add(BoundaryType.Second, (e) => SecondBoundary.Add(e));
            BoundaryTypeMap.Add(BoundaryType.Third, (e) => ThirdBoundary.Add(e));
        }

        int NodeCount(List<Interval> intervals)
        {
            int nodeCount = 0;
            foreach (var i in intervals)
                nodeCount += i.NodeCount;

            return nodeCount - intervals.Count + 1;
        }
        public void Build()
        {
            BuildPoints();
            BuildTriangles();
            BuildBoundary();
        }

        void BuildPoints()
        {
            int index = 0;
            foreach (var yint in Info.YIntervals)
            {
                double height = yint.End - yint.Begin;
                double yStep = height / (yint.NodeCount - 1);

                for (int i = 0; i < yint.NodeCount - 1; i++)
                {
                    foreach (var xint in Info.XIntervals)
                    {
                        double width = xint.End - xint.Begin;
                        double xStep = width / (xint.NodeCount - 1);

                        for (int j = 0; j < xint.NodeCount - 1; j++)
                            Points[index++] = new Point(xint.Begin + j * xStep, yint.Begin + i * yStep);

                    }

                    Points[index++] = new Point(Info.XEnd, yint.Begin + i * yStep);
                }
            }

            foreach (var xint in Info.XIntervals)
            {
                double width = xint.End - xint.Begin;
                double xStep = width / (xint.NodeCount - 1);

                for (int j = 0; j < xint.NodeCount - 1; j++)
                    Points[index++] = new Point(xint.Begin + j * xStep, Info.YEnd);

            }

            Points[index++] = new Point(Info.XEnd, Info.YEnd);
        }

        void BuildTriangles()
        {
            int xIndex = 0;
            int yIndex = 0;
            foreach (var yint in Info.YIntervals)
            {
                for (int i = 0; i < yint.NodeCount - 1; i++, yIndex++)
                {
                    foreach (var xint in Info.XIntervals)
                    {
                        for (int j = 0; j < xint.NodeCount - 1; j++, xIndex++)
                        {
                            var e1 = new Triangle();
                            var e2 = new Triangle();

                            int p1 = yIndex * xNodeCount + xIndex;
                            int p2 = yIndex * xNodeCount + xIndex + 1;
                            int p3 = (yIndex + 1) * xNodeCount + xIndex;
                            int p4 = (yIndex + 1) * xNodeCount + xIndex + 1;

                            var x1 = Points[p1].X;
                            var x2 = Points[p2].X;
                            var y1 = Points[p1].Y;
                            var y2 = Points[p3].Y;

                            int matNo = Math.Min(xint.Mat, yint.Mat);
                            int order = Math.Min(xint.Order, yint.Order);

                            e1.V1 = p1;
                            e1.V2 = p2;
                            e1.V3 = p4;
                            e1.MaterialNo = matNo;
                            e1.Order = order;

                            e2.V1 = p1;
                            e2.V2 = p3;
                            e2.V3 = p4;
                            e2.MaterialNo = matNo;
                            e2.Order = order;

                            Elements.Add(e1);
                            Elements.Add(e2);
                        }
                    }

                    xIndex = 0;
                }
            }
        }

        void BuildBoundary()
        {
            // Left
            for (int i = 0; i < yNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = i * xNodeCount;
                edge.V2 = (i + 1) * xNodeCount;
                edge.No = Info.LeftBoundary.FuncNo;
                BoundaryTypeMap[Info.LeftBoundary.Type](edge);
            }
            // Top
            for (int i = 0; i < xNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = xNodeCount * (yNodeCount - 1) + i;
                edge.V2 = xNodeCount * (yNodeCount - 1) + i + 1;
                edge.No = Info.TopBoundary.FuncNo;
                BoundaryTypeMap[Info.TopBoundary.Type](edge);
            }
            // Right
            for (int i = 0; i < yNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = (i + 1) * xNodeCount - 1;
                edge.V2 = (i + 2) * xNodeCount - 1;
                edge.No = Info.RightBoundary.FuncNo;
                BoundaryTypeMap[Info.RightBoundary.Type](edge);
            }
            // Bottom
            for (int i = 0; i < xNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = i;
                edge.V2 = i + 1;
                edge.No = Info.BottomBoundary.FuncNo;
                BoundaryTypeMap[Info.BottomBoundary.Type](edge);
            }
        }
    }

}
