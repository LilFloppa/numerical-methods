#pragma once

#include <vector>
#include <functional>

struct FiniteElement
{
	double begin, end;
	int v1, v2, v3;
};

using FiniteElementIterator = std::vector<FiniteElement>::iterator;

//TODO: Посчитать матрицы жесткости для трех функций

std::vector<std::vector<double>> M = {
	{ 2.0 / 15, 1.0 / 15, -1.0 / 30 },
	{ 1.0 / 15, 8.0 / 15, 1.0 / 15 },
	{ -1.0 / 30, 1.0 / 15, 2.0 / 15 }
};

std::vector<std::vector<double>> G = {
	{ 7.0 / 3, -8.0 / 3, 1.0 / 3 },
	{ -8.0 / 3, 16.0 / 3, -8.0 / 3 },
	{ 1.0 / 3, -8.0 / 3, 7.0 / 3 }
};

std::vector<std::function<double(double)>> basis =
{
	[](double x) { return 2 * (x - 1) * (x - 0.5); },
	[](double x) { return 4 * x * (1 - x); },
	[](double x) { return 2 * x * (x - 0.5); }
};

std::vector<std::function<double(double)>> basis =
{
	[](double x) { return 4 * x - 3; },
	[](double x) { return 4 - 8 * x; },
	[](double x) { return 4 * x - 1; }
};

double F(double x)
{

}

double lamba(double x, double du)
{
	return 2 * du + 3 * x;
}