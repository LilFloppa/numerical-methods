using System;
using System.IO;

namespace GeneticAlgorithm
{
	class Program
	{
		static void WriteCSV(string filename, GeneticAlgorithm algorithm, double[] trueGenes)
		{
			using (StreamWriter stream = new StreamWriter(filename))
			{
				stream.Write($"Вероятность мутации; {algorithm.Info.MutationProbability}\n");
				stream.Write($"Численность популяции; {algorithm.Info.PopulationCount}\n");
				stream.Write($"Количество генов; {algorithm.Info.GenesCount}\n");
				//stream.Write($"Лучшее значение функционала; {algorithm.BestFuctionalValues[algorithm.BestFuctionalValues.Count - 1]}\n");

				for (int i = 0; i < algorithm.Info.GenesCount; i++)
					stream.Write($"{trueGenes[i]};");

				stream.Write("\n");

				for (int i = 0; i < algorithm.Info.GenesCount; i++)
					stream.Write($"{algorithm.BestGenotype[i]};");

				stream.Write("\n");

				int iter = 0;
				foreach (double value in algorithm.BestFuctionalValues)
				{
					stream.Write($"{iter};{value}\n");
					iter++;
				}
			}
		}

		static void Main(string[] args)
		{
			GeneticAlgorithmInfo info = new GeneticAlgorithmInfo();
			info.PopulationCount = 1000;
			info.GenesCount = 50;
			info.MaxIterCount = 5000;
			info.Eps = 1.0e-15;
			info.minGeneValue = -10.0;
			info.maxGeneValue = 10.0;
			info.MaxParentCount = 20;
			info.MutationProbability = 0.3;

			GeneticAlgorithm algorithm = new GeneticAlgorithm(info);
			algorithm.Solve();

			Console.WriteLine();
			

			double[] trueGenes = new double[info.GenesCount];
			for (int i = 0, k = 0; i < info.GenesCount; i++, k++)
			{
				if (k == Constants.Offsets.Length)
					k = 0;
				trueGenes[i] = Constants.Offsets[k];
			}


			WriteCSV("result1.csv", algorithm, trueGenes);
			Console.WriteLine();
		}
	}	
}
