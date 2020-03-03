#pragma once

#include <vector>
#include <functional>

#include <SLAE/SLAE.h>
#include "../MeshBuilder/MeshBuilder.h"

using namespace std;

const double gamma = 2.0;

const function<double(double, double)> f = [](double x, double y) { return -4 + gamma * (x * x + y * y); };

const vector<function<double(double, double)>> borderFuncs =
{
	[](double x, double y) { return x * x; },
	[](double x, double y) { return y * y; },
	[](double x, double y) { return y * y + 0.25; },
	[](double x, double y) { return x * x + 1.0; },
	[](double x, double y) { return x * x + 0.25; },
	[](double x, double y) { return y * y + 0.25; },
	[](double x, double y) { return y * y + 1.0; },
	[](double x, double y) { return x * x + 2.25; }
};

void BuildMatrix(
	SLAE::Matrix& A,
	vector<double>& b,
	vector<vector<int>>& areas,
	vector<Interval>& intervalsX,
	vector<Interval>& intervalsY,
	vector<BoundaryCondition>& conds,
	vector<double>& x,
	vector<double>& y,
	int kx, int ky);

void BoundaryConditions(
	SLAE::Matrix& A,
	vector<double>& b,
	vector<double>& x,
	vector<double>& y,
	vector<BoundaryCondition>& conds);

int IntervalNo(vector<Interval>& intervals, int index);
bool IsOnBorder(vector<BoundaryCondition>& conds, int ix, int iy);
