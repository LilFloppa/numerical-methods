#pragma once
#include <fstream>
#include <iostream>
#include <utility>
#include <vector>

namespace SLAE
{
	struct Matrix
	{
		std::vector<double> DI, AL, AU;
		std::vector<int> IA;
		int N, ALSize;
	};

	void LUDecomposition(Matrix& mat);
	void Solve(Matrix& mat, std::vector<double>& b);
	void Multiply(Matrix& mat, std::vector<double>& vec, std::vector<double>& res);
	void ToTight(Matrix& m, std::vector<std::vector<double>>& A);
}