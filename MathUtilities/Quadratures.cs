using System;

namespace MathUtilities
{
    public class Quadratures
    {
        public static double Gauss7(Func<double, double, double> f, double x0, double x1, double y0, double y1)
        {
            double result = 0;

            double D = (x1 - x0) * (y1 - y0) / 4;

            double a = 0.3805544332083156;
            double b = 0.8059797829185988;
            double c = 0.9258200997725514;

            double wa = 0.5205929166673944;
            double wb = 0.2374317746906302;
            double wc = 0.2419753086419753;

            Func<double, double, double> x = (double ksi, double eta) => (x1 - x0) * (ksi + 1) / 2 + x0;
            Func<double, double, double> y = (double ksi, double eta) => (y1 - y0) * (eta + 1) / 2 + y0;

            result +=
                f(x(-c, 0), y(-c, 0)) * wc + f(x(c, 0), y(c, 0)) * wc + f(x(0, -c), y(0, -c)) * wc + f(x(0, c), y(0, c)) * wc +
                f(x(-a, -a), y(-a, -a)) * wa + f(x(a, -a), y(a, -a)) * wa + f(x(-a, a), y(-a, a)) * wa + f(x(a, a), y(a, a)) * wa +
                f(x(-b, -b), y(-b, -b)) * wb + f(x(b, -b), y(b, -b)) * wb + f(x(-b, b), y(-b, b)) * wb + f(x(b, b), y(b, b)) * wb;

            return result * D;
        }
    }
}
