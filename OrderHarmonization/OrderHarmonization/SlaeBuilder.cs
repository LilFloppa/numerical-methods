using MathUtilities;
using System;
using System.Collections.Generic;

namespace OrderHarmonization
{
    public class SlaeBuilder
    {
        protected ProblemInfo info;

        protected Point[] points;

        protected double[,] local;
        protected double[] localb;

        protected Func<double, double, double>[] psi;
        protected Dictionary<string, Func<double, double, double>[]> psiDers;

        protected Func<double, double>[] boundaryPsi;

        public SlaeBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;

            local = new double[info.Basis.Size, info.Basis.Size];
            localb = new double[info.Basis.Size];

            psi = info.Basis.GetFuncs();
            psiDers = info.Basis.GetDers();

            boundaryPsi = info.BoundaryBasis.GetFuncs();
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

            foreach (var e in info.Mesh.SecondBoundary)
                AddSecondBoundary(A, b, e);

            foreach (var e in info.Mesh.ThirdBoundary)
                AddThirdBoundary(A, b, e);

            foreach (var e in info.Mesh.FirstBoundary)
                AddFirstBoundary(A, b, e);
        }

        private void ClearLocals()
        {
            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                    local[i, j] = 0.0;

            Array.Fill(localb, 0.0);
        }

        protected virtual void BuildLocalMatrix(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            for (int i = 0; i < info.Basis.Size; i++)
            {
                for (int j = 0; j < info.Basis.Size; j++)
                {
                    double G = Quadratures.TriangleGauss18(GetGrad(i, j, a, b, c));
                    double M = Quadratures.TriangleGauss18((double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
                    local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M) * D;

                    Console.WriteLine($"G: {G:F7}\t\tM: {M:F7}");
                }
            }
        }

        protected virtual void BuildLocalB(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            for (int i = 0; i < info.Basis.Size; i++)
            {
                localb[i] = Quadratures.TriangleGauss18((double ksi, double etta) =>
                {
                    double x = a.X * ksi + b.X * etta + c.X * (1 - ksi - etta);
                    double y = a.Y * ksi + b.Y * etta + c.Y * (1 - ksi - etta);

                    return e.Material.F(x, y) * psi[i](ksi, etta);
                });
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

        protected virtual void AddFirstBoundary(IMatrix A, double[] b, FirstBoundaryEdge edge)
        {
            double[,] M = info.BoundaryBasis.MassMatrix;

            Func<double, double, double> ug = edge.F;

            double x0 = info.Mesh.Points[edge[0]].X;
            double y0 = info.Mesh.Points[edge[0]].Y;
            double x1 = info.Mesh.Points[edge[edge.NodeCount - 1]].X;
            double y1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Y;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)), info.BoundaryBasis);

            for (int i = 0; i < edge.NodeCount; i++)
            {
                A.DI[edge[i]] = 1.0e+50;
                b[edge[i]] = 1.0e+50 * q[i];
            }
        }

