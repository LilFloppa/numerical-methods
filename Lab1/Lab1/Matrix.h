#pragma once
#include <fstream>
#include <iostream>

typedef float real;

struct Matrix
{
	real* DI;
	real* AL;
	real* AU;
	int* IA;
	int N;
	int ALSize;
	bool isDecomposed = false;
};

void ReadMatrixSize(int& N, int& ALsize);			// Чтение размера матрицы
void ReadMatrix(Matrix& mat);						// Чтение матрицы в профильном формате
void ReadB(real* B, int& N);						// Чтение правой части b
void LUDecomposition(Matrix& mat);					// Разложение матрицы А на матрицы L и U
void Solve(Matrix& mat, real* B, real* Y, real* X);	// Решение СЛАУ
void Multiply(Matrix& mat, real* vec, real* res);	// Умножение матрицы на вектор
Matrix HilbertMatrix(int size);						// Создание матрицы Гильберта порядка size