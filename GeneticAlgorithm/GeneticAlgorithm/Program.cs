using System;

namespace GeneticAlgorithm
{
	class Program
	{
		static void Main(string[] args)
		{
			double[] TrueGenes = new double[11] { -2.0, 0.5, 0.0, 0.0, 1.0, 0.8, -1.5, -2.0, 4.0, -0.25, 0.8  };


			GeneticAlgorithmInfo info = new GeneticAlgorithmInfo();
			info.PopulationCount = 1000;
			info.GenesCount = 11;
			info.MaxIterCount = 10000;
			info.Eps = 0.001;
			info.minGeneValue = -3.0;
			info.maxGeneValue = 4.0;
			info.MaxParentCount = 5;
			info.MutationProbability = 0.3;

			info.PointsCount = 100;
			info.MinPoint = -100;
			info.MaxPoint = 100;

			double[] values = Polynom.PolynomValues(info.PointsCount, info.MinPoint, info.MaxPoint, TrueGenes);
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
