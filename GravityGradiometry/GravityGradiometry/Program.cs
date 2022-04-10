﻿using System;
using System.IO;

namespace GravityGradiometry
{

    class Program
    {
        static double[] GenerateAxis(int n, double begin, double step)
        {
            double[] axis = new double[n];
            for (int i = 0; i < n; i++)
                axis[i] = begin + step * i;

            return axis;
        }

        static Point[] GenerateReceivers(int n, double begin, double end)
        {
            Point[] receivers = new Point[n];
            double stepX = (end - begin) / (n - 1);

            for (int i = 0; i < n; i++)
                receivers[i] = new Point(begin + i * stepX, 0.0);

            return receivers;
        }

        static void Test2x2()
        {
            const int xCellCount = 2;
            const int zCellCount = 2;
            const int k = xCellCount * zCellCount;
            const int n = 800;

            double xbegin = 2000.0;
            double xstep = 500.0;
            double[] x = GenerateAxis(xCellCount + 1, xbegin, xstep);
            double zbegin = -1000.0;
            double zstep = 250.0;
            double[] z = GenerateAxis(zCellCount + 1, zbegin, zstep);

            double[] p = new double[k];
            double[] gamma = new double[k];
            double alpha = 1.0e-5;

            for (int i = 0; i < k; i++)
                p[i] = 1.0;

            double beginX = -2000;
            double endX = 6000;
            Point[] receivers = GenerateReceivers(n, beginX, endX);

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(x, z, p);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < k; i++)
                p[i] = 1.0;

            ProblemInfo info = new ProblemInfo();
            info.P = p;
            info.X = x;
            info.Z = z;
            info.Receivers = receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(p);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] newP = solver.Solve(A, b);

            f = F.Calculate(newP);

            PrintToCSV("Test2x2.csv", newP, xCellCount, zCellCount);
        }

        static void Test4x4()
        {
            const int xCellCount = 4;
            const int zCellCount = 4;
            const int k = xCellCount * zCellCount;
            const int n = 800;

            double xbegin = 1500.0;
            double xstep = 500.0;
            double[] x = GenerateAxis(xCellCount + 1, xbegin, xstep);
            double zbegin = -1250.0;
            double zstep = 250.0;
            double[] z = GenerateAxis(zCellCount + 1, zbegin, zstep);

            double[] p = new double[k];
            double[] gamma = new double[k];
            double alpha = 0.0;

            double beginX = -2000;
            double endX = 6000;
            Point[] receivers = GenerateReceivers(n, beginX, endX);

            p[5] = 1.0;
            p[6] = 1.0;
            p[9] = 1.0;
            p[10] = 1.0;

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(x, z, p);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < k; i++)
                p[i] = 0.1;

            ProblemInfo info = new ProblemInfo();
            info.P = p;
            info.X = x;
            info.Z = z;
            info.Receivers = receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(p);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] newP = solver.Solve(A, b);

            f = F.Calculate(newP);

            PrintToCSV("Test4x4.csv", newP, xCellCount, zCellCount);
        }

        static void Test9x6()
        {
            const int xCellCount = 9;
            const int zCellCount = 6;
            const int k = xCellCount * zCellCount;
            const int n = 800;

            double xbegin = 0;
            double xstep = 500.0;
            double[] x = GenerateAxis(xCellCount + 1, xbegin, xstep);
            double zbegin = -1500.0;
            double zstep = 250.0;
            double[] z = GenerateAxis(zCellCount + 1, zbegin, zstep);

            double[] p = new double[k];
            double[] gamma = new double[k];
            double alpha = 1.0e-5;

            double beginX = -2000;
            double endX = 6000;
            Point[] receivers = GenerateReceivers(n, beginX, endX);

            p[23] = 1.0;
            p[24] = 1.0;
            p[32] = 1.0;
            p[33] = 1.0;

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(x, z, p);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < k; i++)
                p[i] = 0.1;

            ProblemInfo info = new ProblemInfo();
            info.P = p;
            info.X = x;
            info.Z = z;
            info.Receivers = receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(p);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] newP = solver.Solve(A, b);

            f = F.Calculate(newP);

            PrintToCSV("Test9x6.csv", newP, xCellCount, zCellCount);
        }

        static void Test18x12()
        {
            const int xCellCount = 18;
            const int zCellCount = 12;
            const int k = xCellCount * zCellCount;
            const int n = 800;

            double xbegin = 0;
            double xstep = 250.0;
            double[] x = GenerateAxis(xCellCount + 1, xbegin, xstep);
            double zbegin = -1500.0;
            double zstep = 125.0;
            double[] z = GenerateAxis(zCellCount + 1, zbegin, zstep);

            double[] p = new double[k];
            double[] gamma = new double[k];
            double alpha = 1.0e-5;

            double beginX = -2000;
            double endX = 6000;
            Point[] receivers = GenerateReceivers(n, beginX, endX);

            p[23] = 1.0;
            p[24] = 1.0;
            p[32] = 1.0;
            p[33] = 1.0;

            double[] realG = new double[n];
            GravityCalculator calc = new GravityCalculator(x, z, p);
            for (int i = 0; i < n; i++)
                realG[i] = calc.CalculateFromAll(receivers[i]);

            for (int i = 0; i < k; i++)
                p[i] = 0.1;

            ProblemInfo info = new ProblemInfo();
            info.P = p;
            info.X = x;
            info.Z = z;
            info.Receivers = receivers;
            info.realG = realG;
            info.Alpha = alpha;
            info.Gamma = gamma;

            Functional F = new Functional(info);

            double f = F.Calculate(p);

            SlaeBuilder builder = new SlaeBuilder(info);
            IMatrix A = new FullSparseMatrix(k);
            double[] b = new double[k];

            builder.Build(A, b);

            ISolver solver = new LOSLU();
            double[] newP = solver.Solve(A, b);

            f = F.Calculate(newP);

            PrintToCSV("Test18x12.csv", newP, xCellCount, zCellCount);
        }

        static void Main(string[] args)
        {
            Test18x12();
        }

        static void PrintToCSV(string filename, double[] p, int width, int height)
        {
            double[,] result = new double[width, height];
  
            for (int z = 0; z < height; z++)
                for (int x = 0; x < width; x++)
                {
                    int k = z * width + x;
                    result[x, z] = p[k];
                }

            using (var w = new StreamWriter(File.Open("C:/repos/" + filename, FileMode.Create)))
            {
                for (int j = height - 1; j >= 0; j--)
                {
                    for (int i = 0; i < width; i++)
                    {
                        w.Write($"{result[i, j]};");
                    }

                    w.WriteLine();
                }
            }
        }
    }
}
