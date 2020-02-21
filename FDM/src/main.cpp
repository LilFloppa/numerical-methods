#include "MeshBuilder/MeshBuilder.h"
#include "MatrixBuilder/MatrixBuilder.h"
#include "MatrixBuilder/Matrix.h"

#include <SLAE/SLAESolver.h>

#include <iostream>
#include <vector>

using namespace std;

int main()
{
	vector<Interval> intervalsX, intervalsY;
	ReadIntervals("input/intervalsX.txt", intervalsX);
	ReadIntervals("input/intervalsY.txt", intervalsY);

	vector<vector<int>> areas;
	ReadAreaMatrix("input/areaMatrix.txt", areas);

	int kx = CountBreakPoints(intervalsX);
	int ky = CountBreakPoints(intervalsY);

	IntervalNumbering(intervalsX);
	IntervalNumbering(intervalsY);

	vector<double> x(kx), y(ky);
	vector<double> hx(kx - 1), hy(ky - 1);
	BuildMesh(intervalsX, kx, x, hx);
	BuildMesh(intervalsY, ky, y, hy);

	Matrix A;
	vector<double> b(kx * ky);
	InitMatrix(A, kx * ky, kx);
	BuildMatrix(A, b, areas, intervalsX, intervalsY, x, y, hx, hy, kx, ky);
	BoundaryConditions(A, b, x, y, kx, ky, intervalsX[0].endN, intervalsY[0].endN);

	//PrintSLAE(A, b);

	SLAESolver::Matrix m;
	m.m = A.m;
	m.N = A.N;
	m.D = A.D.data();
	m.L1 = A.L1.data();
	m.U1 = A.U1.data();
	m.L2 = A.L2.data();
	m.U2 = A.U2.data();
	
	double* bb = b.data();
	double* x0 = new double[b.size()];
	double* r = new double[b.size()];

	for (int i = 0; i < b.size(); i++)
		x0[i] = 0.0;

	int k = SLAESolver::Zeidel(m, bb, x0, r, 1.7);
	return 0;
}