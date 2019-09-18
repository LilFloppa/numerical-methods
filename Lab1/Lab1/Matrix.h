#pragma once
#include <fstream>

typedef float real;

struct Matrix
{
	real* DI;
	real* AL;
	real* AU;
	int* IA;
	int N;
	int ALSize;
};

void ReadMatrixSize(int& N, int& ALsize);			// Чтение размера матрицы
void ReadMatrix(Matrix mat);						// Чтение матрицы в профильном формате
void LUDecomposition(Matrix mat);					// Разложение матрицы А на матрицы L и U
void Solve(Matrix mat, real* B, real* Y, real* X);	// Решение СЛАУ