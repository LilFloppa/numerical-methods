#include "stdafx.h"
#include "IntervalBuilder.h"
#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"
#include "Matrix.h"
#include "SLAESolver/LOS.h"

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
	SparseMatrix A(n);

	// Vector
	std::vector<double> b(n);

	// Portrait building
	SparsePortraitBuilder PBuilder(n, MBuilder.Begin(), MBuilder.End());
	PBuilder.Build(A);

	// SLAE building
	SparseSLAEBuilder SLAEBuilder(MBuilder.Begin(), MBuilder.End());
	SLAEBuilder.BuildMatrix(A);
	SLAEBuilder.BuildB(b);

	int xCount = XBuilder.Count(), yCount = YBuilder.Count(), zCount = ZBuilder.Count();

	// Boundary conditions
	for (int i = 0; i < yCount; i++)
	{
		for (int j = 0; j < xCount; j++)
		{
			int line = i * xCount + j;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder[j], YBuilder[i], ZBuilder[0]);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder[j], YBuilder[i], ZBuilder[0]);
		}
	}

	for (int i = 0; i < zCount; i++)
	{
		for (int j = 0; j < xCount; j++)
		{
			int line = i * yCount * xCount + j;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder[j], YBuilder[0], ZBuilder[i]);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder[j], YBuilder[0], ZBuilder[i]);
		}
	}

	for (int i = 0; i < zCount; i++)
	{
		for (int j = 0; j < yCount; j++)
		{
			int line = i * yCount * xCount + j * xCount + xCount - 1;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder.Back().value, YBuilder[j], ZBuilder[i]);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder.Back().value, YBuilder[j], ZBuilder[i]);
		}
	}

	for (int i = 0; i < zCount; i++)
	{
		for (int j = 0; j < xCount; j++)
		{
			int line = i * yCount * xCount + (yCount - 1) * xCount + j;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder[j], YBuilder.Back().value, ZBuilder[i]);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder[j], YBuilder.Back().value, ZBuilder[i]);
		}
	}

	for (int i = 0; i < zCount; i++)
	{
		for (int j = 0; j < yCount; j++)
		{
			int line = i * yCount * xCount + j * xCount;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder[0], YBuilder[j], ZBuilder[i]);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder[0], YBuilder[j], ZBuilder[i]);
		}
	}

	for (int i = 0; i < yCount; i++)
	{
		for (int j = 0; j < xCount; j++)
		{
			int line = (zCount - 1) * xCount * yCount + i * xCount + j;

			A(2 * line, 2 * line) = 1.0e+50;
			b[2 * line] = 1.0e+50 * usin(XBuilder[j], YBuilder[i], ZBuilder.Back().value);

			A(2 * line + 1, 2 * line + 1) = 1.0e+50;
			b[2 * line + 1] = 1.0e+50 * ucos(XBuilder[j], YBuilder[i], ZBuilder.Back().value);
		}
	}

	std::vector<double> x;
	int k = LOS(A, x, b);
	
	std::cout << "Hello, World!" << std::endl;
	return 0;
}