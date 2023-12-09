using System.Diagnostics.CodeAnalysis;

namespace DevoramUtility
{
    public struct ShortBools
    {
        public static string IndexOutOfRangeExceptionMessage(byte index, int count) => $"index : {index} is bigger than {count}";

        public static implicit operator short(ShortBools value) => value._value;

        public static implicit operator ShortBools(short value) => new(value);

        public static implicit operator ShortBools([NotNull] bool[] value) => new(value);

        public static implicit operator ShortBools([NotNull] List<bool> value) => new(value.ToArray());


        public static ShortBools TRUE { get; } = -1;

        public static ShortBools FALSE { get; } = 0;

        public static int Count { get; } = sizeof(short) * 8;

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

                int source = _value;
                int temp = source & (1 << index);
                return temp != 0;
            }

            set
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException(IndexOutOfRangeExceptionMessage(index, Count));
                }

                int flag = 1 << index;
                int source = _value;
                int temp = value ? (source | flag) : (source & ~flag);
                _value = (short)temp;
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

        private short _value;

        public ShortBools(short source = 0)
        {
            _value = source;
        }

        public ShortBools([NotNull] bool[] source)
        {
            int count = Count < source.Length ? Count : source.Length;
            int result = 0;

            for (int i = 0; i < count; i++)
            {
                int temp = source[i] ? 1 : 0;
                result |= temp << i;
            }

            _value = (short)result;
        }

        public void Set(ShortBools source, byte startIndex) => Set(source.ToIEumerable(), startIndex);

        public void Set(int source, byte startIndex) => Set(((ShortBools)source).ToIEumerable(), startIndex);

        public void Set([NotNull] IEnumerable<bool> source, byte startIndex = 0)
        {
            ShortBools temp = source.ToArray();
            int value = _value;
            value |= (int)temp << startIndex;
            _value = (short)value;
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