#pragma once
#include "../stdafx.h"
#include "../Matrix.h"

void LUDecomposition(SparseMatrix& mat);
void Solve(SparseMatrix& mat, std::vector<double>& x, std::vector<double>& b);
void Multiply(SparseMatrix& mat, std::vector<double>& vec, std::vector<double>& res);