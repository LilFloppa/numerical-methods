#include "Matrix/Matrix.h"

int main()
{
	Matrix m = { };
	int maxiter;
	double eps;
	ReadMatrix(m, maxiter, eps);

	double* b = new double[m.N];

	for (int i = 0; i < m.N; i++)
		b[i] = 1;

	double* res = new double[m.N];

	Multiply(m, b, res);
	return 0;
}