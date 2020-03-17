#include "Matrix.h"
#include <cmath>

namespace SLAE
{
	const int maxiter = 50000;
	const double eps = 1.0e-9;
	int LOS_LU(Matrix& A, double* x, double* f, Matrix& LU, AuxVectors& aux)
	{
		double* Ax = aux.Ax;
		double* r = aux.r;
		double* z = aux.z;
		double* p = aux.p;
		double* temp = aux.temp;
		int N = A.N;

		// Calculate r0
		Multiply(A, x, Ax);
		for (int i = 0; i < N; i++)
			r[i] = f[i] - Ax[i];
		Forward(LU, r, r, false);

		//Calculate z0
		Backward(LU, z, r, false);

		// Calculate p0
		Multiply(A, z, p);
		Forward(LU, p, p, false);

		double diff = DotProduct(N, r, r);

		int k = 0;
		for (; k < maxiter && diff >= eps; k++)
		{
			// Calculate alpha
			double dotP = DotProduct(N, p, p);
			double a = DotProduct(N, p, r) / dotP;

			// Calculate xk, rk
			for (int i = 0; i < N; i++)
			{
				x[i] += a * z[i];
				r[i] -= a * p[i];
			}

			// Calculate beta
			Backward(LU, Ax, r, false);
			Multiply(A, Ax, temp);
			Forward(LU, Ax, temp, false);
			double b = -DotProduct(N, p, Ax) / dotP;

			// Calculate zk, pk
			Backward(LU, temp, r, false);
			for (int i = 0; i < N; i++)
			{
				z[i] = temp[i] + b * z[i];
				p[i] = Ax[i] + b * p[i];
			}

			// Calculate difference
			diff = DotProduct(N, r, r);
		}

		return k;
	}
}