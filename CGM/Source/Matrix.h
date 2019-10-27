#pragma once
#include <fstream>

struct Matrix
{
	int N;
	double* DI;
	double* AL;
	double* AU;
	int* IA;
	int* JA;
};

void ReadMatrix(Matrix& m, int& maxiter, double& eps);
void ReadB(int N, double* b);
void Multiply(Matrix& A, double* vec, double* res);