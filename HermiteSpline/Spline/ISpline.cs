using HermiteSpline.Meshes;
using MathUtilities;
using System;

namespace HermiteSpline.Spline
{
    public interface ISpline
    {
        double GetValue(Point p);
    }

    public class HermiteSpline : ISpline
    {
        private Mesh mesh;
        private double[] q;

        public HermiteSpline(Mesh mesh, double[] q)
        {
            this.mesh = mesh;
            this.q = q;
        }

        public double GetValue(Point p)
        {
            Element e = mesh.FindContainingElement(p);
            double hx = e.X2 - e.X1;
            double hy = e.Y2 - e.Y1;

            double result = 0;

            double ksi = (p.X - e.X1) / hx;
            double etta = (p.Y - e.Y1) / hy;

            for (int i = 0; i < HermiteBasis.Size; i++)
            {
                int index = e.Indices[i];
                Func<double, double, double> psi = HermiteBasis.GetPsi(i, hx, hy);

                result += q[index] * psi(ksi, etta);
            }

            return result;
        }
    }
}
