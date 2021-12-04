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
            return new Dictionary<string, Func<double, double>[]>
            {
                ["x"] = new Func<double, double>[2]
                {
                    (double x) => 1.0,
                    (double x) => -1.0
                }
            };
        }

        public double[,] MassMatrix => new double[4, 4]
        {
            {  8.0 / 105, 33.0 / 560, -3.0 / 140, 19.0 / 1680 },
            { 33.0 / 560, 27.0 / 70, -27.0 / 560, -3.0 / 140 },
            { -3.0 / 140, -27.0 / 560, 27.0 / 70, 33.0 / 560 },
            { 19.0 / 1680, -3.0 / 140, 33.0 / 560, 8.0 / 105 }
        };
    }

    public class TriangleLinearLagrange : ITwoDimBasis
    {
        public int Size => 3;
        public BasisType Type => BasisType.Linear;
        public Func<double, double, double>[] GetFuncs()
        {
            return new Func<double, double, double>[3]
            {
                 (double ksi, double etta) => etta,
                 (double ksi, double etta) => 1.0 - ksi - etta,
                 (double ksi, double etta) => ksi,
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

    // TODO: Implement quadratic and cubic lagrange basis
}
