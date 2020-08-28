#pragma once

#include <vector>
#include <iostream>

#include "FEMInfo.h"
#include "Matrix.h"

using namespace std;


/// <summary>
///  1. Все зависит от лямбды.
///  2. Выясняем какие у нас l1, l2, l3. Если lambda(q, x) = (du / dx)^2, то l1 = lambda(q, x1) = (q1 * dpsi1(x1) / dx + q2 * dpsi2(x1) / dx + q3 * dpsi3(x1) / dx)^2
///  3. Нужно так же записать dl1, dl2, dl3.
/// </summary>


std::vector<std::function<double(double)>> p =
{
	[](double x) { return 2 * (x - 1) * (x - 0.5); },
	[](double x) { return 4 * x * (1 - x); },
	[](double x) { return 2 * x * (x - 0.5); }
};

vector<function<double(double)>> dp =
{
	[](double x) { return 4 * x - 3; },
	[](double x) { return 4 - 8 * x; },
	[](double x) { return 4 * x - 1; }
};


//function<double(double)> l1 = [x1] ()
//function<double(double)> l2 = [x2] ()
//function<double(double)> l3 = [x3] ()

//function<double(double)>

class NewtonSLAEBuilder
{
public:
	NewtonSLAEBuilder(int nodeCount, FEIterator begin, FEIterator end) : nodeCount(nodeCount), begin(begin), end(end)
	{ }

	void BuildGlobal(Matrix& A, vector<double>& q0, double dt)
	{
		for (FEIterator iter = begin; iter != end; iter++)
		{
			FiniteElement e = *iter;
			vector<vector<double>> local = buildLocal(e, q0, dt);

			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					A(e.v[i], e.v[j]) += local[i][j];
		}
	}

	void BuildGlobalB(vector<double>& b, vector<double>& q0, double t, double dt)
	{
		for (FEIterator iter = begin; iter != end; iter++)
		{
			FiniteElement e = *iter;
			vector<double> local = buildLocalB(e, q0, t, dt);

			for (int i = 0; i < 3; i++)
				b[e.v[i]] += local[i];
		}
	}

	void Boundary(Matrix& A, vector<double>& b, double u1, double u2)
	{
		A(0, 0) = 1.0e+50;
		b[0] = u1 * 1.0e+50;

		A(nodeCount - 1, nodeCount - 1) = 1.0e+50;
		b[nodeCount - 1] = u2 * 1.0e+50;
	}

private:
	FEIterator begin, end;
	int nodeCount;

	vector<vector<double>> buildLocal(FiniteElement e, vector<double>& q0, double dt)
	{
		vector<vector<double>> local(3, vector<double>(3, 0));
		vector<double> q;
		double h = e.end - e.begin;

		for (auto v : e.v)
			q.push_back(q0[v]);

		double x1 = e.begin;
		double x2 = (e.begin + e.end) / 2;
		double x3 = e.end;

		double l1 = lambda(x1, x1, h, q);
		double l2 = lambda(x1, x2, h, q);
		double l3 = lambda(x1, x3, h, q);

		// A(q0)
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				local[i][j] = (l1 * G1[i][j] + l2 * G2[i][j] + l3 * G3[i][j]) / h  + h * sigma * M[i][j] / dt;


		double dp11 = dp[0]((x1 - x1) / h) / h;
		double dp12 = dp[1]((x1 - x1) / h) / h;
		double dp13 = dp[2]((x1 - x1) / h) / h;

		double dp21 = dp[0]((x2 - x1) / h) / h;
		double dp22 = dp[1]((x2 - x1) / h) / h;
		double dp23 = dp[2]((x2 - x1) / h) / h;

		double dp31 = dp[0]((x3 - x1) / h) / h;
		double dp32 = dp[1]((x3 - x1) / h) / h;
		double dp33 = dp[2]((x3 - x1) / h) / h;

		//double dl11 = 2 * (q[0] * dp11 + q[1] * dp12 + q[2] * dp13) * dp11;	// dl1 / dq1
		//double dl12 = 2 * (q[0] * dp11 + q[1] * dp12 + q[2] * dp13) * dp12;	// dl1 / dq2
		//double dl13 = 2 * (q[0] * dp11 + q[1] * dp12 + q[2] * dp13) * dp13;	// dl1 / dq3

		//double dl21 = 2 * (q[0] * dp21 + q[1] * dp22 + q[2] * dp23) * dp21;	// dl2 / dq1
		//double dl22 = 2 * (q[0] * dp21 + q[1] * dp22 + q[2] * dp23) * dp22;	// dl2 / dq2
		//double dl23 = 2 * (q[0] * dp21 + q[1] * dp22 + q[2] * dp23) * dp23;	// dl2 / dq3

		//double dl31 = 2 * (q[0] * dp31 + q[1] * dp32 + q[2] * dp33) * dp31;	// dl3 / dq1
		//double dl32 = 2 * (q[0] * dp31 + q[1] * dp32 + q[2] * dp33) * dp32;	// dl3 / dq2
		//double dl33 = 2 * (q[0] * dp31 + q[1] * dp32 + q[2] * dp33) * dp33;	// dl3 / dq3 

		double dl11 = dp11;		// dl1 / dq1
		double dl12 = dp12;		// dl1 / dq2
		double dl13 = dp13;		// dl1 / dq3

		double dl21 = dp21;		// dl2 / dq1
		double dl22 = dp22;		// dl2 / dq2
		double dl23 = dp23;		// dl2 / dq3

		double dl31 = dp31;		// dl3 / dq1
		double dl32 = dp32;		// dl3 / dq2
		double dl33 = dp33;		// dl3 / dq3

		vector<double> dl1dq = { dl11, dl12, dl13 };
		vector<double> dl2dq = { dl21, dl22, dl23 };
		vector<double> dl3dq = { dl31, dl32, dl33 };

		vector<vector<double>> dldq = { dl1dq, dl2dq, dl3dq };

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
			{
				local[i][j] +=
					(dldq[0][j] * G1[i][0] + dldq[1][j] * G2[i][0] + dldq[2][j] * G3[i][0]) * q[0] +
					(dldq[0][j] * G1[i][1] + dldq[1][j] * G2[i][1] + dldq[2][j] * G3[i][1]) * q[1] +
					(dldq[0][j] * G1[i][2] + dldq[1][j] * G2[i][2] + dldq[2][j] * G3[i][2]) * q[2];
			}

		return local;
	}

