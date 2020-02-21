#include "MatrixBuilder.h"

void BuildMatrix(
	Matrix& A,
	vector<double>& b,
	vector<vector<int>>& areas,
	vector<Interval>& intervalsX,
	vector<Interval>& intervalsY,
	vector<double>& x,
	vector<double>& y,
	vector<double>& hx,
	vector<double>& hy,
	int kx, int ky)
{
	for (int iy = 1; iy < ky - 1; iy++)
	{
		int areaJ = IntervalNo(intervalsY, iy);

		for (int ix = 1; ix < kx - 1; ix++)
		{
			int areaI = IntervalNo(intervalsX, ix);

			if (areas[areaI][areaJ] != 0)
			{
				int row = iy * kx + ix;

				A.D[row] = 2.0 / (hx[ix] * hx[ix - 1]) + 2.0 / (hy[iy] * hy[iy - 1]) + gamma;
				A.L1[row - 1] = -2.0 / (hx[ix - 1] * (hx[ix - 1] + hx[ix]));
				A.U1[row] = -2.0 / (hx[ix] * (hx[ix - 1] + hx[ix]));
				A.L2[row - kx] = -2.0 / (hy[iy - 1] * (hy[iy - 1] + hy[iy]));
				A.U2[row] = -2.0 / (hy[iy] * (hy[iy - 1] + hy[iy]));

				b[row] = f(x[ix], y[iy]);
			}
		}
	}

}

void BoundaryConditions(
	Matrix& A, 
	vector<double>& b, 
	vector<double>& x, 
	vector<double>& y, 
	int kx, int ky, 
	int xBorderNode, 
	int yBorderNode)
{
	int lastRowNode = kx * (ky - 1);
	for (int ix = 0; ix < kx; ix++)
	{
		A.D[ix] = 1.0;
		b[ix] = borderFuncs[0](x[ix], y[0]);

		A.D[lastRowNode + ix] = 1.0;
		b[lastRowNode + ix] = borderFuncs[3](x[ix], y[ky - 1]);
	}

	for (int iy = 0; iy < ky; iy++)
	{
		int row = iy * kx;
		A.D[row] = 1.0;
		b[row] = borderFuncs[1](x[0], y[iy]);

		A.D[row + (kx - 1)] = 1.0;
		b[row + (kx - 1)] = borderFuncs[2](x[kx - 1], y[iy]);
	}

	for (int ix = xBorderNode; ix < kx - 1; ix++)
	{
		int row = yBorderNode * kx + ix;
		A.D[row] = 1.0;
		A.L1[row - 1] = A.U1[row] = A.L2[row - kx] = A.U2[row] = 0.0;
		b[row] = borderFuncs[4](x[ix], y[yBorderNode]);
	}

	for (int iy = yBorderNode; iy < ky - 1; iy++)
	{
		int row = xBorderNode + iy * kx;
		A.D[row] = 1.0;
		A.L1[row] = A.U1[row] = A.L2[row - kx] = A.U2[row] = 0.0;
		b[row] = borderFuncs[5](x[xBorderNode], y[iy]);
	}
}

int IntervalNo(vector<Interval>& intervals, int index)
{
	int areaI = 0;
	for (int i = 0; i < intervals.size(); i++)
		if (index <= intervals[i].endN && index >= intervals[i].beginN)
			areaI = i;

	return areaI;
}