using MathUtilities;
using System;
using System.Collections.Generic;

namespace OrderHarmonization
{
    public enum BasisType
    {
        Linear,
        Quadratic,
        Cubic
    }

    public interface IBasis
    {
        public int Size { get; }
        public BasisType Type { get; }
    }

    public interface IOneDimBasis : IBasis
    {
        public Func<double, double>[] GetFuncs();
        public Dictionary<string, Func<double, double>[]> GetDers();

        public double[,] MassMatrix { get; }
    }

    public interface ITwoDimBasis : IBasis
    {
        public Func<double, double, double>[] GetFuncs();
        public Dictionary<string, Func<double, double, double>[]> GetDers();
    }

    public class LineLinearHierarchical : IOneDimBasis
    {
        public int Size => 2;
        public BasisType Type => BasisType.Linear;
        public Func<double, double>[] GetFuncs()
        {
            return new Func<double, double>[2]
            {
                (double x) => 1.0 - x,
                (double x) => x
            };
        }
        public Dictionary<string, Func<double, double>[]> GetDers()
        {
            return new Dictionary<string, Func<double, double>[]>
            {
                ["x"] = new Func<double, double>[2]
                {
                    (double x) => 1.0,
                    (double x) => -1.0
                }
            };
        }

        public double[,] MassMatrix => new double[2, 2]
        {
            { 1.0 / 3.0,  1.0 / 6.0 },
            { 1.0 / 6.0,  1.0 / 3.0 },
        };
    }

    public class LineQuadHierarchical : IOneDimBasis
    {
        public int Size => 3;
        public BasisType Type => BasisType.Cubic;
        public Func<double, double>[] GetFuncs()
        {
            return new Func<double, double>[3]
            {
                (double x) => 1.0 - x,
                (double x) => x * (1.0 - x),
                (double x) => x
            };
        }
        public Dictionary<string, Func<double, double>[]> GetDers()
        {
            throw new NotImplementedException();
        }

        public double[,] MassMatrix => new double[3, 3]
        {
            { 1.0 / 3,      1.0 / 12,   1.0 / 6 },
            { 1.0 / 12,     1.0 / 30,   1.0 / 12 },
            { 1.0 / 6,     1.0 / 12,   1.0 / 3 },
        };
    }

    public class TriangleLinearHierarchical : ITwoDimBasis
    {
        public int Size => 3;
        public BasisType Type => BasisType.Linear;
        public Func<double, double, double>[] GetFuncs()
        {
            return new Func<double, double, double>[3]
            {
                 (double ksi, double etta) => ksi,
                 (double ksi, double etta) => etta,
                 (double ksi, double etta) => 1.0 - ksi - etta,
            };
        }
        public Dictionary<string, Func<double, double, double>[]> GetDers()
        {
            return new Dictionary<string, Func<double, double, double>[]>
            {
                ["ksi"] = new Func<double, double, double>[3]
                {
                    (double ksi, double etta) => 1.0,
                    (double ksi, double etta) => 0.0,
                    (double ksi, double etta) => -1.0,
                },

                ["etta"] = new Func<double, double, double>[3]
                {
                    (double ksi, double etta) => 0.0,
                    (double ksi, double etta) => 1.0,
                    (double ksi, double etta) => -1.0,
                }
            };
        }
    }

    public class TriangleQuadHierarchical : ITwoDimBasis
    {
        public int Size => 6;
        public BasisType Type => BasisType.Cubic;

