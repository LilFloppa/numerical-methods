#include <iostream>

#include "Matrix.h"

typedef float real;

int main()
{
	Matrix matrix = { 0 };
	ReadMatrixSize(matrix.N, matrix.ALSize);

	matrix.DI = new real[matrix.N];
	matrix.AL = new real[matrix.ALSize];
	matrix.AU = new real[matrix.ALSize];
	matrix.IA = new int[matrix.N + 1];

	real* B = new real[matrix.N];
	real* Y = B;
	real* X = B;

	ReadMatrix(matrix);
	LUDecomposition(matrix);
	ReadB(B, matrix.N);
	Solve(matrix, B, Y, X);

	for (int i = 0; i < matrix.N; i++)
		std::cout << matrix.DI[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < matrix.ALSize; i++)
		std::cout << matrix.AL[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < matrix.ALSize; i++)
		std::cout << matrix.AU[i] << ' ';

	std::cout << std::endl << std::endl;

	for (int i = 0; i < matrix.N; i++)
		std::cout << B[i] << ' ';

	return 0;
}