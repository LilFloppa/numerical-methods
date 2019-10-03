#include "Matrix.h"

void ReadMatrixSize(int& N, int& di_num, int& m)
{
	ifstream in("input/size.txt", ios::in);
	in >> N  >> di_num >> m;
	in.close();
}

void ReadMatrix(int& N, int& di_num, float **A)
{
	ifstream in("input/matrix.txt", ios::in);

	for (int i = 0; i < N; i++)
		for (int j = 0; j < di_num; j++)
			in >> A[i][j];

	in.close();
}

void ReadB(float* b,int& N)
{
	ifstream in("input/b.txt", ios::in);

	for (int i = 0; i < N; i++)
		in >> b[i];
}

void Multiply(int& N, int& di_num, float** A, int& m, float* vec, float* res)
{
	int i_di = (di_num - 1) / 2;
	int i_m = m + 1;

	for (int i = 0; i < N; i++)
		res[i] += A[i][i_di] * vec[i];

	for (int i = 1; i < N; i++)
	{
		res[i] += A[i][i_di - 1] * vec[i - 1];
		res[i - 1] += A[i][i_di + 1] * vec[i];
	}

	for (int i = i_m; i < N; i++)
	{
		res[i] += A[i][i_di - 2] * vec[i - i_m];
		res[i - i_m] += A[i][i_di + 2] * vec[i];
	}

}