using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
	class Program
	{
		static void Main(string[] args)
		{
			double[] TrueGenes = new double[11] { -2.0, 0.5, 0.0, 0.0, 1.0, 0.8, -1.5, -2.0, 4.0, -0.25, 0.8  };

			List<double> Points = new List<double>();
			double h = 20.0 / 50;
			for (int i = 0; i < 50; i++)
				Points.Add(-10.0 + i * h);

			for (int i = 0; i < 50; i++)
				Points.Add(100 + i * 5);
			
			double[] values = Polynom.PolynomValues(Points.ToArray(), TrueGenes);

			GeneticAlgorithmInfo info = new GeneticAlgorithmInfo();
			info.PopulationCount = 1000;
			info.GenesCount = 11;
			info.MaxIterCount = 10000;
			info.Eps = 0.001;
			info.minGeneValue = -3.0;
			info.maxGeneValue = 5.0;
			info.MaxParentCount = 5;
			info.MutationProbability = 0.3;
			info.Points = Points.ToArray();
			info.TrueValues = values;

			GeneticAlgorithm algorithm = new GeneticAlgorithm(info);
			algorithm.Solve();

			Console.WriteLine();

			for (int i = 0; i < info.GenesCount; i++)
				Console.WriteLine($"{algorithm.BestGenotype[i]} - {TrueGenes[i]}");

			Console.WriteLine();
		}
	}

	
}
