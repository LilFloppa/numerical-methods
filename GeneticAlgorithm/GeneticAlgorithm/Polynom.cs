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

		static public double[] PolynomValues(double[] points, double[] coeffs)
		{
			double[] values = new double[points.Length];

			for (int i = 0; i < points.Length; i++)
				values[i] = PolynomValue(points[i], coeffs);

			return values;
		}
	}
}
