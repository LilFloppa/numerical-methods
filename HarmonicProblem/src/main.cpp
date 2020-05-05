#include "stdafx.h"
#include "IntervalBuilder.h"
#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"
#include "Matrix.h"

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
		XBuilder.Begin(), XBuilder.End(), XBuilder.NodeCount(),
		YBuilder.Begin(), YBuilder.End(), YBuilder.NodeCount(),
		ZBuilder.Begin(), ZBuilder.End(), ZBuilder.NodeCount()
	);

	// Matrix size
	int n = 2 * XBuilder.NodeCount() * YBuilder.NodeCount() * ZBuilder.NodeCount();

	// Matrix 
	Matrix A(n);

	// Vector
	std::vector<double> b(n);

	// Portrait building
	SparsePortraitBuilder PBuilder(n, MBuilder.Begin(), MBuilder.End());
	PBuilder.Build(A);

	// SLAE building
	SparseSLAEBuilder SLAEBuilder(MBuilder.Begin(), MBuilder.End());
	SLAEBuilder.BuildMatrix(A);
	SLAEBuilder.BuildB(b);

	std::cout << "Hello, World!" << std::endl;
	return 0;
}