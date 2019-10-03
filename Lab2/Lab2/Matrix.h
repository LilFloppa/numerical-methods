#pragma once
#include <fstream>
#include <string>

using namespace std;

void ReadMatrixSize(int& N, int& di_num, int& m);

void ReadMatrix(int& N, int& di_num, float** A);

void ReadB(float* b, int& N);

void Multiply(int& N, int& di_num, float** A, int& m, float* vec, float* res);