using HermiteSpline.Meshes;
using HermiteSpline.SLAE;
using MathUtilities;
using SlaeSolver;
using System;

namespace HermiteSpline.Spline
{
    public class HermiteSplineBuilder
    {
        public  Mesh Mesh { get; set; }

        private (Point p, double f)[] data;
        public void SetData((Point p, double f)[] data)
        {
            this.data = data;

            Mesh.ClearData();

            for (int i = 0; i < data.Length; i++)
            {
                bool contains = false;
                foreach (var e in Mesh)
                {
                    if (e.Contains(data[i].p))
                    {
                        e.DataIndices.Add(i);
                        contains = true;
                        break;
                    }
                }

                if (!contains)
                    throw new Exception("Some points do not belong to any mesh element");
            }
        }

        public HermiteSpline BuildSpline()
        {
            SLAEInfo info = new SLAEInfo { Data = data, Mesh = Mesh };
            SLAEBuilder builder = new SLAEBuilder(info);
            IMatrix A = new FullSparseMatrix(Mesh.NodeCount * 4);
            double[] b = new double[Mesh.NodeCount * 4];
            builder.Build(A, b);
            ISolver solver = new LOSLU();
            double[] q = solver.Solve(A, b);

            return new HermiteSpline(Mesh, q);
        }
    }
}
