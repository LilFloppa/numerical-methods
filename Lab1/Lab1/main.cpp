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
	real* res = new real[matrix.N];

	Matrix mat = HilbertMatrix(2);
	//ReadMatrix(matrix);
	//LUDecomposition(matrix);
	//ReadB(B, matrix.N);
	//Multiply(matrix, B, res);
	//Solve(matrix, B, Y, X);

	return 0;
}