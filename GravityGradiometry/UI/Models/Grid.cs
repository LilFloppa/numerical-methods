using GravityGradiometry;

namespace UI.Models
{
    public class Grid
    {
        public double[] Properties { get; set; }
        public Point[] Receivers { get; set; }   
        public double[] X { get; set; }
        public double[] Z { get; set; }

        public Grid(double[] x, double[] z, double[] properties, Point[] receivers)
        {
            X = x;
            Z = z;
            Properties = properties;
            Receivers = receivers;
        }
    }
}
