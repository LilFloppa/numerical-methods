#include "Matrix.h"

void ReadMatrixSize(int& N, int& ALsize)
{
	std::ifstream in("input/size.txt");
	in >> N >> ALsize;
	in.close();
}

void ReadMatrix(Matrix mat)
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

void LUDecomposition(Matrix mat)
{
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
}

void Solve(Matrix mat, real* B, real *Y, real* X)
{
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

void ReadB(real* B, int& N)
{
	std::ifstream in("input/b.txt");
	
	for (int i = 0; i < N; i++)
		in >> B[i];
}