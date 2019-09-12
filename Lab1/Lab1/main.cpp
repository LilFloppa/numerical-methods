#include <fstream>
#include <iostream>

typedef float real;

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
		real sum1 = 0, sum2 = 0;
		int col = i - (IA[i + 1] - IA[i]);

		for (int k = IA[i]; k < IA[i + 1]; k++)
		{
			for (int j = 0; j < k - IA[i]; j++)
			{
				sum1 += AL[IA[i] + j] * AU[IA[col] + j];
				sum2 += AU[IA[i] + j] * AL[IA[col] + j];
			}

			AL[k] -= sum1;
			AU[k] -= sum2;
			AU[k] /= DI[col];
			
			sum1 = sum2 = 0;
			col++;
		}

		for (int j = IA[i]; j < IA[i + 1]; j++)
			sum1 += AL[j] * AU[j];

		DI[i] -= sum1;
			
	}
}

int main()
{
	int N = 0, ALSize = 0;
	ReadMatrixSize(N, ALSize);

	real* DI = new real[N];
	real* AL = new real[ALSize];
	real* AU = new real[ALSize];
	int* IA = new int[N + 1];

	ReadMatrix(N, ALSize, DI, AL, AU, IA);
	LUDecomposition(N, DI, AL, AU, IA);

	for (int i = 0; i < N; i++)
		std::cout << DI[i] << ' ';

	std::cout << std::endl;

	for (int i = 0; i < ALSize; i++)
		std::cout << AL[i] << ' ';

	std::cout << std::endl;

	for (int i = 0; i < ALSize; i++)
		std::cout << AU[i] << ' ';

	std::cout << std::endl;

	return 0;
}