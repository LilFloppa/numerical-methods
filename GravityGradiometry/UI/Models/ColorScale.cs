using System.Drawing;

namespace UI.Models
{
    public class ColorScale
    {
        public static Color CGEColor1 = Color.FromArgb(127, 85, 85, 255);
        public static Color CGEColor2 = Color.FromArgb(127, 120, 200, 255);
        public static Color CGEColor3 = Color.FromArgb(127, 150, 255, 255);
        public static Color CGEColor4 = Color.FromArgb(127, 150, 255, 150);
        public static Color CGEColor5 = Color.FromArgb(127, 255, 255, 120);
        public static Color CGEColor6 = Color.FromArgb(127, 255, 150, 100);
        public static Color CGEColor7 = Color.FromArgb(127, 255, 100, 100);

        public double[] Values { get; set; }

        private double minValue, maxValue;

        Color[] DefaultStdPalette = { CGEColor1, CGEColor2, CGEColor3, CGEColor4, CGEColor5, CGEColor6, CGEColor7 };

        public void SetValues(double[] values)
        {
            Values = values;
            minValue = values.Min();
            maxValue = values.Max();
        }

        public Color GetColor(double value)
        {
            if (value < minValue)
                return Color.Black;

            if (value > maxValue)
                return Color.White;;

            double alpha = 0.0;
            if (Math.Abs(maxValue - minValue) > 1.0e-9)
                alpha = (value - minValue) / (maxValue - minValue);

            int index = Math.Clamp((int)(alpha * DefaultStdPalette.Length), 0, DefaultStdPalette.Length - 1);
            return DefaultStdPalette[index];
        }

        private Color Interpolate(double alpha)
        {
            Color blue = Color.Blue;
            Color yellow = Color.Yellow;
            Color red = Color.Red;

            int A, R, G, B;

            if (alpha < 0.5)
            {
                R = (int)(yellow.R * alpha + (1.0 - alpha) * blue.R);
                G = (int)(yellow.G * alpha + (1.0 - alpha) * blue.G);
                B = (int)(yellow.B * alpha + (1.0 - alpha) * blue.B);
                A = (int)(255 * 0.5);
            }
            else
            {
                R = (int)(red.R * alpha + (1.0 - alpha) * yellow.R);
                G = (int)(red.G * alpha + (1.0 - alpha) * yellow.G);
                B = (int)(red.B * alpha + (1.0 - alpha) * yellow.B);
                A = (int)(255 * 0.5);
            }

            return Color.FromArgb(A, R, G, B);
        }
    }
}
