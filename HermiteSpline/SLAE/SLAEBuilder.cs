using HermiteSpline.Meshes;
using HermiteSpline.Spline;
using MathUtilities;
using System;

namespace HermiteSpline.SLAE
{
    public class SLAEInfo
    {
        public (Point p, double f)[] Data;
        public Mesh Mesh;

        public double Alpha;
        public double Beta;
    }

    public class SLAEBuilder
    {
        public SLAEInfo Info { get; set; }
        public SLAEBuilder(SLAEInfo info) => Info = info;

        private double[,] localM = new double[HermiteBasis.Size, HermiteBasis.Size];
        private double[] localB = new double[HermiteBasis.Size];

        public void Build(IMatrix A, double[] b)
        {
            foreach (Element e in Info.Mesh)
            {
                ClearLocals();
                BuildLocalMatrix(e);
                BuildLocalRight(e);
                AddLocalToGlobal(A, b, e);
            }
        }
        private void ClearLocals()
        {
            for (int i = 0; i < HermiteBasis.Size; i++)
                for (int j = 0; j < HermiteBasis.Size; j++)
                    localM[i, j] = 0.0;

            Array.Fill(localB, 0.0);
        }
        private void BuildLocalMatrix(Element e)
        {
            double hx = e.X2 - e.X1;
            double hy = e.Y2 - e.Y1;

            for (int i = 0; i < HermiteBasis.Size; i++)
            {
                Func<double, double, double> psiI = HermiteBasis.GetPsi(i, hx, hy);
                Func<double, double, double> D1psiI = HermiteBasis.GetPsiDxDy(i, hx, hy);
                Func<double, double, double> D2psiI = HermiteBasis.GetPsiDx2Dy2(i, hx, hy);

                for (int j = 0; j < HermiteBasis.Size; j++)
                {
                    Func<double, double, double> psiJ = HermiteBasis.GetPsi(j, hx, hy);
                    Func<double, double, double> D1psiJ = HermiteBasis.GetPsiDxDy(j, hx, hy);
                    Func<double, double, double> D2psiJ = HermiteBasis.GetPsiDx2Dy2(j, hx, hy);

                    foreach (var index in e.DataIndices)
                    {
                        Point p = Info.Data[index].p;
                        localM[i, j] += 
                            psiI(p.X, p.Y) * psiJ(p.X, p.Y) + 
                            Info.Alpha * Quadratures.Gauss7((double x, double y) => D2psiI(x, y) * D2psiJ(x, y), e.X1, e.X2, e.Y1, e.Y2) +
                            Info.Beta * Quadratures.Gauss7((double x, double y) => D1psiI(x, y) * D1psiJ(x, y), e.X1, e.X2, e.Y1, e.Y2);
                    }
                }
            }
        }
        private void BuildLocalRight(Element e)
        {
            double hx = e.X2 - e.X1;
            double hy = e.Y2 - e.Y1;

            for (int i = 0; i < HermiteBasis.Size; i++)
            {
                Func<double, double, double> psi = HermiteBasis.GetPsi(i, hx, hy);

                foreach (var index in e.DataIndices)
                {
                    Point p = Info.Data[index].p;
                    double f = Info.Data[index].f;
                    localB[i] += psi(p.X, p.Y) * f;
                }
            }
        }
        private void AddLocalToGlobal(IMatrix A, double[] b, Element e)
        {
            for (int i = 0; i < HermiteBasis.Size; i++)
                for (int j = 0; j < HermiteBasis.Size; j++)
                    A.Add(e.Indices[i], e.Indices[j], localM[i, j]);

            for (int i = 0; i < HermiteBasis.Size; i++)
                b[e.Indices[i]] += localB[i];
        }
    }
}
