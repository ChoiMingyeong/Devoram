using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public struct IntBools
    {
        public static string IndexOutOfRangeExceptionMessage(byte index, int count) => $"index : {index} is bigger than {count}";

        public static implicit operator int([NotNull] IntBools value) => value._value;

        public static implicit operator IntBools(int value) => new(value);

        public static implicit operator IntBools([NotNull] bool[] value) => new(value);

        public static implicit operator IntBools([NotNull] List<bool> value) => new(value.ToArray());

        public static IntBools TRUE { get; } = -1;

        public static IntBools FALSE { get; } = 0;

        public static int Count { get; } = sizeof(int) * 8;

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

                return (_value & (1 << index)) != 0;
            }

            set
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(IndexOutOfRangeExceptionMessage(index, Count));
                }

                int flag = 1 << index;
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

        private int _value;

        public IntBools(int source = 0)
        {
            _value = source;
        }

        public IntBools([NotNull] bool[] source)
        {
            int count = Count < source.Length ? Count : source.Length;
            int result = 0;

            for (int i = 0; i < count; i++)
            {
                int temp = source[i] ? 1 : 0;
                result |= temp << i;
            }

            _value = result;
        }

        public void Set(IntBools source, byte startIndex) => Set(source.ToIEumerable(), startIndex);
        
        public void Set(int source, byte startIndex) => Set(((IntBools)source).ToIEumerable(), startIndex);

        public void Set([NotNull] IEnumerable<bool> source, byte startIndex = 0)
        {
            IntBools temp = source.ToArray();
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