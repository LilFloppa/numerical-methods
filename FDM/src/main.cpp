#include "MeshBuilder/MeshBuilder.h"

#include <iostream>

int main()
{
	std::vector<Interval> intervalsX, intervalsY;
	ReadIntervals("input/intervalsX.txt", intervalsX);
	ReadIntervals("input/intervalsY.txt", intervalsY);
	int kx = CountBreakPoints(intervalsX);
	int ky = CountBreakPoints(intervalsY);

	double* x = new double[kx];
	double* hx = new double[kx - 1];
	BuildMesh(intervalsX, kx, x, hx);

	double* y = new double[ky];
	double* hy = new double[ky - 1];
	BuildMesh(intervalsY, ky, y, hy);

	std::vector<std::vector<Node>> numbering = MeshNumbering(x, y, kx, ky);

	for (auto& x : numbering)
	{
		for (auto& y : x)
			std::cout << y.n << "\t";

		std::cout << std::endl;
	}

	return 0;
}