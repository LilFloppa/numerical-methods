using FEM;
using SlaeSolver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiDimInverseProblem
{
    public class Problem
	{
		public class ProblemInfo
		{
			public Source[] Sources;
			public Receiver[] Receivers;
			public double[] CurrentV;
			public double[] RealV;
			public double[] PivotI;
		}

		public static FEMrz FEM(SolverTypes solverType, double I, double sigma1, double sigma2, double h, double eps)
		{
			Dictionary<int, Material> materials = new Dictionary<int, Material>()
			{
				{ 0, new Material(sigma1) },
				{ 1, new Material(sigma2) }
			};

			Dictionary<AreaSide, (ConditionType, Func<double, double, double>)> Conditions = new Dictionary<AreaSide, (ConditionType, Func<double, double, double>)>()
			{
				{ AreaSide.TopFirst, (ConditionType.Second, (double r, double z) => 0) },
				{ AreaSide.Bottom,   (ConditionType.First, (double r, double z) => 0) },
				{ AreaSide.Right,    (ConditionType.First, (double r, double z) => 0) }
			};

			AreaInfoWithoutDelta areainfo = new AreaInfoWithoutDelta();
			areainfo.R0 = 0.0;
			areainfo.Z0 = 0.0;
			areainfo.Width = 10000;
			areainfo.FirstLayerHeight = h;
			areainfo.SecondLayerHeight = 10000 - h;

			areainfo.HorizontalStartStep = 0.2;
			areainfo.HorizontalCoefficient = 1.1;
			areainfo.VerticalStartStep = 0.2;
			areainfo.VerticalCoefficient = 1.1;
			areainfo.Materials = materials;
			areainfo.Conditions = Conditions;

			areainfo.SplitPoint = 0.5;

			GridBuilderWithoutDelta gb = new GridBuilderWithoutDelta(areainfo);
			gb.Build();

			foreach (Edge edge in gb.SB.Edges)
				edge.Function = (double r, double z) => I / (Math.PI * gb.ClosestSplitPoint * gb.ClosestSplitPoint);

			FEMProblemInfo info = new FEMProblemInfo();
			info.Points = gb.Points.ToArray();
			info.Materials = materials;
			info.Mesh = gb.Grid;
			info.FB = gb.FB;
			info.SB = gb.SB;
			info.F = (double r, double z) => 0.0;
			info.SolverType = solverType;

			FEMrz fem = new FEMrz(info);
			fem.Solver.Eps = eps;
			fem.Solve();

			return fem;
		}

		public static double[,] FEMDerivativeI(ProblemInfo info)
		{
			int m = info.Sources.Length;
			int n = info.Receivers.Length;

			double[,] dirs = new double[m, n];

			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					var s = info.Sources[i];
					var r = info.Receivers[j];

					//double V1 = s.GetValue(r);
					//double I = s.I;
					//s.I = I + 0.05 * I;
					//double V2 = s.GetValue(r);
					//s.I = I;

					double I = s.I;
					s.I = 1.0;
					dirs[i, j] = s.GetValue(r);
					s.I = I;
				}
			}
			return dirs;
		}

		public static double[] DirectProblem(ProblemInfo info)
        {
			int n = info.Receivers.Length;
			int m = info.Sources.Length;

			double[] result = new double[n];

			for (int i = 0; i < n; i++)
				for (int k = 0; k < m; k++)
                {
					result[i] += info.Sources[k].GetValue(info.Receivers[i]);
                }

			return result;
        }

		public static (double, bool) InverseProblemStep(ProblemInfo info, double J)
		{
			double[,] dirs = FEMDerivativeI(info);

			int n = info.Receivers.Length;
			int m = info.Sources.Length;
			double[,] A = new double[m, m];
			double[] B = new double[m];

			// Build Matrix
			for (int i = 0; i < m; i++)
				for (int j = 0; j < m; j++)
					for (int k = 0; k < n; k++)
					{
						double w = 1.0 / info.RealV[k];
						A[i, j] += w * w * dirs[i, k] * dirs[j, k];
					}

			// Build right
			for (int i = 0; i < m; i++)
				for (int k = 0; k < n; k++)
                {
					double w = 1.0 / info.RealV[k];
					B[i] -= w * w * dirs[i, k] * (info.CurrentV[k] - info.RealV[k]);
                }

			double alpha = GetAlpha(info);
			alpha = 0.1 * alpha;
            for (int i = 0; i < m; i++)
            {
				A[i, i] += alpha;
				B[i] -= alpha * (info.Sources[i].I - info.PivotI[i]);
			}

            // Solve SLAE
            double[] dI = Gauss.Solve(A, B);

			// Try make step
			double b = 1.0;
			double[] I0 = info.Sources.Select(s => s.I).ToArray();

			for (int i = 0; i < m; i++)
				info.Sources[i].I = I0[i] + b * dI[i];

			while (b > 1.0e-5 && Functional(info, info.RealV) >= J)
            {
				b /= 2;
				for (int i = 0; i < m; i++)
					info.Sources[i].I = I0[i] + b * dI[i];
			}

			if (b <= 1.0e-5)
            {
				for (int i = 0; i < m; i++)
					info.Sources[i].I = I0[i];

				return (J, false);
            }
            else
            {
				info.CurrentV = DirectProblem(info);
				J = Functional(info, info.RealV);

				return (J, true);
			}		
		}

		public static double Functional(ProblemInfo info, double[] realV)
		{
			double result = 0.0;
			double[] currentV = DirectProblem(info);
			int N = currentV.Length;

			for (int i = 0; i < N; i++)
			{
				double w = 1.0 / realV[i];
				result += w * (currentV[i] - realV[i]) * w * (currentV[i] - realV[i]);
			}

			return result;
		}

		private static double FunctionalI(double[] I0, double[] pivotI)
        {
			double result = 0.0;
			int M = I0.Length;

			for (int i = 0; i < M; i++)
				result += (I0[i] - pivotI[i]) * (I0[i] - pivotI[i]);

			return result;
		}

		public static double GetAlpha(ProblemInfo info)
        {
			double[] I0 = info.Sources.Select(s => s.I).ToArray();

			double Ji = FunctionalI(I0, info.PivotI);
			double Ju = Functional(info, info.RealV);

			if (Math.Abs(Ji) < 1.0e-5)
			{
				I0 = I0.Select(I => I + 0.5 * I).ToArray();
				Ji = FunctionalI(I0, info.PivotI);
			}

			double gamma = 1.0e-3;
			double right = (1 + gamma) * Ju;
			double alpha = (right - Ju) / Ji;

			return alpha;
        }
	}
}
