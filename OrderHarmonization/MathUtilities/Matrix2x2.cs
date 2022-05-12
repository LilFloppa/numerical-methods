using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathUtilities
{
    public struct Matrix2x2
    {
        private double[,] A;

        public double a11 => A[0, 0];
        public double a12 => A[0, 1];
        public double a21 => A[1, 0];
        public double a22 => A[1, 1];
        public Matrix2x2(double a11, double a12, double a21, double a22)
        {
            A = new double[2, 2];
            A[0, 0] = a11;
            A[0, 1] = a12;
            A[1, 0] = a21;
            A[1, 1] = a22;
        }

        public static Matrix2x2 operator*(Matrix2x2 A, Matrix2x2 B) => new Matrix2x2(
                A.a11 * B.a11 + A.a12 * B.a21,
                A.a11 * B.a12 + A.a12 * B.a22,
                A.a21 * B.a11 + A.a22 * B.a21,
                A.a21 * B.a12 + A.a22 * B.a22);

        public static double[] operator *(Matrix2x2 A, double[] v)
        {
            if (v.Length != 2) throw new Exception("Invalid vector length");
            return new double[2] { A.a11 * v[0] + A.a12 * v[1], A.a21 * v[0] + A.a22 * v[1] };
        }

        public static double[] operator *(double[] v, Matrix2x2 A)
        {
            if (v.Length != 2) throw new Exception("Invalid vector length");
            return new double[2] { v[0] * A.a11 + v[1] * A.a21, v[0] * A.a12 + v[1] * A.a22 };
        }

        public Matrix2x2 Inverse()
        {
            double det = Det();
            return new Matrix2x2(a22 / det, -a12 / det, -a21 / det, a11 / det);
        }

        public Matrix2x2 Transpose() => new Matrix2x2(a11, a21, a12, a22);

        public double Det() => a11 * a22 - a12 * a21;
    }
}
