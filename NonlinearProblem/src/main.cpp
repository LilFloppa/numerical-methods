#include <vector>
#include <iostream>
#include <iomanip>
#include <fstream>

#include "MeshBuilder.h"
#include "PortraitBuilder.h"
#include "SLAEBuilder.h"

#include <SLAE/SLAE.h>

void printMatrix(SLAE::Matrix A)
{
	std::vector<std::vector<double>> AT(A.N, std::vector<double>(A.N, 0.0));
	SLAE::ToTight(A, AT);

	for (auto Ai : AT)
	{
		for (auto Aj : Ai)
			std::cout << setprecision(15) << Aj << ";";

		std::cout << std::endl;
	}
}

double u(double x, double t)
{
	return x * x + 2 * t;
}

int main()
{
	MeshBuilder mesh(3, 0.0, 6.0);
	PortraitBuilder portrait(mesh.GetNodeCount(), mesh.Begin(), mesh.End());

	SLAEInfo info;
	info.begin = mesh.Begin();
	info.end = mesh.End();
	info.nodeCount = mesh.GetNodeCount();
	info.IA = portrait.GetIA();
	info.JASize = portrait.GetJASize();
	SLAEBuilder slae(&info);

	int n = mesh.GetNodeCount();
	vector<double> q0(n, 0.0);
	FiniteElement e = *mesh.Begin();
	double h = (e.end - e.begin) / 2;

	for (int i = 0; i < n; i++)
		q0[i] = u(i * h, 0.0);

	double eps = 1.0e-7;
	double diff = 1;
	SLAE::Matrix A;
	vector<double>* b;
	vector<double> Aq(n);

	double t0 = 0.0, t1 = 1.0;

	while (diff >= eps)
	{
		slae.BuildGlobal(q0, t1 - t0);
		slae.BuildGlobalB(q0, t1 - t0);
		slae.Boundary(u(0.0, t1 - t0), u((n - 1) * h, t1 - t0));

		A = slae.GetMatrix();
		b = slae.GetB();
		vector<double> q1 = *b;

		SLAE::LUDecomposition(A);
		SLAE::Solve(A, q1);

		for (int i = 0; i < n; i++)
			q0[i] = q1[i];

		SLAE::Multiply(A, q0, Aq);
		diff = 0;
		for (int i = 0; i < n; i++)
			diff += (Aq[i] - (*b)[i]) * (Aq[i] - (*b)[i]);

		double norm = 0;
		for (int i = 0; i < n; i++)
			norm += (*b)[i] * (*b)[i];

		diff = sqrt(diff / norm);

		std::cout << diff << std::endl;
	}

	std::cout << std::endl << std::endl;

	for (int i = 0; i < n; i++)
		std::cout << q0[i] << std::endl;

	std::cin.get();
	return 0;
}