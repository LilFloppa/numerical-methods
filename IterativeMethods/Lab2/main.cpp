#include "Matrix.h"

int main()
{
	Matrix mat = { };
	ReadMatrix(mat);

	double* f = new double[mat.N];
	ReadF(f, mat.N);
	
	double* x0 = new double[mat.N];
	double* x1 = new double[mat.N];
	double* r = new double[mat.N];

	ReadX0(x0, mat.N);
	int J = Jacobi(mat, f, x0, x1, r, 0.99);

	ReadX0(x0, mat.N);
	int Z = Zeidel(mat, f, x0, r, 1.01);

	while (true)
	{

	}
	return 0;
}