#include "Matrix.h"

void ReadMatrixSize(int& N, int& ALsize)
{
	std::ifstream in("input/size.txt");
	in >> N >> ALsize;
	in.close();
}

void ReadMatrix(Matrix& m)
{
	std::ifstream in("input/di.txt");
	for (int i = 0; i < m.N; i++)
		in >> m.DI[i];
	in.close();

	in.open("input/al.txt");
	for (int i = 0; i < m.ALSize; i++)
		in >> m.AL[i];
	in.close();

	in.open("input/au.txt");
	for (int i = 0; i < m.ALSize; i++)
		in >> m.AU[i];
	in.close();

	in.open("input/ia.txt");
	for (int i = 0; i < m.N + 1; i++)
	{
		in >> m.IA[i];
		m.IA[i]--;
	}
	in.close();
}

void LUDecomposition(Matrix& m)
{
	if (m.isDecomposed)
	{
		std::cout << "ERROR. Matrix is already decomposed." << std::endl;
		return;
	}

	for (int i = 0; i < m.N; i++)
	{
		realScal sumD = 0;
		int i0 = m.IA[i], i1 = m.IA[i + 1];
		int j = i - (i1 - i0);

		for (int k = i0; k < i1; k++, j++)
		{
			realScal sumL = 0, sumU = 0;

			// Calculation elements L[i][j] and U[j][i]
			int j0 = m.IA[j], j1 = m.IA[j + 1];
			int size_i = k - i0, size_j = j1 - j0;
			int diff = size_i - size_j;
			int kl = i0, ku = j0;

			(diff < 0) ? ku -= diff : kl += diff;

			for (; kl < k; kl++, ku++)
			{
				sumL += m.AL[kl] * m.AU[ku];
				sumU += m.AU[kl] * m.AL[ku];
			}

			m.AL[k] -= sumL;
			m.AU[k] -= sumU;
			m.AU[k] /= m.DI[j];

			// Accumulation of sum for DI[i]
			sumD += m.AL[k] * m.AU[k];
		}

		// Calculation DI[i]
		m.DI[i] -= sumD;
		sumD = 0;
	}

	m.isDecomposed = true;
}

void Solve(Matrix& m, real* B, real *res)
{
	if (!m.isDecomposed)
	{
		std::cout << "ERROR. Can't solve. Matrix is not decomposed." << std::endl;
		return;
	}

	real* y = B;

	for (int i = 0; i < m.N; i++)
	{
		realScal sumL = 0;
		int j = i - 1;

		for (int k = m.IA[i + 1] - 1; k >= m.IA[i]; k--, j--)
			sumL += m.AL[k] * y[j];

		y[i] = (B[i] - sumL) / m.DI[i];
	}

	for (int i = m.N - 1; i >= 0; i--)
	{
		int j = i - 1;
		res[i] = y[i];
		for (int k = m.IA[i + 1] - 1; k >= m.IA[i]; k--, j--)
			y[j] -= m.AU[k] * res[i];
	}
}

void Multiply(Matrix& m, real* vector, real* res)
{
	if (m.isDecomposed)
	{
		std::cout << "ERROR. Matrix is decomposed. Can't multiply." << std::endl;
		return;
	}

	for (int i = 0; i < m.N; i++)
		res[i] = vector[i] * m.DI[i];

	for (int i = 1; i < m.N; i++)
	{
		int j = i - (m.IA[i + 1] - m.IA[i]);

		for (int k = m.IA[i]; k < m.IA[i + 1]; k++)
		{
			res[i] += vector[j] * m.AL[k];
			res[j] += vector[i] * m.AU[k];
			j++;
		}
	}
}

Matrix HilbertMatrix(int size)
{
	Matrix m = { };
	int ALSize = size * (size - 1) / 2;
	m.N = size;
	m.ALSize = ALSize;
	m.DI = new real[size];
	m.AL = new real[ALSize];
	m.AU = new real[ALSize];
	m.IA = new int[size + 1];

	for (int i = 0; i < size; i++)
		m.DI[i] = 1.0 / (2 * i + 1);

	m.IA[0] = 0;
	for (int i = 0; i < size; i++)
		m.IA[i + 1] = m.IA[i] + i;

	for (int i = 1; i < size; i++)
		for (int j = 0; j < i; j++)
			m.AU[m.IA[i] + j] = m.AL[m.IA[i] + j] = 1.0 / (i + j + 1);

	return m;
}

void ReadB(real* B, int& N)
{
	std::ifstream in("input/b.txt");
	
	for (int i = 0; i < N; i++)
		in >> B[i];
}

void ToTight(Matrix& m, real **A)
{
	if (m.isDecomposed)
	{
		std::cout << "ERROR! Matrix is decomposed. Can't convert to tight format." << std::endl;
		return;
	}

	for (int i = 0; i < m.N; i++)
		for (int j = 0; j < m.N; j++)
			A[i][j] = 0;

	for (int i = 0; i < m.N; i++)
		A[i][i] = m.DI[i];

	for (int i = 1; i < m.N; i++)
	{
		int i0 = m.IA[i];
		int i1 = m.IA[i + 1];
		int j = i - (i1 - i0);

		for (int k = i0; k < i1; k++, j++)
		{
			A[i][j] = m.AL[k];
			A[j][i] = m.AU[k];
		}
	}
}