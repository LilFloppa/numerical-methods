#include "Matrix.h"

void ReadMatrixSize(int& N, int& ALsize)
{
	std::ifstream in("input/size.txt");
	in >> N >> ALsize;
	in.close();
}

void ReadMatrix(Matrix& mat)
{
	std::ifstream in("input/di.txt");
	for (int i = 0; i < mat.N; i++)
		in >> mat.DI[i];
	in.close();

	in.open("input/al.txt");
	for (int i = 0; i < mat.ALSize; i++)
		in >> mat.AL[i];
	in.close();

	in.open("input/au.txt");
	for (int i = 0; i < mat.ALSize; i++)
		in >> mat.AU[i];
	in.close();

	in.open("input/ia.txt");
	for (int i = 0; i < mat.N + 1; i++)
	{
		in >> mat.IA[i];
		mat.IA[i]--;
	}
	in.close();
}

void LUDecomposition(Matrix& mat)
{
	if (mat.isDecomposed)
	{
		std::cout << "ERROR. Matrix is already decomposed." << std::endl;
		return;
	}

	for (int i = 0; i < mat.N; i++)
	{
		real sumL = 0, sumU = 0, sumD = 0;
		int j = i - (mat.IA[i + 1] - mat.IA[i]);

		for (int k = mat.IA[i]; k < mat.IA[i + 1]; k++)
		{
			// Вычисление элеметов L[i][j] и U[j][i]
			for (int m = 0; m < k - mat.IA[i]; m++)
			{
				sumL += mat.AL[mat.IA[i] + m] * mat.AU[mat.IA[j] + m];
				sumU += mat.AU[mat.IA[i] + m] * mat.AL[mat.IA[j] + m];
			}

			mat.AL[k] -= sumL;
			mat.AU[k] -= sumU;
			mat.AU[k] /= mat.DI[j];

			// Накопление суммы для вычисления DI[i]
			sumD += mat.AL[k] * mat.AU[k];
			sumL = sumU = 0;
			j++;
		}
		
		// Вычисление DI[i]
		mat.DI[i] -= sumD;
		sumD = 0;
	}

	mat.isDecomposed = true;
}

void Solve(Matrix& mat, real* B, real *Y, real* X)
{
	if (!mat.isDecomposed)
	{
		std::cout << "ERROR. Can't solve. Matrix is not decomposed." << std::endl;
		return;
	}

	// Решение системы Ly = b прямым обходом
	for (int i = 0; i < mat.N; i++)
	{
		real sumL = 0;
		int m = i - 1;
		for (int k = mat.IA[i + 1] - 1; k >= mat.IA[i]; k--)
		{
			sumL += mat.AL[k] * B[m];
			m--;
		}

		Y[i] =  (B[i] - sumL) / mat.DI[i];
	}

	// Решение системы Ux = y обратным обходом
	for (int i = mat.N - 1; i > 0; i--)
	{
		int m = i - 1;
		for (int k = mat.IA[i + 1] - 1; k >= mat.IA[i]; k--)
		{
			X[m] = Y[m] - mat.AU[k] * X[i];
			m--;
		}
	}
}

void Multiply(Matrix& mat, real* vec, real* res)
{
	if (mat.isDecomposed)
	{
		std::cout << "ERROR. Matrix is decomposed. Can't multiply." << std::endl;
		return;
	}

	for (int i = 0; i < mat.N; i++)
		res[i] = vec[i] * mat.DI[i];

	for (int i = 1; i < mat.N; i++)
	{
		int j = i - (mat.IA[i + 1] - mat.IA[i]);

		for (int k = mat.IA[i]; k < mat.IA[i + 1]; k++)
		{
			res[i] += vec[j] * mat.AL[k];
			res[j] += vec[i] * mat.AU[k];
			j++;
		}
	}
}

Matrix HilbertMatrix(int size)
{
	Matrix mat = { };
	int ALSize = size * (size - 1) / 2;
	mat.DI = new real[size];
	mat.AL = new real[ALSize];
	mat.AU = new real[ALSize];
	mat.IA = new int[size + 1];

	for (int i = 0; i < size; i++)
		mat.DI[i] = 1.0 / (2 * i - 1);

	mat.IA[0] = 1;
	for (int i = 1; i < size + 1; i++)
		mat.IA[i] = 

	for (int i = 1; i < size; i++)
	{
		for (int j = 0; j < i; j++)

	}

	return mat;
}

void ReadB(real* B, int& N)
{
	std::ifstream in("input/b.txt");
	
	for (int i = 0; i < N; i++)
		in >> B[i];
}