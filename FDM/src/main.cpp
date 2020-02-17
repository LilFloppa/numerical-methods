#include "MeshBuilder/MeshBuilder.h"

#include <iostream>

int main()
{
	std::vector<Interval> intervals;
	ReadIntervals("input/intervals.txt", intervals);
	int kx = 0;
	for (auto& interval : intervals)
		kx += interval.n;
	kx++;

	double* x = new double[kx];
	double* hx = new double[kx - 1];
	BuildMesh(intervals, kx, x, hx);

	for (int i = 0; i < kx - 1; i++)
		std::cout << x[i] << "\t\t\t" << hx[i] << std::endl;

	return 0;
}