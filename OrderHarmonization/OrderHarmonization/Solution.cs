using MathUtilities;
using OrderHarmonization.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderHarmonization
{
    public class Solution
    {
        private double[] q;
        private Mesh mesh;

        public Solution(double[] q, Mesh mesh)
        {
            this.q = q;
            this.mesh = mesh;
        }

        public double GetValue(Point p)
        {
            foreach (var e in mesh.Elements)
            {
                Point a = mesh.Points[e[0]];
                Point b = mesh.Points[e[1]];
                Point c = mesh.Points[e[2]];

                if (Utilities.PointInTriangle(a, b, c, p))
                {
                    double[] L = Utilities.GetL(a, b, c, p);
                    double result = 0;
                    for (int i = 0; i < LInfo.LBasis.Length; i++)
                        result += q[e[i]] * LInfo.LBasis[i](L[0], L[1], L[2]);

                    return result;
                }
            }

            throw new Exception();
        }

        public double GetValue(double x, double y)
        {
            Point p = new Point { X = x, Y = y };
            foreach (var e in mesh.Elements)
            {
                Point a = mesh.Points[e[0]];
                Point b = mesh.Points[e[1]];
                Point c = mesh.Points[e[2]];

                if (Utilities.PointInTriangle(a, b, c, p))
                {
                    double[] L = Utilities.GetL(a, b, c, p);
                    double result = 0;
                    for (int i = 0; i < LInfo.LBasis.Length; i++)
                        result += q[e[i]] * LInfo.LBasis[i](L[0], L[1], L[2]);

                    return result;
                }
            }

            throw new Exception();
        }
    }
}
