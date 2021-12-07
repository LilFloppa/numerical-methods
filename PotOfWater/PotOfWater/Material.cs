using System;

namespace PotOfWater
{
    public class Material
    {
        public double Lambda { get; set; }
        public double RoCp { get; set; }
        public Func<double, double, double, double> F { get; set; }
    }
}
