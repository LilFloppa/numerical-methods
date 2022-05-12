using System;

namespace OrderHarmonization
{
    public class Material
    {
        public string Name { get; set; }
        public double Lambda { get; set; }
        public double RoCp { get; set; }
        public Func<double, double, double> F { get; set; }
    }
}
