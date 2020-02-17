#pragma once

#include <vector>
#include <string>
#include <fstream>

struct Interval
{
	double begin, end;
	int n;
};

void ReadIntervals(std::string filename, std::vector<Interval>& intervals);
void BuildMesh(std::vector<Interval> intervals, int k, double* x, double* hx);