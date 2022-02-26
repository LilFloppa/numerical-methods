using System.Collections.Generic;

namespace GravityGradiometry
{
    class QuadratureNode
    {
        public QuadratureNode(Point node, double weight)
        {
            Node = node;
            Weight = weight;
        }
        public Point Node { get; }
        public double Weight { get; }
    }

    static class Quadratures
    {
        public static IEnumerable<QuadratureNode> RectangleOrder3()
        {
            double a = 1.7320508075688772;
            double[] p1 = { -1.0 / a, -1.0 / a, 1.0 / a, 1.0 / a };
            double[] p2 = { -1.0 / a, 1.0 / a, -1.0 / a, 1.0 / a };

            int count = 4;

            for (int i = 0; i < count; i++)
                yield return new QuadratureNode(new Point(p1[i], p2[i]), 1.0);
        }
    }
}
