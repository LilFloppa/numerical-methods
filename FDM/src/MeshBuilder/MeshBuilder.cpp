#include "MeshBuilder.h"

void ReadIntervals(std::string filename, std::vector<Interval>& intervals)
{
	std::ifstream in(filename);

	int count;
	in >> count;

	for (int i = 0; i < count; i++)
	{
		Interval interval;
		in >> interval.begin >> interval.end >> interval.n;
		intervals.push_back(interval);
	}
}

void BuildMesh(std::vector<Interval> intervals, int k, double* x, double* hx)
{
	int pos = 0;
	for (auto interval : intervals)
	{
		double begin = interval.begin;
		double end = interval.end;
		int n = interval.n;

		x[pos++] = begin;

		double h = (end - begin) / n;
		for (int i = 1; i < n; i++, pos++)
		{
			double xi = begin + i * h;
			x[pos] = xi;
			hx[pos - 1] = xi - x[pos - 1];
		}
		hx[pos - 1] = end - x[pos - 1];
	}

	x[pos] = intervals.back().end;
	hx[pos - 1] = x[pos] - x[pos - 1];
}