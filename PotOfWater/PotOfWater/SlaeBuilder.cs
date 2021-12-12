using MathUtilities;
using System;
using System.Collections.Generic;

namespace PotOfWater
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
                for (int j = 0; j < info.Basis.Size; j++)
                {
                    double G = Quadratures.TriangleGauss18(GetGrad(i, j, a, b, c));
                    double M = Quadratures.TriangleGauss18((double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
                    local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M) * D;
                }
        }

        protected virtual void BuildLocalB(FiniteElement e)
        {
            // TODO: implement this method

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

                    return e.Material.F(x, y, 0) * psi[i](ksi, etta);
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

            Func<double, double,  double, double> ugtime = edge.F;
            Func<double, double, double> ug = (double x, double y) => ugtime(x, y, 0);

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
            Func<double, double, double, double> thetatime = edge.Theta;
            Func<double, double, double> theta = (double x, double y) => thetatime(x, y, 0);

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

            Func<double, double, double, double> ubetatime = edge.UBeta;
            Func<double, double, double> ubeta = (double x, double y) => ubetatime(x, y, 0);
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
                //return
                //(psiDers["ksi"][i](ksi, etta) * JJTinv.a11 + psiDers["etta"][i](ksi, etta) * JJTinv.a21) * psiDers["ksi"][j](ksi, etta) +
                //(psiDers["ksi"][i](ksi, etta) * JJTinv.a12 + psiDers["etta"][i](ksi, etta) * JJTinv.a22) * psiDers["etta"][j](ksi, etta);
            };

            //Func<double, double, double> ksiDer = (double ksi, double etta) => psiDers["ksi"][i](ksi, etta) * psiDers["ksi"][j](ksi, etta);
            //Func<double, double, double> ettaDer = (double ksi, double etta) => psiDers["etta"][i](ksi, etta) * psiDers["etta"][j](ksi, etta);

            //return (double ksi, double etta) => ksiDer(ksi, etta) + ettaDer(ksi, etta);
        }
    }

    public class LSlaeBuilder : SlaeBuilder
    {
        public LSlaeBuilder(ProblemInfo info) : base(info) { }

        protected override void BuildLocalMatrix(FiniteElement e)
        {
            // TODO: implement this method
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            double[] alpha = Alpha(a, b, c);

            (double a1, double a2)[] grads = new (double, double)[3]
            {
                (alpha[0], alpha[1]),
                (alpha[2], alpha[3]),
                (alpha[4], alpha[5])
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double M = Quadratures.TriangleGauss18((double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
                    double G = (grads[i].a1 * grads[j].a1 + grads[i].a2 * grads[j].a2) / 2;
                    local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M) * D;  
                }
            }
        }

        private double[] Alpha(Point a, Point b, Point c)
        {
            double[] alpha = new double[6];
            double x1 = a.X;
            double y1 = a.Y;
            double x2 = b.X;
            double y2 = b.Y;
            double x3 = c.X;
            double y3 = c.Y;

            double D = Utilities.Det(a, b, c);

            alpha[0] = (y2 - y3) / D;
            alpha[1] = (x3 - x2) / D;
            alpha[2] = (y3 - y1) / D;
            alpha[3] = (x1 - x3) / D;
            alpha[4] = (y1 - y2) / D;
            alpha[5] = (x2 - x1) / D;

            return alpha;
        }
    }

    public class TimeSlaeBuilder
    {
        public ILayer Layer { get; set; } = null;
        public double CurrentT { get; set; }

        protected ProblemInfo info;

        protected Point[] points;

        protected double[,] local;
        protected double[] localb;

        protected double[,] massMatrix;

        protected Func<double, double, double>[] psi;
        protected Dictionary<string, Func<double, double, double>[]> psiDers;

        protected Func<double, double>[] boundaryPsi;

        public TimeSlaeBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;

            local = new double[info.Basis.Size, info.Basis.Size];
            localb = new double[info.Basis.Size];

            psi = info.Basis.GetFuncs();
            psiDers = info.Basis.GetDers();

            boundaryPsi = info.BoundaryBasis.GetFuncs();

            CalculateMassMatrix();
        }

        private void CalculateMassMatrix()
        {
            massMatrix = new double[info.Basis.Size, info.Basis.Size];

            for (int i = 0; i < info.Basis.Size; i++)
                for (int j = 0; j < info.Basis.Size; j++)
                    massMatrix[i, j] = Quadratures.TriangleGauss18((double ksi, double etta) => psi[i](ksi, etta) * psi[j](ksi, etta));
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

        protected void BuildLocalMatrix(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            double[] alpha = Alpha(a, b, c);

            (double a1, double a2)[] grads = new (double, double)[3]
            {
                (alpha[0], alpha[1]),
                (alpha[2], alpha[3]),
                (alpha[4], alpha[5])
            };

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    double M = massMatrix[i, j];
                    double G = (grads[i].a1 * grads[j].a1 + grads[i].a2 * grads[j].a2) / 2;

                    if (Layer != null)
                        local[i, j] = (e.Material.Lambda * G + Layer.Coeff0 * e.Material.RoCp * M) * D;
                    else
                        local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M) * D;
                }
            }
        }

        protected virtual void BuildLocalB(FiniteElement e)
        {
            // TODO: implement this method

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

                    return e.Material.F(x, y, CurrentT) * psi[i](ksi, etta);
                });


                if (Layer != null)
                {
                    for (int k = 0; k < Layer.Size; k++)
                        for (int j = 0; j < info.Basis.Size; j++)
                            localb[i] += Layer.Coeffs[k] * massMatrix[i, j] * Layer.Q[k][e[j]] * e.Material.RoCp;
                }

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

            Func<double, double, double, double> ug = edge.F;

            double x0 = info.Mesh.Points[edge[0]].X;
            double y0 = info.Mesh.Points[edge[0]].Y;
            double x1 = info.Mesh.Points[edge[edge.NodeCount - 1]].X;
            double y1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Y;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0), CurrentT), info.BoundaryBasis);

            for (int i = 0; i < edge.NodeCount; i++)
            {
                A.DI[edge[i]] = 1.0e+50;
                b[edge[i]] = 1.0e+50 * q[i];
            }
        }

        protected virtual void AddSecondBoundary(IMatrix A, double[] b, SecondBoundaryEdge edge)
        {
            Func<double, double, double, double> theta = edge.Theta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => theta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0), CurrentT) * boundaryPsi[i](ksi));
        }

        protected virtual void AddThirdBoundary(IMatrix A, double[] b, ThirdBoundaryEdge edge)
        {
            double[,] M = info.BoundaryBasis.MassMatrix;

            Func<double, double, double, double> ubeta = edge.UBeta;
            double beta = edge.Beta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double x0 = p1.X;
            double y0 = p1.Y;
            double x1 = p2.X;
            double y1 = p2.Y;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => ubeta(x0 + ksi * (x1 - x0), y0 + ksi * (y1 - y0), CurrentT) * boundaryPsi[i](ksi));

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                for (int j = 0; j < info.BoundaryBasis.Size; j++)
                    A.Add(edge[i], edge[j], beta * h * M[i, j]);
        }

        private double[] Alpha(Point a, Point b, Point c)
        {
            double[] alpha = new double[6];
            double x1 = a.X;
            double y1 = a.Y;
            double x2 = b.X;
            double y2 = b.Y;
            double x3 = c.X;
            double y3 = c.Y;

            double D = Utilities.Det(a, b, c);

            alpha[0] = (y2 - y3) / D;
            alpha[1] = (x3 - x2) / D;
            alpha[2] = (y3 - y1) / D;
            alpha[3] = (x1 - x3) / D;
            alpha[4] = (y1 - y2) / D;
            alpha[5] = (x2 - x1) / D;

            return alpha;
        }
    }
}
