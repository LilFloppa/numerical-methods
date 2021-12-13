using MathUtilities;
using System;
using System.Collections.Generic;

namespace PotOfWater
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

    public class LineLinearLagrange : IOneDimBasis
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

    public class LineCubicLagrange : IOneDimBasis
    {
        public int Size => 4;
        public BasisType Type => BasisType.Cubic;
        public Func<double, double>[] GetFuncs()
        {
            return new Func<double, double>[]
            {
                (double x) => 0.5 * (1 - x) * (3 * (1 - x) - 1) * (3 * (1 - x) - 2),
                (double x) => 4.5 * (1 - x) * x* (3 * (1 - x) - 1),
                (double x) => 4.5 * (1 - x) * x * (3 * x - 1),
                (double x) => 0.5 * x * (3 * x - 1) * (3 * x - 2)
            };
        }
        public Dictionary<string, Func<double, double>[]> GetDers()
        {
            throw new NotImplementedException();
            //return new Dictionary<string, Func<double, double>[]>
            //{
            //    ["x"] = new Func<double, double>[2]
            //    {
            //        (double x) => 1.0,
            //        (double x) => -1.0
            //    }
            //};
        }

        public double[,] MassMatrix => new double[4, 4]
        {
            {  8.0 / 105, 33.0 / 560, -3.0 / 140, 19.0 / 1680 },
            { 33.0 / 560, 27.0 / 70, -27.0 / 560, -3.0 / 140 },
            { -3.0 / 140, -27.0 / 560, 27.0 / 70, 33.0 / 560 },
            { 19.0 / 1680, -3.0 / 140, 33.0 / 560, 8.0 / 105 }
        };
    }

    public class TriangleCubicLagrange : ITwoDimBasis
    {
        public int Size => 10;
        public BasisType Type => BasisType.Cubic;

        Func<double, double, double>[] ITwoDimBasis.GetFuncs()
        {
            throw new NotImplementedException();
        }

        Dictionary<string, Func<double, double, double>[]> ITwoDimBasis.GetDers()
        {
            throw new NotImplementedException();
        }
    }

    public class TriangleLinearLagrange : ITwoDimBasis
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
        public const int BasisSize = 10;

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

        static public Func<double, double, double, double>[] LBasis = new Func<double, double, double, double>[BasisSize]
        {
            (double L1, double L2, double L3) => 0.5 * L1 * (3 * L1 - 1) * (3 * L1 - 2),
            (double L1, double L2, double L3) => 0.5 * L2 * (3 * L2 - 1) * (3 * L2 - 2),
            (double L1, double L2, double L3) => 0.5 * L3 * (3 * L3 - 1) * (3 * L3 - 2),

            (double L1, double L2, double L3) => 9.0 * L1 * L2 * (3 * L1 - 1) / 2.0,
            (double L1, double L2, double L3) => 9.0 * L1 * L2 * (3 * L2 - 1) / 2.0,

            (double L1, double L2, double L3) => 9.0 * L3 * L1 * (3 * L1 - 1) / 2.0,
            (double L1, double L2, double L3) => 9.0 * L3 * L1 * (3 * L3 - 1) / 2.0,

            (double L1, double L2, double L3) => 9.0 * L2 * L3 * (3 * L2 - 1) / 2.0,
            (double L1, double L2, double L3) => 9.0 * L2 * L3 * (3 * L3 - 1) / 2.0,

            (double L1, double L2, double L3) => 27.0 * L1 * L2 * L3
        };
    }
}
