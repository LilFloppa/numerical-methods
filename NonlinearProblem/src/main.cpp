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

	std::ofstream out("matrix.csv");

	for (int i = 0; i < A.N; i++)
		AT[i][i] = A.DI[i];

	for (int i = 0; i < A.N; i++)
	{
		for (int j = A.IA[i]; j < A.IA[i + 1]; j++)
		{
			AT[i][A.JA[j]] = AT[A.JA[j]][i] = A.AL[j];
		}
	}

	for (auto Ai : AT)
	{
		for (auto Aj : Ai)
		{
			out << setprecision(15) << Aj << ";";
		}

		out << std::endl;
	}
}

double u(double x, double t)
{
	return t;
}

int main()
{
	MeshBuilder mesh(10, 0.0, 1.0);
	PortraitBuilder portrait(mesh.GetNodeCount(), mesh.Begin(), mesh.End());

	SLAEInfo info;
	info.begin = mesh.Begin();
	info.end = mesh.End();
	info.nodeCount = mesh.GetNodeCount();
	info.IA = portrait.GetIA();
	info.JA = portrait.GetJA();
	info.JASize = portrait.GetJASize();
	SLAEBuilder slae(&info);

	int n = mesh.GetNodeCount();
	vector<double> q0(n, 0.0);
	FiniteElement e = *mesh.Begin();
	double h = (e.end - e.begin) / 2;

	for (int i = 0; i < n; i++)
		q0[i] = u(i * h, 0.0);

	double* q1 = new double[n] { 0 };

	double eps = 1.0e-7;
	double diff = 1;
	SLAE::Matrix A;
	vector<double>* b;
	


		slae.BuildGlobal(q0, 1.0);
		slae.BuildGlobalB(q0, 1.0);
		//slae.Boundary(u(0.0, 1.0), u((n - 1) * h, 1.0));

		A = slae.GetMatrix();
		b = slae.GetB();

		printMatrix(A);


		for (int i = 0; i < n; i++)
		{
			q0[i] = q1[i];
			q1[i] = 0;
		}

		SLAE::Multiply(A, q0.data(), Aq);
		diff = 0;
		for (int i = 0; i < n; i++)
			diff += (Aq[i] - (*b)[i]) * (Aq[i] - (*b)[i]);

		double norm = 0;
		for (int i = 0; i < n; i++)
			norm += (*b)[i] * (*b)[i];

		diff = sqrt(diff / norm);


	//printMatrix(A);
		//TODO: На каждой итерации нужно пересчитывать матрицу и вектор правой части
		//TODO: Передавая в SLAEBuilder начальное приближение
		//TODO: При сборке матрицы нужно для каждой локальной матрицы пересчитывать коэффициенты лямбда
		//TODO: локальная матрица собирается так: L = G1 * l1 + G2 * l2 + G3 * l3 + sigma * M / dt

	std::cout << std::endl << std::endl;

	for (int i = 0; i < n; i++)
		std::cout << q0[i] << std::endl;

	std::cin.get();
	return 0;
}