	vector<double> buildLocalB(FiniteElement e, vector<double>& q0, double t, double dt)
	{
		vector<double> f, q;
		f.push_back(F(e.begin, t));
		f.push_back(F((e.begin + e.end) / 2, t));
		f.push_back(F(e.end, t));

		double x1 = e.begin;
		double x2 = (e.begin + e.end) / 2;
		double x3 = e.end;

		for (auto v : e.v)
			q.push_back(q0[v]);

		vector<double> b(3, 0.0);
		double h = e.end - e.begin;

		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				b[i] += h * (f[j] * M[i][j] + sigma * q[j] * M[i][j] / dt);


		double dp11 = dp[0]((x1 - x1) / h) / h;
		double dp12 = dp[1]((x1 - x1) / h) / h;
		double dp13 = dp[2]((x1 - x1) / h) / h;

		double dp21 = dp[0]((x2 - x1) / h) / h;
		double dp22 = dp[1]((x2 - x1) / h) / h;
		double dp23 = dp[2]((x2 - x1) / h) / h;

		double dp31 = dp[0]((x3 - x1) / h) / h;
		double dp32 = dp[1]((x3 - x1) / h) / h;
		double dp33 = dp[2]((x3 - x1) / h) / h;

		double dl11 = dp11;		// dl1 / dq1
		double dl12 = dp12;		// dl1 / dq2
		double dl13 = dp13;		// dl1 / dq3

		double dl21 = dp21;		// dl2 / dq1
		double dl22 = dp22;		// dl2 / dq2
		double dl23 = dp23;		// dl2 / dq3

		double dl31 = dp31;		// dl3 / dq1
		double dl32 = dp32;		// dl3 / dq2
		double dl33 = dp33;		// dl3 / dq3

		vector<double> dl1dq = { dl11, dl12, dl13 };
		vector<double> dl2dq = { dl21, dl22, dl23 };
		vector<double> dl3dq = { dl31, dl32, dl33 };

		vector<vector<double>> dldq = { dl1dq, dl2dq, dl3dq };

		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				b[i] += q[j] *
					(
						(dldq[0][0] * G1[i][j] + dldq[1][0] * G2[i][j] + dldq[2][0] * G3[i][j]) * q[0] +
						(dldq[0][1] * G1[i][j] + dldq[1][1] * G2[i][j] + dldq[2][1] * G3[i][j]) * q[1] +
						(dldq[0][2] * G1[i][j] + dldq[1][2] * G2[i][j] + dldq[2][2] * G3[i][j]) * q[2]
						);
			}
		}

		return b;
	}

	double lambda(double a, double x, double h, vector<double> q)
	{
		double du1 = basisDirs[0](x - a, h) * q[0];
		double du2 = basisDirs[1](x - a, h) * q[1];
		double du3 = basisDirs[2](x - a, h) * q[2];

		return Lamdba(x, du1 + du2 + du3);
	}
};