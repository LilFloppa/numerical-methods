#include "Matrix.h"

void ReadMatrix(Matrix& M)
{
	// Reading Matrix size
	ifstream in("input/size.txt", ios::in);
	in >> M.N >> M.m;
	in.close();

	int mSize = M.N - M.m - 1;
	// Matrix initialization 
	M.D = new double[M.N];
	M.L1 = new double[M.N - 1];
	M.U1 = new double[M.N - 1];
	M.L2 = new double[mSize];
	M.U2 = new double[mSize];

	// Reading Matrix diagonals
	in.open("input/matrix.txt", ios::in);
	for (int i = 0; i < mSize; i++) in >> M.L2[i];
	for (int i = 0; i < M.N - 1; i++) in >> M.L1[i];
	for (int i = 0; i < M.N; i++) in >> M.D[i];
	for (int i = 0; i < M.N - 1; i++) in >> M.U1[i];
	for (int i = 0; i < mSize; i++) in >> M.U2[i];
	in.close();
}

void ReadF(double* f,int& N)
{
	ifstream in("input/f.txt", ios::in);
	for (int i = 0; i < N; i++)
		in >> f[i];
	in.close();
}

void ReadX0(double* x0, int& N)
{
	ifstream in("input/x0.txt", ios::in);
	if (!in.fail())
	{
		for (int i = 0; i < N; i++)
			in >> x0[i];
	}
	else
	{
		for (int i = 0; i < N; i++)
			x0[i] = 0;
	}

	in.close();
}

double Multiply(Matrix& M, double* vec, int i)
{
	int i_m = M.m + 1;
	double sum = 0;

	sum += M.D[i] * vec[i];

	if (i < M.N - 1)
		sum += M.U1[i] * vec[i + 1];

	if (i > 0)
		sum += M.L1[i - 1] * vec[i - 1];

	if (i < M.N - i_m)
		sum += M.U2[i] * vec[i + i_m];

	if (i > i_m - 1)
		sum += M.L2[i - i_m] * vec[i - i_m];

	return sum;
}

double Norm(double* vec, int& N)
{
	double norm = 0;

	for (int i = 0; i < N; i++)
		norm += vec[i] * vec[i];

	return sqrt(norm);
}

int Jacobi(Matrix& M, double* f, double *x0, double* x1, double* r, double w)
{
	int iterNum = 0;

	// Relative difference
	for (int i = 0; i < M.N; i++)
	{
		double sum = Multiply(M, x0, i);
		r[i] = f[i] - sum;
	}
	double diff = Norm(r, M.N) / Norm(f, M.N);

	// x1 = x0
	for (int i = 0; i < M.N; i++)
		x1[i] = x0[i];

	for (; iterNum < maxiter && diff >= eps; iterNum++)
	{
		for (int i = 0; i < M.N; i++)
		{
			double sum = Multiply(M, x0, i);
			r[i] = f[i] - sum;
			x1[i] = x0[i] + w * r[i] / M.D[i];
		}

		diff = Norm(r, M.N) / Norm(f, M.N);

		for (int i = 0; i < M.N; i++)
			x0[i] = x1[i];
	}

	return iterNum;
}

int Zeidel(Matrix& M, double* f, double* x, double* r, double w)
{
	int iterNum = 0;

	// Relative difference
	for (int i = 0; i < M.N; i++)
	{
		double sum = Multiply(M, x, i);
		r[i] = f[i] - sum;
	}
	double diff = Norm(r, M.N) / Norm(f, M.N);

	for (; iterNum < maxiter && diff >= eps; iterNum++)
	{
		for (int i = 0; i < M.N; i++)
		{
			double sum = Multiply(M, x, i);
			r[i] = f[i] - sum;
			x[i] += w * r[i] / M.D[i];
		}

		diff = Norm(r, M.N) / Norm(f, M.N);
	}

	return iterNum;
}