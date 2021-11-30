using MathUtilities;
using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public class SLAEBuilder
    {
        private ProblemInfo info;

        private Point[] points;

        private double[,] local;
        private double[] localb;

        private Func<double, double, double>[] psi;
        private Dictionary<string, Func<double, double, double>[]> psiDers;

        private Func<double, double>[] boundaryPsi;

        public SLAEBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;

            local = new double[info.Basis.Size, info.Basis.Size];
            localb = new double[info.Basis.Size];

            psi = info.Basis.GetFuncs();
            psiDers = info.Basis.GetDers();

            boundaryPsi = info.BondaryBasis.GetFuncs();
        }

        public void Build(IMatrix A, double[] b)
        {
            foreach (FiniteElement e in info.Mesh.Elements)
            {
                ClearLocals();
                BuildLocalMatrix(e);
                BuildLocalB(e);
                AddLocalToGlobal(A, b, e);
            }

            foreach (Edge e in info.Mesh.FirstBoundary)
                AddFirstBoundary(A, b, e);
        }

        private void ClearLocals()
        {
            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                    local[i, j] = 0.0;

            Array.Fill(localb, 0.0);
        }

        private void BuildLocalMatrix(FiniteElement e)
        {
            // TODO: implement this method
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                {
                    double G = Quadratures.TriangleGauss18(a, b, c, GetGradProduct(i, j));
                    double M = Quadratures.TriangleGauss18(a, b, c, (double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
                    local[i, j] = (e.Material.RoCp * M + e.Material.Lambda * G) * D;
                }
        }

        private void BuildLocalB(FiniteElement e)
        {
            // TODO: implement this method

            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            for (int i = 0; i < info.Basis.Size; i++)
            {
                localb[i] = 0.0;
                localb[i] *= D;
            }
        }

        private void AddLocalToGlobal(IMatrix A, double[] b, FiniteElement e)
        {
            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                    A.Add(e[i], e[j], local[i, j]);

            for (int i = 0; i < info.Basis.Size; i++)
                b[e[i]] += localb[i];
        }

        private void AddFirstBoundary(IMatrix A, double[] b, Edge edge)
        {
            double[,] edgeM = new double[2, 2]
            {
                { 1.0 / 3.0,  1.0 / 6.0 },
                { 1.0 / 6.0,  1.0 / 3.0 },
            };

            Func<double, double> Ug = edge.F;

            double x0 = info.Mesh.Points[edge[0]].X;
            double y0 = info.Mesh.Points[edge[0]].Y;
            double x1 = info.Mesh.Points[edge[edge.NodeCount - 1]].X;
            double y1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Y;

            double[] f = new double[edge.NodeCount];
            for (int i = 0; i < edge.NodeCount; i++)
                f[i] = Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => Ug(ksi) * boundaryPsi[i](ksi));

            double[] q = new double[edge.NodeCount];
            Gauss.Solve(edgeM, q, f);

            for (int i = 0; i < edge.NodeCount; i++)
            {
                A.DI[edge[i]] = 1.0e+50;
                b[edge[i]] = 1.0e+50 * q[i];
            }
        }

        private Func<double, double, double> GetGradProduct(int i, int j)
        {
            Func<double, double, double> ksiDer = (double ksi, double etta) => psiDers["ksi"][i](ksi, etta) * psiDers["ksi"][j](ksi, etta);
            Func<double, double, double> ettaDer = (double ksi, double etta) => psiDers["etta"][i](ksi, etta) * psiDers["etta"][j](ksi, etta);

            return (double ksi, double etta) => ksiDer(ksi, etta) + ettaDer(ksi, etta);
        }
    }
}