        public Func<double, double, double>[] GetFuncs()
        {
            Func<double, double, double> L1 = (k, e) => k;
            Func<double, double, double> L2 = (k, e) => e;
            Func<double, double, double> L3 = (k, e) => 1 - k - e;

            return new Func<double, double, double>[6]
            {
                 (k,e) => L1(k, e),
                 (k,e) => L2(k, e),
                 (k,e) => L3(k, e),
                 (k,e) => L1(k, e) * L2(k, e),
                 (k,e) => L1(k, e) * L3(k, e),
                 (k,e) => L2(k, e) * L3(k, e),
            };
        }
        public Dictionary<string, Func<double, double, double>[]> GetDers()
        {
            Func<double, double, double> L1 = (k, e) => k;
            Func<double, double, double> L2 = (k, e) => e;
            Func<double, double, double> L3 = (k, e) => 1 - k - e;

            double dL1dk = 1.0;
            double dL2dk = 0.0;
            double dL3dk = -1.0;

            double dL1de = 0.0;
            double dL2de = 1.0;
            double dL3de = -1.0;

            return new Dictionary<string, Func<double, double, double>[]>
            {
                ["ksi"] = new Func<double, double, double>[6]
                {
                    (k, e) => dL1dk,
                    (k, e) => dL2dk,
                    (k, e) => dL3dk,

                    (k,e) => dL1dk * L2(k, e) + L1(k, e) * dL2dk,
                    (k,e) => dL1dk * L3(k, e) + L1(k, e) * dL3dk,
                    (k,e) => dL2dk * L3(k, e) + L2(k, e) * dL3dk,
                },

                ["etta"] = new Func<double, double, double>[6]
                {
                    (k, e) => dL1de,
                    (k, e) => dL2de,
                    (k, e) => dL3de,

                    (k,e) => dL1de * L2(k, e) + L1(k, e) * dL2de,
                    (k,e) => dL1de * L3(k, e) + L1(k, e) * dL3de,
                    (k,e) => dL2de * L3(k, e) + L2(k, e) * dL3de,
                }
            };
        }
    }

    public static class BasisHelpers
    {
        public static double[] ExpandInBasis(Func<double, double> f, IOneDimBasis basis)
        {
            if (basis.Type == BasisType.Linear)
                return new double[2] { f(0.0), f(1.0) };

            var psi = basis.GetFuncs();
            double[] b = new double[basis.Size];
            for (int i = 0; i < basis.Size; i++)
                b[i] = Quadratures.NewtonCotes(0.0, 1.0, (double ksi) => f(ksi) * psi[i](ksi));

            double[] q = new double[basis.Size];
            Gauss.Solve(basis.MassMatrix, q, b);

            return q;
        }
    }

     public class LInfo
    {
        public const int BasisSize = 6;

        public struct DerComp
        {
            public DerComp(int gradNo, double coefficient, int v1, int v2, int v3)
            {
                GradNo = gradNo;
                Coefficient = coefficient;
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }
            public int GradNo { get; set; }
            public double Coefficient { get; set; }
            public int V1 { get; set; }
            public int V2 { get; set; }
            public int V3 { get; set; }
        };

        public struct PsiComp
        {
            public PsiComp(double coefficient, int v1, int v2, int v3)
            {
                Coefficient = coefficient;
                V1 = v1;
                V2 = v2;
                V3 = v3;
            }

            public double Coefficient { get; set; }
            public int V1 { get; set; }
            public int V2 { get; set; }
            public int V3 { get; set; }
        };

        public struct LocalCompG
        {
            public LocalCompG(int grad1, int grad2, double coefficient, int rnum)
            {
                Grad1 = grad1;
                Grad2 = grad2;
                Coefficient = coefficient;
                Rnum = rnum;
            }

            public int Grad1 { get; set; }
            public int Grad2 { get; set; }
            public double Coefficient { get; set; }
            public int Rnum { get; set; }
        };

        public struct LocalCompM
        {
            public LocalCompM(double coefficient, int rnum)
            {
                Coefficient = coefficient;
                Rnum = rnum;
            }

            public double Coefficient { get; set; }
            public int Rnum { get; set; }
        };

