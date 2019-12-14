#include "Core.h"
#include <fstream>
#include <vector>
#include <string>

using namespace std;

struct Point
{
	Point(double x, double y) : x(x), y(y) {  }
	double x, y;
};

vector<Point> points;

void WritePoints(string filename)
{
	ofstream out(filename);
	out << points.size() << endl;
	for (auto point : points)
		out << point.x << "\t" << point.y << endl;

	out.close();
}

void ReadInitialValues(double* x0, double& eps, int& maxiter, double& delta)
{
	ifstream in("values.txt");
	for (int i = 0; i < N; i++)
		in >> x0[i];

	in >> eps >> maxiter >> delta;

	in.close();
}

void Newton(double** A, double* F, double* x0, double eps, int maxiter, double delta)
{
	double* dx = new double[N];
	CalculateF(F, x0);
	points.emplace_back(x0[0], x0[1]);
	printf_s("x: %f\ty: %f\tbeta: %f\n", x0[0], x0[1], 1.0);

	for (int i = 0; i < maxiter && Norm(F) >= eps; i++)
	{
		CalculateF(F, x0);
		for (int i = 0; i < N; i++)
			F[i] = -F[i];

		CalculateMatrix(A, x0);
		Solve(A, dx, F);

		double norm = Norm(F);
		double beta = 1;

		/*CalculateF(F, x0, dx, beta);
		while (Norm(F) >= norm && beta >= delta)
		{
			beta /= 2;
			CalculateF(F, x0, dx, beta);
		} */

		for (int i = 0; i < N; i++)
			x0[i] += dx[i];

		points.emplace_back(x0[0], x0[1]);
		printf_s("x: %f\ty: %f\tbeta: %f\n", x0[0], x0[1], beta);
	}
}

void main()
{
	double* x0 = new double[N];
	double** A = new double*[N];
	for (int i = 0; i < N; i++)
		A[i] = new double[N];

	double* F = new double[N];
	double eps = 0, delta = 0;
	int maxiter = 0;
	ReadInitialValues(x0, eps, maxiter, delta);
	Newton(A, F, x0, eps, maxiter, delta);

	WritePoints("C:/input/points.txt");
}