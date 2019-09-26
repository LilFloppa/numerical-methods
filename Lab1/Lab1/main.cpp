#include <iostream>
#include <stdio.h>
#include "Matrix.h"

int main()
{
	/*Matrix m = { };
	real* X = nullptr;
	real* B = nullptr;
	real* res = nullptr;

	for (int k = 1; k < 8; k++)
	{
		m = HilbertMatrix(k);
		X = new real[k];
		for (int i = 0; i < k; i++)
			X[i] = i + 1;

		B = new real[k];
		Multiply(m, X, B);

		res = new real[k];
		LUDecomposition(m);
		Solve(m, B, res);

		for (int i = 0; i < k; i++)
			printf_s("%10.6E  %10.6E\n", res[i], X[i] - res[i]);

		std::cout << std::endl << std::endl;
	}*/


	Matrix mat = HilbertMatrix(20);
	
	real** A = new real*[mat.N];

	for (int i = 0; i < mat.N; i++)
		A[i] = new real[mat.N];


	ToTight(mat, A);

	for (int i = 0; i < mat.N; i++)
	{
		for (int j = 0; j < mat.N; j++)
			printf_s("%4.2f ", A[i][j]);

		std::cout << std::endl;
	}

	std::cout << std::endl << std::endl;
	system("pause");
	return 0;
}