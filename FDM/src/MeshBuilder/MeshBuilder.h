#pragma once

#include <vector>
#include <string>
#include <fstream>

struct Interval
{
	double begin, end;
	int n;
};

struct Node
{
	int i, j;
	int n;
	Node(int i, int j, int n): i(i), j(j), n(n) {}
};

const double eps = 1.0e-5;

void ReadIntervals(std::string filename, std::vector<Interval>& intervals);
int CountBreakPoints(std::vector<Interval>& intervals);
void BuildMesh(std::vector<Interval> intervals, int k, double* x, double* hx);
std::vector<std::vector<Node>> MeshNumbering(double* x, double* y, int kx, int ky);