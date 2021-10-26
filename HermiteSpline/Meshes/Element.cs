using MathUtilities;
using System.Collections.Generic;

namespace HermiteSpline.Meshes
{
    public class Element
    {
        public List<int> DataIndices { get; set; } = new List<int>();
        public int[] Indices { get; set; } = new int[16];

        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }


        public bool Contains(Point point)
        {
            double X = point.X;
            double Y = point.Y;

            if (X > X1 || Double.IsEqual(X, X1))
                if (X < X2 || Double.IsEqual(X, X2))
                    if (Y > Y1 || Double.IsEqual(Y, Y1))
                        if (Y < Y2 || Double.IsEqual(Y, Y2))
                            return true;

            return false;
        }
    }
}
