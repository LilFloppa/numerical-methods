#include "Matrix.h"

void ReadMatrix(Matrix& M)
{
	// Reading Mrix size
	ifstream in("input/size.txt", ios::in);
	in >> M.N >> M.di_num >> M.m;
	in.close();

	// Matrix initialization 
	M.A = new float* [M.N];
	for (int i = 0; i < M.N; i++)
		M.A[i] = new float[M.di_num];

	// Reading Mix diagonals
	in.open("input/matrix.txt", ios::in);
	for (int i = 0; i < M.N; i++)
		for (int j = 0; j < M.di_num; j++)
			in >> M.A[i][j];
	in.close();
}

void ReadF(float* f,int& N)
{
	ifstream in("input/f.txt", ios::in);
	for (int i = 0; i < N; i++)
		in >> f[i];
	in.close();
}

void ReadX0(float* x0, int& N)
{
	ifstream in("input/x0.txt", ios::in);
	for (int i = 0; i < N; i++)
		in >> x0[i];
	in.close();
}

void Multiply(Matrix& M, float* vec, float* res)
{
	int i_di = (M.di_num - 1) / 2;
	int i_m = M.m + 1;

	for (int i = 0; i < M.N; i++)
		res[i] = M.A[i][i_di] * vec[i];

	for (int i = 1; i < M.N; i++)
	{
		res[i] += M.A[i][i_di - 1] * vec[i - 1];
		res[i - 1] += M.A[i][i_di + 1] * vec[i];
	}

	for (int i = i_m; i < M.N; i++)
	{
		res[i] += M.A[i][i_di - 2] * vec[i - i_m];
		res[i - i_m] += M.A[i][i_di + 2] * vec[i];
	}

}

float Norm(float* vec, int& N)
{
	float norm = 0;

	for (int i = 0; i < N; i++)
		norm += vec[i] * vec[i];

	return sqrt(norm);
}

float RelativeDifference(Matrix& M, float* f, float* x)
{
	float* diffVector = new float[M.N];
	Multiply(M, x, diffVector);

	for (int i = 0; i < M.N; i++)
		diffVector[i] -= f[i];

	return Norm(diffVector, M.N) / Norm(f, M.N);
}

int Jacobi(Matrix& M, float* f, float *x0, float* x, float w)
{
	int i_di = (M.di_num - 1) / 2;
	int i = 0;

	for (int i = 0; i < M.N; i++)
		x[i] = x0[i];

	float relDiff = RelativeDifference(M, f, x);
	float* temp = new float[M.N];

	for (; i < maxiter && relDiff >= eps; i++)
	{
		Multiply(M, x, temp);
		for (int i = 0; i < M.N; i++)
			x[i] += w * (f[i] - temp[i]) / M.A[i][i_di];
		
		relDiff = RelativeDifference(M, f, x);
	}

	return i;
}

int Seidel(Matrix& M, float* f, float* x0, float* x, float w)
{
	int i_di = (M.di_num - 1) / 2;
	int i = 0;

	for (int i = 0; i < M.N; i++)
		x[i] = x0[i];

	float relDiff = RelativeDifference(M, f, x);
	float* temp = new float[M.N];

	for (; i < maxiter && relDiff >= eps; i++)
	{
		for (int i = 0; i < M.N; i++)
		{
			Multiply(M, x, temp);
			x[i] += w * (f[i] - temp[i]) / M.A[i][i_di];
		}

		relDiff = RelativeDifference(M, f, x);
	}

	return i;
}