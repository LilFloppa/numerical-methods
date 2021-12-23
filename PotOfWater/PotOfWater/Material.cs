using System;

namespace PotOfWater
{
    public class Material
    {
        public string Name { get; set; }
        public double Lambda { get; set; }
        public double RoCp { get; set; }
        public (double, double ) V { get; set; }
        public Func<double, double, double, double> F { get; set; }
    }
}
