#include <vector>
#include <iostream>
#include <iomanip>
#include <fstream>

#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"
#include "TimeMeshBuilder.h"
#include "CSV.h"
#include "Matrix.h"
#include "SLAESolver/SLAESolver.h"

#include "NewtonSLAEBuilder.h"

#include <memory>

std::vector<double> _ = { 0.0, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0 };
std::vector<double> __ = { 0.0, 4.0156862745098039, 6.0235294117647058, 7.0274509803921568, 7.5294117647058822, 7.7803921568627450, 7.9058823529411768, 7.9686274509803923, 8.0 };

std::vector<double> _t = { 0.0, 0.5, 1.0, 1.5, 2.0 };
std::vector<double> __t = { 0.0, 1.0666666666666667, 1.6000000000000001, 1.8666666666666667, 2.0 };

double u(double x, double t)
{
	return x * x;
}

int main()
{
	// Build Mesh
	std::vector<Interval> intervals;
	std::ifstream in("input/intervals.txt");
	int count;
	in >> count;
	for (int i = 0; i < count; i++)
	{
		Interval interval;
		in >> interval;
		intervals.push_back(interval);
	}
	in.close();

	MeshBuilder mesh(intervals);

	// Build Time Mesh
	std::vector<Interval> time_intervals;
	in.open("input/time_intervals.txt");
	int time_count;
	in >> time_count;
	for (int i = 0; i < time_count; i++)
	{
		Interval interval;
		in >> interval;
		time_intervals.push_back(interval);
	}
	in.close();
	TimeMeshBuilder time(time_intervals);

	// Matrix
	Matrix A(mesh.GetNodeCount());

	// Build Matrix Portrait
	PortraitBuilder portrait(mesh.GetNodeCount(), mesh.Begin(), mesh.End());
	portrait.Build(A);
	A.AL.resize(A.IA.back());
	A.AU.resize(A.IA.back());

	NewtonSLAEBuilder newton(mesh.GetNodeCount(), mesh.Begin(), mesh.End());
	SLAEBuilder slae(mesh.GetNodeCount(), mesh.Begin(), mesh.End());

	// Preparing for fixed-point iteration
	int n = mesh.GetNodeCount();
	vector<double> x, q0, q1(n, 0);

	// Find x coords
	FEIterator begin = mesh.Begin();
	FEIterator end = mesh.End();

	for (FEIterator i = begin; i != end; i++)
	{
		x.push_back(i->begin);
		x.push_back((i->begin + i->end) / 2);
	}
	x.push_back((end - 1)->end);

	// Find q0 
	TimeIterator time_begin = time.Begin();
	TimeIterator time_end = time.End();

	for (int i = 0; i < n; i++)
		q0.push_back(u(x[i] + 0.123, *time_begin));

	// Start fixed-point iteration
	vector<double> b(n);
	vector<double> Aq(n);

	double eps = 10e-12;
	double delta = 10e-12;

	for (TimeIterator i = time_begin + 1; i != time_end; i++)
	{
		double t = *i;
		double tPrev = *(i - 1);
		double dt = t - tPrev;

		double diff = 1;
		double diff1 = 1;

		double u0 = u(x[0], t);
		double un = u(x[n - 1], t);

		int k = 0;
		while (diff >= eps && diff1 >= delta)
		{
			newton.BuildGlobal(A, q0, dt);
			newton.BuildGlobalB(b, q0, t, dt);
			newton.Boundary(A, b, u0, un);

			LU(A, q1, b);

			A.Clear();
			fill(b.begin(), b.end(), 0.0);

			slae.BuildGlobal(A, q1, dt);
			slae.BuildGlobalB(b, q0, t, dt);

			Multiply(A, q1, Aq);

			Aq[0] = b[0] = u0;
			Aq[n - 1] = b[n - 1] = un;

			diff = 0;
			double norm = 0;
			for (int i = 0; i < n; i++)
			{
				diff += (Aq[i] - b[i]) * (Aq[i] - b[i]);
				norm += b[i] * b[i];
			}
			diff = sqrt(diff / norm);

			diff1 = 0;
			double norm1 = 0;
			for (int i = 0; i < n; i++)
			{
				diff1 += (q1[i] - q0[i]) * (q1[i] - q0[i]);
				norm1 += q1[i] * q1[i];
			}
			diff1 = sqrt(diff1 / norm1);

			q0 = q1;

			A.Clear();
			fill(b.begin(), b.end(), 0.0);

			k++;
		}

		cout << "iterations: " << k << endl;
	}

	cout << endl << endl;
	for (auto qi : q1)
	{
		cout << qi << endl;
	}

	cin.get();
	return 0;
}