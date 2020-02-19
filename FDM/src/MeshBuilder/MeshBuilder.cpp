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

int CountBreakPoints(std::vector<Interval>& intervals)
{
	int k = 0;
	for (auto& interval : intervals)
		k += interval.n;
	k++;

	return k;
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

std::vector<std::vector<Node>> MeshNumbering(double* x, double* y, int kx, int ky)
{
	std::vector<std::vector<Node>> meshNumbering;
	meshNumbering.resize(ky, std::vector<Node>(kx, Node(-1, -1, -1)));

	int num = 0;
	for (int i = 0; i < ky; i++)
	{
		for (int j = 0; j < kx; j++)
		{
			if (y[i] < 0.5 || abs(y[i] - 0.5) / 0.5 < eps)
			{
				meshNumbering[ky - i - 1][j] = Node(i, j, num);
				num++;
			}

			if (y[i] > 0.5 && x[j] < 0.5)
			{
				meshNumbering[ky - i - 1][j] = Node(i, j, num);
				num++;
			}

			if (y[i] > 0.5 && abs(x[i] - 0.5) / 0.5 < eps)
			{
				meshNumbering[ky - i - 1][j] = Node(i, j, num);
				num++;
			}
		}
	}

	return meshNumbering;
}