#pragma once
#include "stdafx.h"

class Matrix
{
public:
	Matrix(int n) : N(n)
	{
		DI.resize(n, 0.0);
		IA.resize(n + 1, 0);
	}

	double& operator()(int i, int j)
	{
		if (i >= N || j >= N || i < 0 || j < 0)
			throw std::out_of_range("Bad index for matrix");

		if (i == j)
			return DI[i];

		if (i > j)
		{
			for (int col = IA[i]; col < IA[i + 1]; col++)
				if (JA[col] == j)
					return AL[col];
		}
		else
		{
			for (int line = IA[j]; line < IA[j + 1]; line++)
				if (JA[line] == i)
					return AU[line];
		}

		throw std::out_of_range("Bad index for matrix");
	}

	void Clear()
	{
		for (auto& di : DI)
			di = 0;

		for (auto& al : AL)
			al = 0;

		for (auto& au : AU)
			au = 0;
	}

	int N = 0;
	std::vector<double> DI, AL, AU;
	std::vector<int> IA, JA;
};