#include "Matrix.h"

void InitMatrix(Matrix& A, int N, int m)
{
	A.N = N;
	A.m = m - 1;
	int mSize = A.N - A.m - 1;

	A.D.resize(A.N, 1);
	A.L1.resize(A.N - 1);
	A.U1.resize(A.N - 1);
	A.L2.resize(mSize);
	A.U2.resize(mSize);
}

void ToTight(Matrix& A, vector<vector<double>>& AT)
{
	for (int i = 0; i < A.N; i++)
	{
		AT[i][i] = A.D[i];
	}

	for (int i = 0; i < A.N - 1; i++)
	{
		AT[i + 1][i] = A.L1[i];
		AT[i][i + 1] = A.U1[i];
	}

	for (int i = 0; i < A.N - A.m - 1; i++)
	{
		AT[i + A.m + 1][i] = A.L2[i];
		AT[i][i + A.m + 1] = A.U2[i];
	}
}

void PrintMatrix(Matrix& A)
{
	vector<vector<double>> AT(A.N, vector<double>(A.N));
	ToTight(A, AT);

	for (auto x : AT)       
	{
		for (auto y : x)
			cout << y << "\t";

		cout << endl;
	}
}

void PrintSLAE(Matrix& A, vector<double>& b)
{
	vector<vector<double>> AT(A.N, vector<double>(A.N));
	ToTight(A, AT);

	ofstream out("out.csv");

	int i = 0;
	for (int i = 0; i < A.N; i++)
	{
		for (auto y : AT[i])
			out << y << ';';
	
		out << ";;;" << b[i] << endl;
	}

	out.close();
}