﻿namespace UI.Models
{
    public class Axis : ICloneable
    {
        public double Begin;
        public double End;
        public int CellCount;

        public double[]? Build()
        {
            if (Begin >= End)
                return null;

            if (CellCount <= 0)
                return null;

            double[] result = new double[CellCount + 1];
            double step = (End - Begin) / CellCount;

            for (int i = 0; i < CellCount + 1; i++)
                result[i] = Begin + i * step;

            return result;
        }

        public object Clone()
        {
            Axis clone = new();
            clone.Begin = Begin;
            clone.End = End;
            clone.CellCount = CellCount;
            return clone;
        }
    }
}