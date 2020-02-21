#pragma once

#include <vector>
#include <iostream>
#include <fstream>

using namespace std;

struct Matrix
{
	vector<double> D, L1, L2, U1, U2;
	int N, m;
};

void InitMatrix(Matrix& A, int N, int m);
void ToTight(Matrix& A, vector<vector<double>>& AT);
void PrintMatrix(Matrix& A);
void PrintSLAE(Matrix& A, vector<double>& b);