using System;

namespace BecomeJPEG
{
    internal class Resolution
    {
        public int width;
        public int height;

        public Resolution(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public static implicit operator Resolution(Tuple<int, int> t)
        {
            return new Resolution(t.Item1, t.Item2);
        }

        public override string ToString()
        {
            return $"{width}x{height}";
        }
    }
}
