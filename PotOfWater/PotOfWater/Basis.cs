using System;
using System.Collections.Generic;

namespace PotOfWater
{
    public interface IBasis
    {
        public int Size { get; set; }
        public IEnumerable<Func<double, double, double>> GetFuncs();
    }

    // TODO: Implement Lagrange basis
}
