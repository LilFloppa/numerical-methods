#pragma once

#include <vector>
#include <SLAE/Matrix.h>

std::vector<std::vector<double>> M = {
	{ 2.0 / 15, 1.0 / 15, -1.0 / 30 },
	{ 1.0 / 15, 8.0 / 15, 1.0 / 15 },
	{ -1.0 / 30, 1.0 / 15, 2.0 / 15 }
};

std::vector<std::vector<double>> G = {
	{ 7.0 / 3, -8.0 / 3, 1.0 / 3 },
	{ -8.0 / 3, 16.0 / 3, -8.0 / 3 },
	{ 1.0 / 3, -8.0 / 3, 7.0 / 3 }
};

const int BasisSize = 3;

struct FiniteElement
{
	double begin, end;
	int v1, v2, v3;
};

int MatrixALSize(int size)
{
	return 3 * (size - 1) / 2;
}

void InitMatrix(SLAE::Matrix& A, int size, int ALsize)
{
	A.N = size;
	A.DI = new double[size];
	A.AL = new double[ALsize];
	A.AU = new double[ALsize];
	A.IA = new int[size + 1];
	A.JA = new int[ALsize];
}

void BuildPortrait(SLAE::Matrix& A)
{
	int n = A.N;
	int offset = 1;

	A.IA[0] = A.IA[1] = 0;
	for (int i = 1; i < n; i++, offset++)
	{
		A.IA[i + 1] = A.IA[i] + offset;

		if (offset == 2)
			offset = 0;
	}

	int alsize = A.IA[n];
	A.JA[0] = 0;
	int i = 1;
	while (i < alsize)
	{
		A.JA[i] = A.JA[i - 1];
		i++;
		A.JA[i] = A.JA[i - 1] + 1;
		i++;
		A.JA[i] = A.JA[i - 1] + 1;
		i++;
	}
}

void BuildGlobal(SLAE::Matrix& A, std::vector<FiniteElement>& elements)
{
	std::vector<std::vector<double>> local(BasisSize, std::vector<double>(BasisSize));
	std::vector<double> b(BasisSize);

	for (auto& element : elements)
	{
		BuildLocal(local, element);
		BuildLocalB(b, element);
	}
}

void BuildLocal(std::vector<std::vector<double>>& local, FiniteElement& element)
{

}

void BuildLocalB(std::vector<double>& b, FiniteElement& element)
{

}