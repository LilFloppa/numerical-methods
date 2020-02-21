#pragma once

#include <vector>
#include <functional>

#include "../MeshBuilder/MeshBuilder.h"
#include "Matrix.h"

using namespace std;

const double gamma = 2.0;

const function<double(double, double)> f = [](double x, double y) { return gamma * x; };

const vector<function<double(double, double)>> borderFuncs =
{
	[](double x, double y) { return x; },
	[](double x, double y) { return 0.0; },
	[](double x, double y) { return 2.0; },
	[](double x, double y) { return x; },
	[](double x, double y) { return x; },
	[](double x, double y) { return 1.0; }
};

void BuildMatrix(
	Matrix& A,
	vector<double>& b,
	vector<vector<int>>& areas,
	vector<Interval>& intervalsX,
	vector<Interval>& intervalsY,
	vector<double>& x,
	vector<double>& y,
	vector<double>& hx,
	vector<double>& hy,
	int kx, int ky);

void BoundaryConditions(
	Matrix& A, 
	vector<double>& b, 
	vector<double>& x,
	vector<double>& y, 
	int kx, int ky, 
	int xBorderNode, 
	int yBorderNode);

int IntervalNo(vector<Interval>& intervals, int index);
