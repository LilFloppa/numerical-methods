using MathUtilities;
using System;
using System.Collections.Generic;

namespace OrderHarmonization
{
    public class HarmonicSlaeBuilder
    {
        protected ProblemInfo info;

        protected Point[] points;

        protected double[,] local;
        protected double[] localb;

        protected Func<double, double, double>[] psi;
        protected Dictionary<string, Func<double, double, double>[]> psiDers;

        List<LInfo.LocalCompG>[,] gPattern;
        List<LInfo.LocalCompM>[,] mPattern;

        double[][] M;

        public HarmonicSlaeBuilder(ProblemInfo info)
        {
            this.info = info;

            points = info.Mesh.Points;

            local = new double[info.Basis.Size, info.Basis.Size];
            localb = new double[info.Basis.Size];

            psi = info.Basis.GetFuncs();
            psiDers = info.Basis.GetDers();

            BuildPatterns();
        }

        void BuildPatterns()
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

        void BuildLocalMatrix(FiniteElement e)
        {
            Point a = points[e[0]];
            Point b = points[e[1]];
            Point c = points[e[2]];

            double D = Math.Abs(Utilities.Det(a, b, c));
            double[] alpha = Utilities.Alpha(a, b, c);

            LInfo.Grad[] grads = new LInfo.Grad[3]
            {
                new LInfo.Grad(alpha[0], alpha[1]),
                new LInfo.Grad(alpha[2], alpha[3]),
                new LInfo.Grad(alpha[4], alpha[5])
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
                        G += comp.Coefficient * scalGrad * points[e[comp.Rnum]].R;
                    }

                    foreach (LInfo.LocalCompM comp in mPattern[i, j])
                        M[i][j] += comp.Coefficient * points[e.Vertices[comp.Rnum]].R;

                    local[i, j] = (e.Material.RoCp * M[i][j] + e.Material.Lambda * G) * D;
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
                    double r = a.R * ksi + b.R * etta + c.R * (1 - ksi - etta);
                    double z = a.Z * ksi + b.Z * etta + c.Z * (1 - ksi - etta);

                    return e.Material.F(r, z) * psi[i](ksi, etta) * r;
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
            var basis = edge.Order == 1 ? info.BoundaryBasisFirstOrder : info.BoundaryBasisThirdOrder;
            double[,] M = basis.MassMatrix;

            Func<double, double, double> ug = edge.F;

            double r0 = points[edge[0]].R;
            double z0 = points[edge[0]].Z;
            double r1 = points[edge[edge.NodeCount - 1]].R;
            double z1 = points[edge[edge.NodeCount - 1]].Z;

            double[] q = BasisHelpers.ExpandInBasis((double ksi) => ug(r0 + ksi * (r1 - r0), z0 + ksi * (z1 - z0)), basis);

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
    }
}