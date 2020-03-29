#pragma once

#include <SLAE/Matrix.h>
#include <vector>
#include <iostream>

#include "FEMInfo.h"

using namespace std;

class SLAEBuilder
{
public:
	SLAEBuilder(SLAEInfo* info)
	{
		begin = info->begin;
		end = info->end;
		nodeCount = info->nodeCount;
		JASize = info->JASize;
		IA = info->IA;
		JA = info->JA;

		A.N = nodeCount;
		A.DI = new double[nodeCount];
		A.AL = new double[JASize];
		A.AU = new double[JASize];
		A.IA = IA;
		A.JA = JA;

		for (int i = 0; i < nodeCount; i++)
			A.DI[i] = 0.0;

		for (int i = 0; i < JASize; i++)
			A.AL[i] = A.AU[i] = 0.0;

		b.resize(nodeCount);
	}

	void BuildGlobal(vector<double>& q, double dt)
	{
		for (FEIterator iter = begin; iter != end; iter++)
		{
			vector<vector<double>> local = buildLocal(*iter , q, dt);
			AddLocalToGlobal(*iter, local);
		}
	}

	void BuildGlobalB(vector<double>& q, double dt)
	{
		for (FEIterator iter = begin; iter != end; iter++)
		{
			vector<double> local = buildLocalB(*iter, q, dt);
			AddLocalBToGlobalB(*iter, local);
		}
	}

	void Boundary(double u1, double u2)
	{
		A.DI[0] = 10e+50;
		b[0] = u1 * 10e+50;

		A.DI[nodeCount - 1] = 10e+50;
		b[nodeCount - 1] = u2 * 10e+50;
	}

	SLAE::Matrix GetMatrix() { return A; }
	vector<double>* GetB() { return &b; }

private:
	FEIterator begin, end;

	SLAE::Matrix A;
	vector<double> b;

	int nodeCount, JASize;
	int* IA;
	int* JA;

	vector<vector<double>> buildLocal(FiniteElement e, vector<double>& qGlobal, double dt)
	{
		vector<vector<double>> local(3, vector<double>(3, 0));
		vector<double> q;
		double h = e.end - e.begin;

		for (auto v : e.v)
			q.push_back(qGlobal[v]);

		double l1 = lambda(e.begin, e.begin, h, q);
		double l2 = lambda(e.begin, (e.begin + e.end) / 2, h, q);
		double l3 = lambda(e.begin, e.end, h, q);

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				local[i][j] = (l1 * G1[i][j] + l2 * G2[i][j]+ l3 * G3[i][j]) / h + h * sigma * M[i][j] / dt;

		return local;
	}

	vector<double> buildLocalB(FiniteElement e, vector<double>& qGlobal, double dt)
	{
		vector<double> f, q;
		f.push_back(F(e.begin));
		f.push_back(F((e.begin + e.end) / 2));
		f.push_back(F(e.end));

		for (auto v : e.v)
			q.push_back(qGlobal[v]);

		vector<double> b(3, 0.0);
		double h = e.end - e.begin;

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				b[i] += h * (f[j] * M[i][j] + sigma * q[j] * M[i][j] / dt);

		return b;
	}

	void AddLocalToGlobal(FiniteElement e, vector<vector<double>> local)
	{
		for (int i = 0; i < 3; i++)
			A.DI[e.v[i]] += local[i][i];

		for (int i = 0; i < 3; i++)
		{
			for (int j = i + 1; j < 3; j++)
			{
				int begin = IA[e.v[j]];
				int end = IA[e.v[j] + 1];
				int index = -1;
				for (int k = begin; k < end; k++)
					if (e.v[i] == JA[k]) 
						index = k;

				if (index != -1)
				{
					A.AL[index] += local[i][j];
					A.AU[index] += local[i][j];
				}
			}
		}
	}

	void AddLocalBToGlobalB(FiniteElement e, vector<double> local)
	{
		for (int i = 0; i < 3; i++)
			b[e.v[i]] += local[i];
	}

	double lambda(double b, double x, double h, vector<double> q)
	{
		double du1 = basisDirs[0](x - b, h) * q[0];
		double du2 = basisDirs[1](x - b, h) * q[1];
		double du3 = basisDirs[2](x - b, h) * q[2];
		return Lamdba(x, du1 + du2 + du3);
	}
};