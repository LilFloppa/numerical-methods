using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithm
{
	class Polynom
	{
		static public double PolynomValue(double point, double[] coeffs)
		{
			double result = 0.0;
			for (int i = 0; i < coeffs.Length; i++)
				result = result * point + coeffs[i];

			return result;
		}

		static public double[] PolynomValues(int n, double min, double max, double[] coeffs)
		{
			double[] values = new double[n];

			double h = (max - min) / n;

			for (int i = 0; i < n; i++)
			{
				double point = min + h * i;
				values[i] = PolynomValue(point, coeffs);
			}

			return values;
		}

	}
}
