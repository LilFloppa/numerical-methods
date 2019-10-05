#pragma once
#include <fstream>
#include <string>

using namespace std;

const int maxiter = 50000;
const float eps = 1e-43;

struct Matrix
{
	float** A;
	int N;
	int di_num;
	int m; 
};

void ReadMatrix(Matrix& M);										// Чтение матрицы
void ReadX0(float* x0, int& N);									// Чтение начального приближения
void ReadF(float* f, int& N);									// Чтение правой части
void Multiply(Matrix& M, float* vec, float* res);				// Умножение матрицы на вектор
float Norm(float* vec, int& N);									// Норма вектора
float RelativeDifference(Matrix& M, float* f, float* x);		// Относительная невязка
int Jacobi(Matrix& M, float* f, float* x0, float* x, float w);	// Метод Якоби
int Seidel(Matrix& M, float* f, float* x0, float* x, float w);	// Метод Зейделя
