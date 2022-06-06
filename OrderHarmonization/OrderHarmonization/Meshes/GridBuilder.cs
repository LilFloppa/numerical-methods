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

    public struct OrderSubDomain
    {
        public double X1, X2, Y1, Y2;
        public int Order;

        public OrderSubDomain(double x1, double x2, double y1, double y2, int order)
        {
            X1 = x1; Y1 = y1;
            X2 = x2; Y2 = y2;
            Order = order;
        }
        public bool Contains(double x1, double x2, double y1, double y2) => x1 >= X1 && x2 <= X2 && y1 >= Y1 && y2 <= Y2;
    }


    public struct GridInfo
    {
        public int XNodeCount { get; set; }
        public int YNodeCount { get; set; }
        public double XBegin { get; set; }
        public double YBegin { get; set; }
        public double XEnd { get; set; }
        public double YEnd { get; set; }

        public GridBoundary LeftBoundary { get; set; }
        public GridBoundary TopBoundary { get; set; }
        public GridBoundary RightBoundary { get; set; }
        public GridBoundary BottomBoundary { get; set; }

        public List<OrderSubDomain> OrderSubDomains { get; set; }
        public List<OrderSubDomain> MaterialSubDomains { get; set; }
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

        public GridBuilder(GridInfo info)
        {
            Info = info;

            int pointCount = Info.XNodeCount * Info.YNodeCount;
            Points = new Point[pointCount];
            Elements = new();

            BoundaryTypeMap.Add(BoundaryType.First, (e) => FirstBoundary.Add(e));
            BoundaryTypeMap.Add(BoundaryType.Second, (e) => SecondBoundary.Add(e));
            BoundaryTypeMap.Add(BoundaryType.Third, (e) => ThirdBoundary.Add(e));
        }

        public void Build()
        {
            BuildPoints();
            BuildTriangles();
            BuildBoundary();
        }

        void BuildPoints()
        {
            double width = Info.XEnd - Info.XBegin;
            double height = Info.YEnd - Info.YBegin;
            double xStep = width / (Info.XNodeCount - 1);
            double yStep = height / (Info.YNodeCount - 1);

            for (int i = 0; i < Info.YNodeCount; i++)
                for (int j = 0; j < Info.XNodeCount; j++)
                    Points[i * Info.XNodeCount + j] = new Point(Info.XBegin + j * xStep, Info.YBegin + i * yStep);
        }

        void BuildTriangles()
        {
            for (int i = 0; i < Info.YNodeCount - 1; i++)
            {
                for (int j = 0; j < Info.XNodeCount - 1; j++)
                {
                    var e1 = new Triangle();
                    var e2 = new Triangle();

                    int p1 = i * Info.XNodeCount + j;
                    int p2 = i * Info.XNodeCount + j + 1;
                    int p3 = (i + 1) * Info.XNodeCount + j;
                    int p4 = (i + 1) * Info.XNodeCount + j + 1;

                    var x1 = Points[p1].X;
                    var x2 = Points[p2].X;
                    var y1 = Points[p1].Y;
                    var y2 = Points[p3].Y;

                    int matNo = GetMaterial(x1, x2, y1, y2);
                    int order = GetOrder(x1, x2, y1, y2);

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
        }

        void BuildBoundary()
        {
            // Left
            for (int i = 0; i < Info.YNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = i * Info.XNodeCount;
                edge.V2 = (i + 1) * Info.XNodeCount;
                edge.No = Info.LeftBoundary.FuncNo;
                BoundaryTypeMap[Info.LeftBoundary.Type](edge);
            }
            // Top
            for (int i = 0; i < Info.XNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = Info.XNodeCount * (Info.YNodeCount - 1) + i;
                edge.V2 = Info.XNodeCount * (Info.YNodeCount - 1) + i + 1;
                edge.No = Info.TopBoundary.FuncNo;
                BoundaryTypeMap[Info.TopBoundary.Type](edge);
            }
            // Right
            for (int i = 0; i < Info.YNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = (i + 1) * Info.XNodeCount - 1;
                edge.V2 = (i + 2) * Info.XNodeCount - 1;
                edge.No = Info.RightBoundary.FuncNo;
                BoundaryTypeMap[Info.RightBoundary.Type](edge);
            }
            // Bottom
            for (int i = 0; i < Info.XNodeCount - 1; i++)
            {
                var edge = new GridEdge();
                edge.V1 = i;
                edge.V2 = i + 1;
                edge.No = Info.BottomBoundary.FuncNo;
                BoundaryTypeMap[Info.BottomBoundary.Type](edge);
            }
        }

        int GetMaterial(double x1, double x2, double y1, double y2)
        {
            if (Info.MaterialSubDomains != null)
            {
                foreach (var sub in Info.MaterialSubDomains)
                    if (sub.Contains(x1, x2, y1, y2))
                        return sub.Order;
            }
            return 0;
        }

        int GetOrder(double x1, double x2, double y1, double y2)
        {
            if (Info.OrderSubDomains != null)
            {
                foreach (var sub in Info.OrderSubDomains)
                    if (sub.Contains(x1, x2, y1, y2))
                        return sub.Order;
            }

            return 1;
        }
    }

}
