#pragma once

#include <fstream>

typedef float real;

void ReadMatrixSize(int& N, int& ALsize);										// Чтение размера матрицы

void ReadMatrix(int& N, int& ALsize, real* DI, real* AL, real* AU, int* IA);	// Чтение матрицы в профильном формате

void LUDecomposition(int& N, real* DI, real* AL, real* AU, int* IA);			// Разложение матрицы А на матрицы L и U

void Solve(int& N, real* DI, real* AL, real* AU, int* IA, real* B);				// Решение СЛАУ