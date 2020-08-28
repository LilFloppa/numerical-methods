#pragma once
#include <fstream>
#include <iostream>
#include <utility>

typedef double real;
typedef double realScal;

struct Matrix
{
	real *DI, *AL, *AU;
	int *IA;
	int N, ALSize;
	bool isDecomposed = false;
};

void ReadMatrixSize(int& N, int& ALsize);			// Чтение размера матрицы
void ReadMatrix(Matrix& mat);						// Чтение матрицы в профильном формате
void ReadB(real* B, int& N);						// Чтение правой части b
void LUDecomposition(Matrix& mat);					// Разложение матрицы А на матрицы L и U
void Solve(Matrix& mat, real* b);					// Решение СЛАУ
void Multiply(Matrix& mat, real* vec, real* res);	// Умножение матрицы на вектор
Matrix HilbertMatrix(int size);						// Создание матрицы Гильберта порядка size
void ToTight(Matrix& m, real** A);					// Перевод матрицы в плотный формат