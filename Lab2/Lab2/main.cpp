#include "Matrix.h"

int main()
{
	Matrix mat = { };
	ReadMatrix(mat);

	float* f = new float[mat.N];
	ReadF(f, mat.N);
	
	float* x0 = new float[mat.N];
	float* x = new float[mat.N];
	ReadX0(x0, mat.N);

	int J = Jacobi(mat, f, x0, x, 0.909);
	int S = Seidel(mat, f, x0, x, 0.909);

	while (true)
	{

	}
	return 0;
}