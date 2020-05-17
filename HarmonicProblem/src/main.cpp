#include "stdafx.h"
#include "IntervalBuilder.h"
#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"
#include "Boundary.h"
#include "Matrix.h"
#include "SLAESolver/LOS.h"
#include "SLAESolver/LU.h"

#include <iostream>
#include <ctime>
#include <ratio>
#include <chrono>

void IntervalsFromFile(std::string filename, std::vector<RawInterval>& intervals)
{
	std::ifstream in(filename);

	if (in.is_open())
	{
		int count;
		in >> count;

		for (int i = 0; i < count; i++)
		{
			RawInterval interval;
			in >> interval;
			intervals.push_back(interval);
		}
	}
}

int main()
{
	// Intervals reading
	std::vector<RawInterval> X;
	std::vector<RawInterval> Y;
	std::vector<RawInterval> Z;
	IntervalsFromFile("input/X.txt", X);
	IntervalsFromFile("input/Y.txt", Y);
	IntervalsFromFile("input/Z.txt", Z);

	// Intervals building
	IntervalBuilder XBuilder(X), YBuilder(Y), ZBuilder(Z);


	// Mesh building
	MeshBuilder MBuilder(
		XBuilder.Begin(), XBuilder.End(), XBuilder.Count(),
		YBuilder.Begin(), YBuilder.End(), YBuilder.Count(),
		ZBuilder.Begin(), ZBuilder.End(), ZBuilder.Count()
	);

	// Matrix size
	int n = 2 * XBuilder.Count() * YBuilder.Count() * ZBuilder.Count();

	// Matrix 
	ProfileMatrix A(n);
	SparseMatrix B(n);

	// Vector
	std::vector<double> b(n);

	// Portrait building
	PortraitBuilder PBuilder(n, MBuilder.Begin(), MBuilder.End());
	PBuilder.BuildProfile(A);
	PBuilder.BuildSparse(B);

	// SLAE building
	SLAEBuilder SLAEBuilder(MBuilder.Begin(), MBuilder.End());
	SLAEBuilder.BuildProfileMatrix(A);
	SLAEBuilder.BuildSparseMatrix(B);
	SLAEBuilder.BuildB(b);

	// Boundary conditions
	SetBoundary(A, b, XBuilder, YBuilder, ZBuilder);
	SetBoundary(B, b, XBuilder, YBuilder, ZBuilder);

	std::vector<double> LUx, LOSx;

	auto LOSs = std::chrono::high_resolution_clock::now();
	int k = LOS::LOS(B, LOSx, b);
	auto LOSf = std::chrono::high_resolution_clock::now();

	auto LUs = std::chrono::high_resolution_clock::now();
	LU::LU(A, LUx, b);
	auto LUf = std::chrono::high_resolution_clock::now();

	std::chrono::duration<double> LUspan = LUf - LUs;
	std::chrono::duration<double> LOSspan = LOSf - LOSs;

	std::cout << "LU: " << LUspan.count() << std::endl;
	std::cout << "LOS: " << LOSspan.count() << std::endl;

	std::cin.get();
	return 0;
}