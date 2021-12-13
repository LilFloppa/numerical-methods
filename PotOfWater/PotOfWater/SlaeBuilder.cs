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
        public double CurrentT { get; set; } = 0.0;
        private double[,] M = new double[3, 3];
        public LSlaeBuilder(ProblemInfo info) : base(info) { }

        protected override void BuildLocalMatrix(FiniteElement e)
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


            ClearM();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        int[] v = new int[3];
                        v[i]++;
                        v[j]++;
                        v[k]++;
                        double r = points[e[k]].R;

                        M[i, j] += Integral(v[0], v[1], v[2]) * r;
                    }

                    double G = (grads[i].a1 * grads[j].a1 + grads[i].a2 * grads[j].a2) / 2;
                    G *= (a.R + b.R + c.R) / 6;

                    local[i, j] = (e.Material.Lambda * G + e.Material.RoCp * M[i, j]) * D;  
                }
            }
        }
        protected override void BuildLocalB(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));

            //for (int i = 0; i < 3; i++)
            //{
            //    localb[i] = Quadratures.TriangleGauss18((double ksi, double etta) =>
            //    {
            //        double r = a.R * ksi + b.R * etta + c.R * (1 - ksi - etta);
            //        double z = a.Z * ksi + b.Z * etta + c.Z * (1 - ksi - etta);

            //        return e.Material.F(r, z, 0) * psi[i](ksi, etta) * r;
            //    });
            //    localb[i] *= D;
            //}

            double[] f = new double[3];
            for (int i = 0; i < 3; i++)
            {
                var p = points[e[i]];
                f[i] = e.Material.F(p.R, p.Z, CurrentT);
            }

            for (int i = 0; i < 3; i++)
            {
                localb[i] = f[0] * M[i, 0];

                for (int j = 1; j < 3; j++)
                    localb[i] += f[j] * M[i, j];

                localb[i] *= D;
            }
        }
        protected override void AddFirstBoundary(IMatrix A, double[] b, FirstBoundaryEdge edge)
        {
            double[,] M = info.BoundaryBasis.MassMatrix;

            Func<double, double, double, double> ugtime = edge.F;
            Func<double, double, double> ug = (double r, double z) => ugtime(r, z, 0);

            double r0 = info.Mesh.Points[edge[0]].R;
            double z0 = info.Mesh.Points[edge[0]].Z;
            double r1 = info.Mesh.Points[edge[edge.NodeCount - 1]].R;
            double z1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Z;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)), info.BoundaryBasis);

            for (int i = 0; i < edge.NodeCount; i++)
            {
                A.DI[edge[i]] = 1.0e+50;
                b[edge[i]] = 1.0e+50 * q[i];
            }
        }
        protected override void AddSecondBoundary(IMatrix A, double[] b, SecondBoundaryEdge edge)
        {
            Func<double, double, double, double> thetatime = edge.Theta;
            Func<double, double, double> theta = (double r, double z) => thetatime(r, z, 0);

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double r0 = p1.R;
            double z0 = p1.Z;
            double r1 = p2.R;
            double z1 = p2.Z;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => theta(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)) * boundaryPsi[i](ksi) * (r0 + ksi * (r1 - r0)));
        }
        protected override void AddThirdBoundary(IMatrix A, double[] b, ThirdBoundaryEdge edge)
        {
            Func<double, double, double, double> ubetatime = edge.UBeta;
            Func<double, double, double> ubeta = (double r, double z) => ubetatime(r, z, 0);
            double beta = edge.Beta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double z0 = p1.R;
            double r0 = p1.Z;
            double z1 = p2.R;
            double r1 = p2.Z;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => ubeta(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)) * boundaryPsi[i](ksi) * (r0 + ksi * (r1 - r0)));

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                for (int j = 0; j < info.BoundaryBasis.Size; j++)
                {
                    double M = beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => boundaryPsi[i](ksi) * boundaryPsi[j](ksi) * (r0 + ksi * (r1 - r0)));
                    A.Add(edge[i], edge[j], M);
                }
        }
        private void ClearM()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    M[i, j] = 0.0;
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
        private double Integral(int v1, int v2, int v3)
        {
            return Utilities.Factorial(v1) * Utilities.Factorial(v2) * Utilities.Factorial(v3) / (double)Utilities.Factorial(v1 + v2 + v3 + 2);
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

    public class CubicSlaeBuilder
    {
        private ProblemInfo info;

        private Point[] points;

        private double[,] local;
        private double[] localb;

        private List<LInfo.LocalCompG>[,] gPattern;
        private List<LInfo.LocalCompM>[,] mPattern;

        private double[][] M;

        private Func<double, double>[] boundaryPsi;

        public CubicSlaeBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;


            local = new double[LInfo.BasisSize, LInfo.BasisSize];
            localb = new double[LInfo.BasisSize];

            BuildPatterns();

            boundaryPsi = info.BoundaryBasis.GetFuncs();
        }

        public void Build(IMatrix A, double[] b)
        {
            foreach (FiniteElement e in info.Mesh.Elements)
            {
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

        private void AddFirstBoundary(IMatrix A, double[] b, FirstBoundaryEdge edge)
        {
            double[,] M = info.BoundaryBasis.MassMatrix;

            Func<double, double, double, double> ugtime = edge.F;
            Func<double, double, double> ug = (double r, double z) => ugtime(r, z, 0);

            double r0 = info.Mesh.Points[edge[0]].R;
            double z0 = info.Mesh.Points[edge[0]].Z;
            double r1 = info.Mesh.Points[edge[edge.NodeCount - 1]].R;
            double z1 = info.Mesh.Points[edge[edge.NodeCount - 1]].Z;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)), info.BoundaryBasis);

            for (int i = 0; i < edge.NodeCount; i++)
            {
                A.DI[edge[i]] = 1.0e+50;
                b[edge[i]] = 1.0e+50 * q[i];
            }
        }

        private void AddSecondBoundary(IMatrix A, double[] b, SecondBoundaryEdge edge)
        {
            Func<double, double, double, double> thetatime = edge.Theta;
            Func<double, double, double> theta = (double x, double y) => thetatime(x, y, 0);

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double r0 = p1.R;
            double z0 = p1.Z;
            double r1 = p2.R;
            double z1 = p2.Z;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => theta(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)) * boundaryPsi[i](ksi) * (r0 + ksi * (r1 - r0)));
        }

        private void AddThirdBoundary(IMatrix A, double[] b, ThirdBoundaryEdge edge)
        {
            Func<double, double, double, double> ubetatime = edge.UBeta;
            Func<double, double, double> ubeta = (double x, double y) => ubetatime(x, y, 0);
            double beta = edge.Beta;

            Point p1 = info.Mesh.Points[edge[0]];
            Point p2 = info.Mesh.Points[edge[edge.NodeCount - 1]];

            double z0 = p1.R;
            double r0 = p1.Z;
            double z1 = p2.R;
            double r1 = p2.Z;

            double h = Point.Distance(p1, p2);

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                b[edge[i]] += beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => ubeta(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)) * boundaryPsi[i](ksi) * (r0 + ksi * (r1 - r0)));

            for (int i = 0; i < info.BoundaryBasis.Size; i++)
                for (int j = 0; j < info.BoundaryBasis.Size; j++)
                {
                    double M = beta * h * Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => boundaryPsi[i](ksi) * boundaryPsi[j](ksi) * (r0 + ksi * (r1 - r0))); 
                    A.Add(edge[i], edge[j], M);
                }
        }

        private void BuildPatterns()
        {
            gPattern = new List<LInfo.LocalCompG>[LInfo.BasisSize, LInfo.BasisSize];
            mPattern = new List<LInfo.LocalCompM>[LInfo.BasisSize, LInfo.BasisSize];

            for (int i = 0; i < LInfo.BasisSize; i++)
                for (int j = 0; j < LInfo.BasisSize; j++)
                {
                    gPattern[i, j] = new List<LInfo.LocalCompG>();
                    mPattern[i, j] = new List<LInfo.LocalCompM>();
                }

            M = new double[LInfo.BasisSize][];
            for (int i = 0; i < LInfo.BasisSize; i++)
                M[i] = new double[LInfo.BasisSize];

            for (int k = 0; k < 3; k++)
            {
                int rnum = 0;
                int v11 = 0, v22 = 0, v33 = 0;
                if (k == 0) { v11++; rnum = 0; }
                else if (k == 1) { v22++; rnum = 1; }
                else { v33++; rnum = 2; }

                for (int i = 0; i < LInfo.BasisSize; i++)
                    for (int j = 0; j < LInfo.BasisSize; j++)
                    {
                        foreach (var r in LInfo.Ders[i])
                            foreach (var z in LInfo.Ders[j])
                            {
                                int v1 = r.V1 + z.V1 + v11;
                                int v2 = r.V2 + z.V2 + v22;
                                int v3 = r.V3 + z.V3 + v33;

                                double coeff = r.Coefficient * z.Coefficient * Utilities.Factorial(v1) * Utilities.Factorial(v2) * Utilities.Factorial(v3) / (double)Utilities.Factorial(v1 + v2 + v3 + 2);
                                gPattern[i, j].Add(new LInfo.LocalCompG(r.GradNo, z.GradNo, coeff, rnum));
                            }

                        double sum = 0;
                        foreach (var r in LInfo.Basis[i])
                            foreach (var z in LInfo.Basis[j])
                            {
                                int v1 = r.V1 + z.V1 + v11;
                                int v2 = r.V2 + z.V2 + v22;
                                int v3 = r.V3 + z.V3 + v33;

                                sum += r.Coefficient * z.Coefficient * Utilities.Factorial(v1) * Utilities.Factorial(v2) * Utilities.Factorial(v3) / (double)Utilities.Factorial(v1 + v2 + v3 + 2);
                            }

                        mPattern[i, j].Add(new LInfo.LocalCompM(sum, rnum));
                    }
            }
        }

        private void BuildLocalMatrix(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));
            double[] alpha = Alpha(a, b, c);

            (double R, double Z)[] grads = new (double R, double Z)[3]
            {
                new (alpha[0], alpha[1]),
                new (alpha[2], alpha[3]),
                new (alpha[4], alpha[5])
            };

            for (int i = 0; i < LInfo.BasisSize; i++)
                Array.Fill(M[i], 0.0);

            for (int i = 0; i < LInfo.BasisSize; i++)
                for (int j = 0; j < LInfo.BasisSize; j++)
                {
                    double G = 0;
                    foreach (LInfo.LocalCompG comp in gPattern[i, j])
                    {
                        double scalGrad = grads[comp.Grad1].R * grads[comp.Grad2].R + grads[comp.Grad1].Z * grads[comp.Grad2].Z;
                        G += comp.Coefficient * scalGrad * points[e.Vertices[comp.Rnum]].R;
                    }

                    foreach (LInfo.LocalCompM comp in mPattern[i, j])
                        M[i][j] += comp.Coefficient * points[e.Vertices[comp.Rnum]].R;

                    local[i, j] = (e.Material.RoCp * M[i][j] + e.Material.Lambda * G) * D;
                }
        }

        private void BuildLocalB(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            Point[] coords = Utilities.TriangleCubicPoints(a, b, c);
            double D = Math.Abs(Utilities.Det(a, b, c));

            double[] temp = new double[LInfo.BasisSize];
            for (int i = 0; i < LInfo.BasisSize; i++)
                temp[i] = e.Material.F(coords[i].R, coords[i].Z, 0.0);

            for (int i = 0; i < LInfo.BasisSize; i++)
            {
                localb[i] = temp[0] * M[i][0];

                for (int j = 1; j < LInfo.BasisSize; j++)
                    localb[i] += temp[j] * M[i][j];

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
    };
}
