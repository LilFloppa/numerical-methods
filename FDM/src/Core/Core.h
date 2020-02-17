#pragma once

#include <string>
#include <fstream>

struct MeshSize
{
	double x0, x1, y0, y1;
	double break_x, break_y;
};

void ReadMeshSize(std::string filename, MeshSize& size);
void BuildMesh(int kx, MeshSize size, double* x, double* hx);