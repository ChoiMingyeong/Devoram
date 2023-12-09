using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public struct LongBools
    {
        public static string IndexOutOfRangeExceptionMessage(byte index, int count) => $"index : {index} is bigger than {count}";

        public static implicit operator long(LongBools value) => value._value;

        public static implicit operator LongBools(long value) => new(value);

        public static implicit operator LongBools(bool[] value) => new(value);

        public static implicit operator LongBools([NotNull] List<bool> value) => new(value.ToArray());

        public static LongBools TRUE { get; } = -1;

        public static LongBools FALSE { get; } = 0;

        public static int Count { get; } = sizeof(long) * 8;

        public List<bool> ToList() => ToIEumerable().ToList();

        public bool[] ToArray() => ToIEumerable().ToArray();

        public bool this[byte index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(IndexOutOfRangeExceptionMessage(index, Count));
                }

                return (_value & (1L << index)) != 0;
            }

            set
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(IndexOutOfRangeExceptionMessage(index, Count));
                }

                long flag = 1L << index;
                _value = value ? (_value | flag) : (_value & ~flag);
            }
        }

        public bool this[params byte[] indexes]
        {
            set
            {
                foreach (var index in indexes)
                {
                    if (index >= Count)
                    {
                        throw new IndexOutOfRangeException(IndexOutOfRangeExceptionMessage(index, Count));
                    }

                    this[index] = value;
                }
            }
        }

        private long _value;

        public LongBools(long source = 0L)
        {
            _value = source;
        }

        public LongBools([NotNull] bool[] source)
        {
            int count = Count < source.Length ? Count : source.Length;
            long result = 0L;

            for (int i = 0; i < count; i++)
            {
                long temp = source[i] ? 1L : 0L;
                result |= temp << i;
            }

            _value = result;
        }

        public void Set(LongBools source, byte startIndex) => Set(source.ToIEumerable(), startIndex);
        
        public void Set(long source, byte startIndex) => Set(((LongBools)source).ToIEumerable(), startIndex);

        public void Set([NotNull] IEnumerable<bool> source, byte startIndex = 0)
        {
            LongBools temp = source.ToArray();
            _value |= temp << startIndex;
        }

        private IEnumerable<bool> ToIEumerable()
        {
            bool[] result = new bool[Count];

            for (byte i = 0; i < Count; i++)
            {
                result[i] = this[i];
            }

            return result;
        }
    }
}