using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public interface IBasis
    {
        public int Size { get; set; }
        public IEnumerable<Func<double, double, double>> GetFuncs();

        public IEnumerable<Func<double, double, double>> GetDers();
    }

    // TODO: Implement Linear Lagrange basis
}
