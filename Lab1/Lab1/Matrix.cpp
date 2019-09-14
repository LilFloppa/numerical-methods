#include "Matrix.h"


void ReadMatrixSize(int& N, int& ALsize)
{
	std::ifstream in("input/size.txt");
	in >> N >> ALsize;
	in.close();
}

void ReadMatrix(int& N, int& ALsize, real* DI, real* AL, real* AU, int* IA)
{
	std::ifstream in("input/di.txt");
	for (int i = 0; i < N; i++)
		in >> DI[i];
	in.close();

	in.open("input/al.txt");
	for (int i = 0; i < ALsize; i++)
		in >> AL[i];
	in.close();

	in.open("input/au.txt");
	for (int i = 0; i < ALsize; i++)
		in >> AU[i];
	in.close();

	in.open("input/ia.txt");
	for (int i = 0; i < N + 1; i++)
	{
		in >> IA[i];
		IA[i]--;
	}
	in.close();
}

void LUDecomposition(int& N, real* DI, real* AL, real* AU, int* IA)
{
	for (int i = 0; i < N; i++)
	{
		real sumL = 0, sumU = 0, sumD = 0;
		int j = i - (IA[i + 1] - IA[i]);

		for (int k = IA[i]; k < IA[i + 1]; k++)
		{
			// Вычисление элеметов L[i][j] и U[j][i]
			for (int m = 0; m < k - IA[i]; m++)
			{
				sumL += AL[IA[i] + m] * AU[IA[j] + m];
				sumU += AU[IA[i] + m] * AL[IA[j] + m];
			}

			AL[k] -= sumL;
			AU[k] -= sumU;
			AU[k] /= DI[j];

			// Накопление суммы для вычисления DI[i]
			sumD += AL[k] * AU[k];
			sumL = sumU = 0;
			j++;
		}
		
		// Вычисление DI[i]
		DI[i] -= sumD;
		sumD = 0;
	}
}

void Solve(int& N, real* DI, real* AL, real* AU, int* IA, real* B, real *Y, real* X)
{
	// Решение системы Ly = b прямым обходом
	for (int i = 0; i < N; i++)
	{
		real sumL = 0;
		int m = i - 1;
		for (int k = IA[i + 1] - 1; k >= IA[i]; k--)
		{
			sumL += AL[k] * B[m];
			m--;
		}

		Y[i] =  (B[i] - sumL) / DI[i];
	}

	// Решение системы Ux = y обратным обходом
	for (int i = N - 1; i > 0; i--)
	{
		int m = i - 1;
		for (int k = IA[i + 1] - 1; k >= IA[i]; k--)
		{
			X[m] = Y[m] - AU[k] * X[i];
			m--;
		}
	}
}