using System;
using System.Collections.Generic;

namespace GeneticAlgorithm
{
	struct GeneticAlgorithmInfo
	{ 
		public int PopulationCount { get; set; }
		public int GenesCount { get; set; }

		public int MaxIterCount { get; set; }
		public double Eps { get; set; }

		public double minGeneValue { get; set; }
		public double maxGeneValue { get; set; }

		public double MutationProbability { get; set; }
		public double MaxParentCount { get; set; }
	}

	class GeneticAlgorithm
	{
		public GeneticAlgorithmInfo Info { get; set; }

		Random Random = new Random();
		Population PreviousPopulation = null;
		Population CurrentPopulation = null;

		public List<double> BestFuctionalValues { get; set; } = new List<double>();
		public int Iteration { get; set; } = 0;

		public double[] BestGenotype { get; set; } = null;
		public List<double> FunctionalValues { get; set; } = new List<double>();

		public GeneticAlgorithm(GeneticAlgorithmInfo info) => Info = info;

		public void Solve()
		{
			PreviousPopulation = CreateInitialPopulation();
			PreviousPopulation.SortIndividuals();
			BestFuctionalValues.Add(PreviousPopulation.Individuals[0].FunctionalValue);

			double bestFunctionalValue = BestFuctionalValues[0];
			FunctionalValues.Add(bestFunctionalValue);
			Iteration = 0;
			while (Iteration < Info.MaxIterCount && bestFunctionalValue >= Info.Eps)
			{
				CurrentPopulation = CreateNewPopulation();
				CurrentPopulation.SortIndividuals();
				BestFuctionalValues.Add(CurrentPopulation.Individuals[0].FunctionalValue);
				bestFunctionalValue = CurrentPopulation.Individuals[0].FunctionalValue;
				FunctionalValues.Add(bestFunctionalValue);

				PreviousPopulation = Selection();
				CurrentPopulation = null;
				Iteration++;

				Console.WriteLine($"{bestFunctionalValue, 20}\t\t {Iteration}");
			}

			BestGenotype = PreviousPopulation.Individuals[0].Genes;
		}

		Population CreateInitialPopulation()
		{	
			Population population = new Population(Info.PopulationCount);

			for (int i = 0; i < Info.PopulationCount; i++)
			{
				Individual individual = new Individual(Info.GenesCount);

				for (int j = 0; j < Info.GenesCount; j++)
					individual.Genes[j] = Info.minGeneValue + Random.NextDouble() * (Info.maxGeneValue - Info.minGeneValue);

				individual.CaluclatePhenotype();
				SetFunctionalValue(individual);
				population.Individuals[i] = individual;
			}

			return population;
		}

		Population CreateNewPopulation()
		{
			Population population = new Population(Info.PopulationCount);

			int n = 0;
			for (int i = 0; i < Info.MaxParentCount; i++)
			{
				for (int j = 0; j < Info.PopulationCount / Info.MaxParentCount; j++)
				{
					Individual father = PreviousPopulation.Individuals[i];
					Individual mother = PreviousPopulation.GetRandomIndividual(Random);

					Individual child = CreateChild(father, mother);

					if (BestFuctionalValues[BestFuctionalValues.Count - 1] >= 1.0e-5)
						Mutation2(child);
					else
						Mutation1(child);

					child.CaluclatePhenotype();
					SetFunctionalValue(child);
					population.Individuals[n] = child;
					n++;
				}
			}

			return population;
		}

		Individual CreateChild(Individual mother, Individual father)
		{
			Individual child = new Individual(Info.GenesCount);

			int IndCross = Random.Next(Info.GenesCount);
			for (int i = 0; i < IndCross; i++)
				child.Genes[i] = father.Genes[i];

			for (int i = IndCross; i < Info.GenesCount; i++)
				child.Genes[i] = mother.Genes[i];

			return child;		
		}

		void Mutation1(Individual individual)
		{
			double p = Random.NextDouble();
			if (p <= Info.MutationProbability)
			{
				int i = Random.Next(Info.GenesCount);
				double p1 = Random.NextDouble();
				double p2 = Random.NextDouble() * 0.1;

				if (p1 >= 0.5)
					individual.Genes[i] *= 1.0 + p2;
				else
					individual.Genes[i] *= 1.0 - p2;
			}
		}

		void Mutation2(Individual individual)
		{
			double p = Random.NextDouble();
			if (p <= Info.MutationProbability)
			{
				int i = Random.Next(Info.GenesCount);
				individual.Genes[i] = Info.minGeneValue + Random.NextDouble() * (Info.maxGeneValue - Info.minGeneValue);
			}
		}

		void CrossingOver(Individual i1, Individual i2)
		{
			int i = Random.Next(Info.GenesCount / 2);

			for (int j = 0; j < i; j++)
				(i1.Genes[j], i2.Genes[j]) = (i2.Genes[j], i1.Genes[j]);
		}

		Population Selection()
		{
			Population population = new Population(Info.PopulationCount);

			int k = 0;
			int m = 0;
			for (int i = 0; i < Info.PopulationCount; i++)
			{
				double F1 = PreviousPopulation.Individuals[k].FunctionalValue;
				double F2 = CurrentPopulation.Individuals[m].FunctionalValue;
				if (F1 < F2)
				{
					population.Individuals[i] = PreviousPopulation.Individuals[k];
					k++;
				}
				else
				{
					population.Individuals[i] = CurrentPopulation.Individuals[m];
					m++;
				}
			}

			return population;
		}

		void SetFunctionalValue(Individual individual)
		{
			double result = 0.0;
			foreach (double value in individual.Phenotype)
				result += value * value;

			individual.FunctionalValue = Math.Sqrt(result);
		}
	}

	class Individual
	{
		public double[] Genes { get; set; }
		public double[] Phenotype { get; set; }
		public double FunctionalValue { get; set; }
		public Individual(int genesCount)
		{
			Genes = new double[genesCount];
			Phenotype = new double[genesCount];
		}

		public void CaluclatePhenotype()
		{
			for (int i = 0, k = 0; i < Genes.Length; i++, k++)
			{
				if (k == Constants.Offsets.Length) 
					k = 0;
				Phenotype[i] = (Genes[i] - Constants.Offsets[k]) * (Genes[i] - Constants.Offsets[k]);
			}
		}
	}

	class Population
	{
		public Individual[] Individuals = null;

		public Population(int populationCount) => Individuals = new Individual[populationCount];

		public void SortIndividuals() => Array.Sort(Individuals, (Individual x, Individual y) => x.FunctionalValue.CompareTo(y.FunctionalValue));

		public Individual GetRandomIndividual(Random random) => Individuals[random.Next(Individuals.Length)];

	}
}
