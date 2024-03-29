﻿using GravityGradiometry;

namespace UI.Models
{
    public class ReceiversInfo : ICloneable
    {
        public double BeginX;
        public double EndX;
        public int Count;

        public object Clone()
        {
            ReceiversInfo clone = new();
            clone.BeginX = BeginX;
            clone.EndX = EndX;
            clone.Count = Count;
            return clone;
        }

        public bool Validate() => Count > 0 && BeginX < EndX;

        public Point[] BuildReceivers()
        {
            Point[] receivers = new Point[Count];

            double stepX = (EndX - BeginX) / (Count - 1);

            for (int i = 0; i < Count; i++)
                receivers[i] = new Point(BeginX + i * stepX, 0.0);

            return receivers;
        }
    }
}
