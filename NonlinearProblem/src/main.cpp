#include <vector>

#include <SLAE/Matrix.h>
#include <SLAE/LOS.h>

#include "MeshBuilding/MeshBuilding.h"
#include "FEM/Core.h"

int main()
{
	int n = 0;
	double begin = 0.0, end = 0.0;
	ReadMeshData("input/MeshData.txt", n, begin, end);

	// Построение сетки
	std::vector<double> mesh;
	std::vector<FiniteElement> elements;
	BuildMesh(n, begin, end, mesh, elements);

	// Инициализация матрицы и построение портрета
	int size = mesh.size();
	SLAE::Matrix global;
	int ALsize = MatrixALSize(size);
	InitMatrix(global, size, ALsize);
	BuildPortrait(global);

	return 0;
}