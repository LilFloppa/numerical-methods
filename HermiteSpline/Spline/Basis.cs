using System;

namespace HermiteSpline.Spline
{
    public class HermiteBasis
    {
        public static int Size = 16;

        private static Func<double, double, double>[] Psi = new Func<double, double, double>[]
        {
            (double x, double h) => 1 - 3 * x * x + 2 * x * x * x,
            (double x, double h) => h * (x - 2 * x * x + x * x * x),
            (double x, double h) => 3 * x * x - 2 * x * x * x,
            (double x, double h) => h * (-x * x + x * x * x)
        };

        private static Func<double, double, double>[] D2psi = new Func<double, double, double>[]
        {
            (double x, double h) => -6 + 12 * x,
            (double x, double h) => h * (-4 + 6 * x),
            (double x, double h) => 6 - 12 * x,
            (double x, double h) => h * (-2 + 6 * x)
        };

        private static Func<int, int> XIndex => (int i) =>
        {
            if (i < 1 || i > 16)
                throw new Exception("Wrong sequence number. Must be i >= 1 or i <= 16.");
            return 2 * ((i - 1) / 4 % 2) + ((i - 1) % 2);
        };
        private static Func<int, int> YIndex => (int i) =>
        {
            if (i < 1 || i > 16)
                throw new Exception("Wrong sequence number. Must be i >= 1 or i <= 16.");
            return 2 * ((i - 1) / 8) + ((i - 1) / 2 % 2);
        };

        public static Func<double, double, double> GetPsi(int i, double hx, double hy) =>
            (double x, double y) => Psi[XIndex(i + 1)] (x, hx) * Psi[YIndex(i + 1)] (y, hy);
    }
}
