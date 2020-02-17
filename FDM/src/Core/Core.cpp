#include "Core.h"

void ReadMeshSize(std::string filename, MeshSize& size)
{
	std::ifstream in(filename);
	
	in >> size.x0 >> size.x1 >> size.break_x;
	in >> size.y0 >> size.y1; size.break_y;
	
	in.close();
}

void BuildMesh(int kx, MeshSize size, double* x,  double* hx)
{
	double h = (size.x1 - size.x0) / kx;
	double break_x = size.break_x;
	double break_y = size.break_y;

	double x0 = size.x0;
	x[0] = size.x0;

	for (int i = 1; i < kx - 1; i++)
	{
		double xi = x0 + i * h;

		if (abs(xi - break_x) < h)
		{
			hx[i - 1] = break_x - x[i - 1];
			x[i] = break_x;
			x0 =+ abs(xi - break_x);
		}
		else
		{
			x[i] = xi;
			hx[i - 1] = xi - x[i - 1];
		}
	}

	x[kx - 1] = size.x1;
	hx[kx - 2] = x[kx - 1] - x[kx - 2];
}