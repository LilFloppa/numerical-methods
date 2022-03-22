using GravityGradiometry;

namespace UI.Models
{
    public class Problem
    {
        public double[] X { get; set; } = null;
        public double[] Z { get; set; } = null;
        public Point[] Receivers { get; set; } = null;
    }
}