        protected virtual void AddSecondBoundary(IMatrix A, double[] b, SecondBoundaryEdge edge)
        {
            Func<double, double, double> theta = edge.Theta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => theta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)) * boundaryPsi[i](ksi));
        }

        protected virtual void AddThirdBoundary(IMatrix A, double[] b, ThirdBoundaryEdge edge)
        {
            double[,] M = info.BoundaryBasis.MassMatrix;

            Func<double, double, double> ubeta = edge.UBeta;
            double beta = edge.Beta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => ubeta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)) * boundaryPsi[i](ksi));

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                for (int j = 0; j < info.BoundaryBasis.Size; j++)
                    A.Add(edge[i], edge[j], beta * h * M[i, j]);
        }

        private Func<double, double, double> GetGrad(int i, int j, Point a, Point b, Point c)
        {
            var J = new Matrix2x2(
                a.X - c.X, a.Y - c.Y,
                b.X - c.X, b.Y - c.Y);

            var JT = J.Transpose();

            var JJT = J * JT;
            var JJTinv = JJT.Inverse();

            return (double ksi, double etta) =>
            {
                double[] gradi = new double[2] { psiDers["ksi"][i](ksi, etta), psiDers["etta"][i](ksi, etta) };
                double[] gradj = new double[2] { psiDers["ksi"][j](ksi, etta), psiDers["etta"][j](ksi, etta) };

                double[] v = gradi * JJTinv;
                return v[0] * gradj[0] + v[1] * gradj[1];
            };
        }
    }

    public class HarmonicSlaeBuilder
    {
        protected ProblemInfo info;

        protected Point[] points;

        protected double[,] local;
        protected double[] localb;

        protected Func<double, double, double>[] psi;
        protected Dictionary<string, Func<double, double, double>[]> psiDers;

        public HarmonicSlaeBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;

            local = new double[info.Basis.Size, info.Basis.Size];
            localb = new double[info.Basis.Size];

            psi = info.Basis.GetFuncs();
            psiDers = info.Basis.GetDers();
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

            foreach (var e in info.Mesh.SecondBoundary)
                AddSecondBoundary(A, b, e);

            foreach (var e in info.Mesh.ThirdBoundary)
                AddThirdBoundary(A, b, e);

            foreach (var e in info.Mesh.FirstBoundary)
                AddFirstBoundary(A, b, e);
        }

        private void ClearLocals()
        {
            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                    local[i, j] = 0.0;

            Array.Fill(localb, 0.0);
        }

        protected virtual void BuildLocalMatrix(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            // TODO: Can be optimized by checking if vertice == -1
            for (int i = 0; i < info.Basis.Size; i++)
            {
                for (int j = 0; j < info.Basis.Size; j++)
                {
                    double G = Quadratures.TriangleGauss18(GetGrad(i, j, a, b, c));
                    double M = Quadratures.TriangleGauss18((double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
                    local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M) * D;
                }
            }
        }

        protected virtual void BuildLocalB(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            for (int i = 0; i < info.Basis.Size; i++)
            {
                // TODO: Can be optimized by checking if vertice == -1
                localb[i] = Quadratures.TriangleGauss18((double ksi, double etta) =>
                {
                    double x = a.X * ksi + b.X * etta + c.X * (1 - ksi - etta);
                    double y = a.Y * ksi + b.Y * etta + c.Y * (1 - ksi - etta);

                    return e.Material.F(x, y) * psi[i](ksi, etta);
                });
                localb[i] *= D;
            }
        }

        private void AddLocalToGlobal(IMatrix A, double[] b, FiniteElement e)
        {
            for (int i = 0; i < info.Basis.Size; i++)
            {
                if (e[i] == -1)
                    continue;

                for (int j = 0; j < info.Basis.Size; j++)
                {
                    if (e[j] == -1)
                        continue;

                    A.Add(e[i], e[j], local[i, j]);
                }
            }

            for (int i = 0; i < info.Basis.Size; i++)
            {
                if (e[i] == -1)
                    continue;

                b[e[i]] += localb[i];
            }
        }

        protected virtual void AddFirstBoundary(IMatrix A, double[] b, FirstBoundaryEdge edge)
        {
            // TODO: add second basis for first order edges
            var basis = edge.Order == 1 ? info.BoundaryBasisFirstOrder : info.BoundaryBasisThirdOrder;

            double[,] M = basis.MassMatrix;

            Func<double, double, double> ug = edge.F;

            double x0 = info.Mesh.Points[edge[0]].X;
            double y0 = info.Mesh.Points[edge[0]].Y;
            double x1 = info.Mesh.Points[edge[edge.NodeCount - 1]].X;
            double y1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Y;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)), basis);

            int index = 0;
            for (int i = 0; i < edge.NodeCount; i++)
            {
                if (edge.Order == 1)
                {
                    if (edge[i] != -1)
                    {
                        A.DI[edge[i]] = 1.0e+50;
                        b[edge[i]] = 1.0e+50 * q[index];
                        index++;
                    }
                }
                else
                {
                    if (edge[i] != -1)
                    {
                        A.DI[edge[i]] = 1.0e+50;
                        b[edge[i]] = 1.0e+50 * q[i];
                    }
                }
            }
        }

        protected virtual void AddSecondBoundary(IMatrix A, double[] b, SecondBoundaryEdge edge)
        {
            var basis = edge.Order == 1 ? info.BoundaryBasisFirstOrder : info.BoundaryBasisThirdOrder;

            var boundaryPsi = basis.GetFuncs();

            Func<double, double, double> theta = edge.Theta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            int index = 0;
            for (int i = 0; i < edge.NodeCount; i++)
                if (edge[i] != -1)
                {
                    b[edge[i]] += h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => theta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)) * boundaryPsi[index](ksi));
                    index++;
                }
        }

        protected virtual void AddThirdBoundary(IMatrix A, double[] b, ThirdBoundaryEdge edge)
        {
            var basis = edge.Order == 1 ? info.BoundaryBasisFirstOrder : info.BoundaryBasisThirdOrder;
            var boundaryPsi = basis.GetFuncs();

            double[,] M = basis.MassMatrix;

            Func<double, double, double> ubeta = edge.UBeta;
            double beta = edge.Beta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            int index = 0;
            for (int i = 0; i < edge.NodeCount; i++)
                if (edge[i] != -1)
                {
                    b[edge[i]] += beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => ubeta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0)) * boundaryPsi[index](ksi));
                    index++;
                }

            int indexi = 0;
            int indexj = 0;
            for (int i = 0; i < edge.NodeCount; i++)
            {
                if (edge[i] != -1)
                {
                    for (int j = 0; j < edge.NodeCount; j++)
                    {
                        if (edge[j] != -1)
                        {
                            A.Add(edge[i], edge[j], beta * h * M[indexi, indexj]);
                            indexj++;
                        }
                    }
                    indexi++;
                }
            }
        }

        private Func<double, double, double> GetGrad(int i, int j, Point a, Point b, Point c)
        {
            var J = new Matrix2x2(
                a.X - c.X, a.Y - c.Y,
                b.X - c.X, b.Y - c.Y);

            var JT = J.Transpose();

            var JJT = J * JT;
            var JJTinv = JJT.Inverse();

            return (double ksi, double etta) =>
            {
                double[] gradi = new double[2] { psiDers["ksi"][i](ksi, etta), psiDers["etta"][i](ksi, etta) };
                double[] gradj = new double[2] { psiDers["ksi"][j](ksi, etta), psiDers["etta"][j](ksi, etta) };

                double[] v = gradi * JJTinv;
                return v[0] * gradj[0] + v[1] * gradj[1];
            };
        }
    }
}