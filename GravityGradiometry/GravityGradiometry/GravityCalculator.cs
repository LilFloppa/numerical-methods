using System;
using System.Collections.Generic;

namespace GravityGradiometry
{
    class GravityCalculator
    {

        public double[] X { get; set; }
        public double[] Z { get; set; }
        public double[] P { get; set; }

        private IEnumerable<QuadratureNode> quadrature = Quadratures.RectangleOrder3();

        public GravityCalculator(double[] x, double[] z, double[] p)
        {
            X = x;
            Z = z;
            P = p;
        }

        public double CalculateFromAll(Point m)
        {
            double result = 0.0;
            for (int zi = 0; zi < Z.Length - 1; zi++)
            {
                for (int xi = 0; xi < X.Length - 1; xi++)
                {
                    double gk = 0.0;
                    int k = zi * (X.Length - 1) + xi;
                    double x1 = X[xi], x2 = X[xi + 1];
                    double z1 = Z[zi], z2 = Z[zi + 1];
                    double pk = P[k];
                    double mes = (x2 - x1) * (z2 - z1);

                    foreach (var node in quadrature)
                    {
                        Point gaussPoint = new Point(
                            x1 + (node.Node.X + 1) * (x2 - x1) / 2, 
                            z1 + (node.Node.Z + 1) * (z2 - z1) / 2);

                        double r = gaussPoint.Distance(m);
                        double z = m.Z - gaussPoint.Z;
                        gk += node.Weight * z / (r * r * r);
                    }

                    result += gk * mes * pk / (4 * Math.PI);
                }
            }

            return result;
        }

        public double CalculateFromOne(Point m, int k)
        {
            int zi = k / (X.Length - 1);
            int xi = k % (X.Length - 1);
            
            double gk = 0.0;
            double x1 = X[xi], x2 = X[xi + 1];
            double z1 = Z[zi], z2 = Z[zi + 1];
            double pk = P[k];
            double mes = (x2 - x1) * (z2 - z1);

            foreach (var node in quadrature)
            {
                Point gaussPoint = new Point(
                    x1 + (node.Node.X + 1) * (x2 - x1) / 2,
                    z1 + (node.Node.Z + 1) * (z2 - z1) / 2);

                double r = gaussPoint.Distance(m);
                double z = m.Z - gaussPoint.Z;
                gk += node.Weight * z / (r * r * r);
            }

            return gk * mes * pk / (4 * Math.PI);
        }
    }
}
