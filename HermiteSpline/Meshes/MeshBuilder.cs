using System;

namespace HermiteSpline.Meshes
{
    public class MeshOptions
    {
        public double X1 { get; set; }
        public double X2 { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public int XN { get; set; }
        public int YN { get; set; }
    }

    public class MeshBuilder
    {
        double[] x;
        double[] y;
        public Mesh Build(MeshOptions options)
        {
            int[] nodeIndices = new int[options.XN * options.YN];
            Array.Fill(nodeIndices, -1);

            double[] x = BuildIntervals(options.X1, options.X2, options.XN);
            double[] y = BuildIntervals(options.Y1, options.Y2, options.YN);

            Mesh mesh = new Mesh(nodeIndices.Length);

            for (int j = 0; j < y.Length - 1; j++)
                for (int i = 0; i < x.Length - 1; i++)
                {
                    Element e = new Element();
                    e.X1 = x[i];
                    e.X2 = x[i + 1];
                    e.Y1 = y[j];
                    e.Y2 = y[j + 1];

                    Func<int, int, int> index = (int p, int s) => (j + s) * options.XN + (i + p);

                    for (int s = 0; s < 2; s++)
                        for (int p = 0; p < 2; p++)
                        {
                            int nodeIndex = index(p, s);
                            if (nodeIndices[nodeIndex] == -1)
                                nodeIndices[nodeIndex] = nodeIndex * 4;

                            for (int k = 0; k < 4; k++)
                                e.Indices[4 * (s * 2 + p) + k] = nodeIndices[nodeIndex] + k;
                        }

                    mesh.Elements.Add(e);
                }

            return mesh;
        }

        private double[] BuildIntervals(double a, double b, int n)
        {
            if (b < a)
                throw new Exception("Arguments is incorrect. b must be greater than a");

            double[] values = new double[n];
            double h = (b - a) / (n - 1);

            for (int i = 0; i < n; i++)
                values[i] = a + h * i;

            return values;
        }
    }
}
