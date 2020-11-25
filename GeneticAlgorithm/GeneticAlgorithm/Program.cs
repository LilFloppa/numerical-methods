using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
	class Program
	{
		static void Main(string[] args)
		{
			double[] TrueGenes = new double[11] { -2, 0.5, 0, 0, 1, 0.8, -1.5, -2, 4, -0.25, 0.8 };

			List<double> Points = new List<double>();
			double h = 2.0 / 10;
			for (int i = 0; i < 10; i++)
				Points.Add(-1.0 + i * h);
			
			double[] values = Polynom.PolynomValues(Points.ToArray(), TrueGenes);

			GeneticAlgorithmInfo info = new GeneticAlgorithmInfo();
			info.PopulationCount = 1000;
			info.GenesCount = 11;
			info.MaxIterCount = 10000;
			info.Eps = 0.0001;
			info.minGeneValue = -2.0;
			info.maxGeneValue = 4.0;
			info.MaxParentCount = 5;
			info.MutationProbability = 0.5;
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
