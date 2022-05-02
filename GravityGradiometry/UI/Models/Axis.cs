namespace UI.Models
{
    public class Axis : ICloneable
    {
        public double Begin;
        public double End;
        public int CellCount;

        public bool Validate() => Begin < End && CellCount > 0;

        public double[] Build()
        {
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

        public int GetLabel(int i) => (int)(Begin + i * (End - Begin) / CellCount);
    }
}
