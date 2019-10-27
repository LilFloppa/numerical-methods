#include "Matrix.h"

void ReadMatrix(Matrix& m, int& maxiter, double& eps)
{
	std::ifstream in("input/kuslau.txt");
	in >> m.N >> maxiter >> eps;
	in.close();

	m.IA = new int[m.N + 1];
	m.DI = new double[m.N];

	in.open("input/ig.txt");
	for (int i = 0; i < m.N + 1; i++)
		in >> m.IA[i];
	in.close();

	int size = m.IA[m.N];
	m.JA = new int[size];
	m.AL = new double[size];
	m.AU = new double[size];

	in.open("input/jg.txt");
	for (int i = 0; i < m.IA[m.N]; i++)
		in >> m.JA[i];
	in.close();

	in.open("input/ggl.txt");
	for (int i = 0; i < m.IA[m.N]; i++)
		in >> m.AL[i];
	in.close();

	in.open("input/ggu.txt");
	for (int i = 0; i < m.IA[m.N]; i++)
		in >> m.AU[i];
	in.close();

	in.open("input/di.txt");
	for (int i = 0; i < m.N; i++)
		in >> m.DI[i];
	in.close();
}

void ReadB(int N, double* b)
{
	std::ifstream in("input/pr.txt");
	for (int i = 0; i < N; i++)
		in >> b[i];
	in.close();
}

void Multiply(Matrix& A, double* vec, double* res)
{
	for (int i = 0; i < A.N; i++)
		res[i] = vec[i] * A.DI[i];

	for (int i = 0; i < A.N; i++)
	{
		for (int j = A.IA[i]; j < A.IA[i + 1]; j++)
		{
			int col = A.JA[j];
			res[i] += A.AL[j] * vec[col];
			res[col] += A.AU[j] * vec[i];
		}
	}
}
