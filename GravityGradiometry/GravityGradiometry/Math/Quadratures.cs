using System;
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

        public static IEnumerable<QuadratureNode> RectangleOrder7()
        {
            double v1 = 3.0 * Math.Sqrt(583.0);

            double a = Math.Sqrt((114.0 - v1) / 287.0);
            double b = Math.Sqrt((114.0 + v1) / 287.0);
            double c = Math.Sqrt(6.0 / 7.0);

            double v2 = 923.0 / (270.0 * Math.Sqrt(583.0));

            double wa = 307.0 / 810.0 + v2;
            double wb = 307.0 / 810.0 - v2;
            double wc = 98.0 / 405.0;

            double[] p1 = { -c, c, 0.0, 0.0, -a, a, -a, a, -b, b, -b, b };
            double[] p2 = { 0.0, 0.0, -c, c, -a, -a, a, a, -b, -b, b, b };
            double[] w = { wc, wc, wc, wc, wa, wa, wa, wa, wb, wb, wb, wb };

            int count = 12;

            for (int i = 0; i < count; i++)
                yield return new QuadratureNode(new Point(p1[i], p2[i]), w[i]);
        }
    }
}
