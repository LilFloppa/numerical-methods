﻿using SlaeSolver;
using FEM;
using System.Collections.Generic;
using System;
using MathUtilities;

namespace ExampleTest
{
	class Program
	{
		static void Main(string[] args)
		{

			double sigma = 0.1;

			Dictionary<int, Material> materials = new Dictionary<int, Material>()
			{
				{ 0, new Material(2 * Math.PI * sigma) },
				{ 1, new Material(2 * Math.PI * sigma) }
			};

			Dictionary<AreaSide, (ConditionType, Func<double, double, double>)> Conditions = new Dictionary<AreaSide, (ConditionType, Func<double, double, double>)>()
			{
				{ AreaSide.Top,			(ConditionType.SecondNull, (double r, double z) => 0) },
				{ AreaSide.Left,			(ConditionType.SecondNull, (double r, double z) => r * r) },
				{ AreaSide.Bottom,		(ConditionType.SecondNull, (double r, double z) => 0) },
				{ AreaSide.Right,			(ConditionType.SecondNull, (double r, double z) => 0) },
				{ AreaSide.TopRight,		(ConditionType.First, (double r, double z) => 1.0 / (2 * Math.PI * sigma * Math.Sqrt(z * z + r * r))) },
				{ AreaSide.TopBottom,	(ConditionType.First, (double r, double z) => 1.0 / (2 * Math.PI * sigma * Math.Sqrt(z * z + r * r))) }
			};
			
			AreaInfo areainfo = new AreaInfo();
			areainfo.R0 = 0.0;
			areainfo.Z0 = 0.0;
			areainfo.Width = 3.0;
			areainfo.FirstLayerHeight = 2.0;
			areainfo.SecondLayerHeight = 30;

			areainfo.HorizontalNodeCount = 7;
			areainfo.FirstLayerVerticalNodeCount = 5;
			areainfo.SecondLayerVerticalNodeCount = 2;
			areainfo.Materials = materials;
			areainfo.Conditions = Conditions;

			GridBuilder2 gb = new GridBuilder2(areainfo);
			gb.Build();

			Func<double, double, double> F = (double r, double z) => 0.0;

			ProblemInfo info = new ProblemInfo();
			info.Points = gb.Points.ToArray();
			info.Materials = materials;
			info.Mesh = gb.Grid;
			info.FB = gb.FB;
			info.F = F;
			info.SolverType = SolverTypes.LOSLU;
			FEMrz fem = new FEMrz(info);
			fem.Solve();

			Console.WriteLine(fem.Info.Points[33].R);
			Console.WriteLine(fem.Info.Points[33].Z);
			Console.WriteLine(fem.Weights[33]);

			ProblemInfoDelta infoDelta = new ProblemInfoDelta();
			infoDelta.Points = gb.Points.ToArray();
			infoDelta.Materials = materials;
			infoDelta.Mesh = gb.Grid;
			infoDelta.FB = gb.FB;
			infoDelta.R0 = 0.0;
			infoDelta.Z0 = 0.0;
			infoDelta.DeltaCoefficient = 1.0;
			infoDelta.SolverType = SolverTypes.LOSLU;

			FEMrzDelta femDelta = new FEMrzDelta(infoDelta);
			femDelta.Solve();

			Console.WriteLine(femDelta.Info.Points[27].R);
			Console.WriteLine(femDelta.Info.Points[27].Z);
			Console.WriteLine(femDelta.Weights[27]);

			//Vector3 A = new Vector3(0.0f, 0.0f, 0.0f);
			//Vector3 B = new Vector3(100.0f, 0.0f, 0.0f);
			//double I = 1.0;

			//Source source = new Source(A, B, I);

			//Vector3 M1 = new Vector3(200.0f, 0.0f, 0.0f);
			//Vector3 N1 = new Vector3(300.0f, 0.0f, 0.0f);

			//Vector3 M2 = new Vector3(500.0f, 0.0f, 0.0f);
			//Vector3 N2 = new Vector3(600.0f, 0.0f, 0.0f);

			//Vector3 M3 = new Vector3(1000.0f, 0.0f, 0.0f);
			//Vector3 N3 = new Vector3(1100.0f, 0.0f, 0.0f);

			//List<Receiver> receivers = new List<Receiver>() { new Receiver(M1, N1), new Receiver(M2, N2), new Receiver(M3, N3) };

			//Problem.ProblemInfo info = new Problem.ProblemInfo();

			//double trueP = 0.1;
			//info.TrueV = Problem.DirectProblem(source, receivers, trueP);

			//double initialP = 0.01;
			//info.V = Problem.DirectProblem(source, receivers, initialP);

			//info.p = new double[1] { initialP };
			//info.Source = source;
			//info.Receivers = receivers;

			//while (Problem.Functional(info.V, info.TrueV) > 1.0e-7)
			//{
			//	double[] dp = Problem.InverseProblem(info);
			//	for (int i = 0; i < dp.Length; i++)
			//		info.p[i] += dp[i];

			//	info.V = Problem.DirectProblem(source, receivers, info.p[0]);
			//}
		}
	}
}