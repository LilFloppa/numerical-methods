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
                 (double x) => x,
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