        static public List<List<DerComp>> Ders = new List<List<DerComp>>()
        {
            new List<DerComp>() { new DerComp(0, 13.5, 2, 0, 0),  new DerComp(0, -9.0, 1, 0, 0),   new DerComp(0, 1.0, 0, 0, 0) },
            new List<DerComp>() { new DerComp(1, 13.5, 0, 2, 0),  new DerComp(1, -9.0, 0, 1, 0),   new DerComp(1, 1.0, 0, 0, 0) },
            new List<DerComp>() { new DerComp(2, 13.5, 0, 0, 2),  new DerComp(2, -9.0, 0, 0, 1),   new DerComp(2, 1.0, 0, 0, 0) },

            new List<DerComp>() { new DerComp(0, 27.0, 1, 1, 0),  new DerComp(0, -4.5, 0, 1, 0),   new DerComp(1, 13.5, 2, 0, 0),   new DerComp(1, -4.5, 1, 0, 0) }, // 4
			new List<DerComp>() { new DerComp(0, 13.5, 0, 2, 0),  new DerComp(0, -4.5, 0, 1, 0),   new DerComp(1, 27.0, 1, 1, 0),   new DerComp(1, -4.5, 1, 0, 0) }, // 5

			new List<DerComp>() { new DerComp(0, 27.0, 1, 0, 1),  new DerComp(0, -4.5, 0, 0, 1),   new DerComp(2, 13.5, 2, 0, 0),   new DerComp(2, -4.5, 1, 0, 0) }, // 9
			new List<DerComp>() { new DerComp(0, 13.5, 0, 0, 2),  new DerComp(0, -4.5, 0, 0, 1),   new DerComp(2, 27.0, 1, 0, 1),   new DerComp(2, -4.5, 1, 0, 0) }, // 8

			new List<DerComp>() { new DerComp(1, 27.0, 0, 1, 1),  new DerComp(1, -4.5, 0, 0, 1),   new DerComp(2, 13.5, 0, 2, 0),   new DerComp(2, -4.5, 0, 1, 0) }, // 6
			new List<DerComp>() { new DerComp(1, 13.5, 0, 0, 2),  new DerComp(1, -4.5, 0, 0, 1),   new DerComp(2, 27.0, 0, 1, 1),   new DerComp(2, -4.5, 0, 1, 0) }, // 7
			new List<DerComp>() { new DerComp(0, 27.0, 0, 1, 1),  new DerComp(1, 27.0, 1, 0, 1),   new DerComp(2, 27.0, 1, 1, 0) }
        };

        static public List<List<PsiComp>> Basis = new List<List<PsiComp>>()
        {
            new List<PsiComp>() { new PsiComp(4.5, 3, 0, 0),      new PsiComp(-4.5, 2, 0, 0),   new PsiComp(1.0, 1, 0, 0) },
            new List<PsiComp>() { new PsiComp(4.5, 0, 3, 0),      new PsiComp(-4.5, 0, 2, 0),   new PsiComp(1.0, 0, 1, 0) },
            new List<PsiComp>() { new PsiComp(4.5, 0, 0, 3),      new PsiComp(-4.5, 0, 0, 2),   new PsiComp(1.0, 0, 0, 1) },

            new List<PsiComp>() { new PsiComp(13.5, 2, 1, 0),     new PsiComp(-4.5, 1, 1, 0) },
            new List<PsiComp>() { new PsiComp(13.5, 1, 2, 0),     new PsiComp(-4.5, 1, 1, 0) },

            new List<PsiComp>() { new PsiComp(13.5, 2, 0, 1),     new PsiComp(-4.5, 1, 0, 1) },
            new List<PsiComp>() { new PsiComp(13.5, 1, 0, 2),     new PsiComp(-4.5, 1, 0, 1) },

            new List<PsiComp>() { new PsiComp(13.5, 0, 2, 1),     new PsiComp(-4.5, 0, 1, 1) },
            new List<PsiComp>() { new PsiComp(13.5, 0, 1, 2),     new PsiComp(-4.5, 0, 1, 1) },
            new List<PsiComp>() { new PsiComp(27.0, 1, 1, 1) }
        };

        static public Func<double, double, double, double>[] LBasis = new Func<double, double, double, double>[6]
        {
            (double L1, double L2, double L3) => L1,
            (double L1, double L2, double L3) => L2,
            (double L1, double L2, double L3) => L3,
            (double L1, double L2, double L3) => L1 * L2,
            (double L1, double L2, double L3) => L1 * L3,
            (double L1, double L2, double L3) => L2 * L3,
        };
    }
}
