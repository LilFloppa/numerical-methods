#include "Matrix.h"

int main()
{
	int N = 0, di_num = 0, m = 0;
	ReadMatrixSize(N, di_num, m);

	float** A = new float* [N];
	for (int i = 0; i < N; i++)
		A[i] = new float[di_num];

	ReadMatrix(N, di_num, A);

	float* res = new float[N];
	float* vec = new float[N];

	for (int i = 0; i < N; i++)
	{
		res[i] = 0;
		vec[i] = 1;
	}

	Multiply(N, di_num, A, m, vec, res);

	while (true)
	{
		m++;
	}

	return 0;
}