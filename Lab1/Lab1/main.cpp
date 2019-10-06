#include <iostream>
#include <stdio.h>
#include "Matrix.h"

int main()
{
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