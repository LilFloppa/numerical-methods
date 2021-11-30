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
        public int EdgeNodeCount { get; }
        public BasisType Type { get; }
        public IEnumerable<Func<double, double, double>> GetFuncs();
        public Dictionary<string, IEnumerable<Func<double, double, double>>> GetDers();
    }

    public class LinearLagrange : IBasis
    {
        public int Size => 3;
        public int EdgeNodeCount => 2;
        public BasisType Type => BasisType.Linear;
        public IEnumerable<Func<double, double, double>> GetFuncs()
        {
            return new Func<double, double, double>[3]
            {
                 (double ksi, double etta) => ksi,
                 (double ksi, double etta) => etta,
                 (double ksi, double etta) => 1.0 - ksi - etta
            };
        }
        public Dictionary<string, IEnumerable<Func<double, double, double>>> GetDers()
        {
            return new Dictionary<string, IEnumerable<Func<double, double, double>>>
            {
                ["ksi"] = new Func<double, double, double>[3]
                {
                    (double ksi, double etta) => 1.0,
                    (double ksi, double etta) => 0.0,
                    (double ksi, double etta) => -1.0
                },

                ["etta"] = new Func<double, double, double>[3]
                {
                    (double ksi, double etta) => 0.0,
                    (double ksi, double etta) => 1.0,
                    (double ksi, double etta) => -1.0
                }
            };
        }
    }

    // TODO: Implement quadratic and cubic lagrange basis
}
