#include <iostream>

#include "Matrix.h"

typedef float real;

int main()
{
	int N = 0, ALSize = 0;
	ReadMatrixSize(N, ALSize);

	real* DI = new real[N];
	real* AL = new real[ALSize];
	real* AU = new real[ALSize];
	int* IA = new int[N + 1];

	real* B = new real[N];
	B[0] = 1;
	B[1] = 1;
	B[2] = 1;
	B[3] = 1;

	ReadMatrix(N, ALSize, DI, AL, AU, IA);
	LUDecomposition(N, DI, AL, AU, IA);
	Solve(N, DI, AL, AU, IA, B);

	for (int i = 0; i < N; i++)
		std::cout << DI[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < ALSize; i++)
		std::cout << AL[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < ALSize; i++)
		std::cout << AU[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < N; i++)
		std::cout << B[i] << ' ';

	return 0;
}