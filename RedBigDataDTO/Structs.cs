namespace RedBigData
{
    public struct SelectColumn
    {
        public DllArray data;
        // 0 = dynamic
        public ulong byteLen;

        public SelectColumn(DllArray data, ulong byteLen)
        {
            this.data = data;
            this.byteLen = byteLen;
        }

        public Array GetData()
        {
            switch (byteLen)
            {
                case 0ul:
                    return data.ToArray<DllString>().Select(s => s.String()).ToArray();
                case 1ul:
                    return data.ToArray<byte>();
                case 2ul:
                    return data.ToArray<short>();
                case 4ul:
                    return data.ToArray<int>();
                case 8ul:
                    return data.ToArray<long>();
            }
            throw new NotImplementedException();
        }

        public T[] GetData<T>()
            where T : unmanaged
            => data.ToArray<T>();
    }

    public struct Column
    {
        public string name;
        // 0 = dynamic
        public ulong byteLen;

        public Column(string name, ulong byteLen)
        {
            this.name = name;
            this.byteLen = byteLen;
        }
    }

    public struct Table
    {
        public Column[] columns;
        public ulong rows;

        public Table(Column[] columns, ulong rows)
        {
            this.columns = columns;
            this.rows = rows;
        }
    }

    public struct DllColumn
    {
        public DllString name;
        // 0 = dynamic
        public ulong byteLen;

        public Column Column()
        {
            return new Column(name.String(), byteLen);
        }
    }

    public struct DllTable
    {
        public DllArray<DllColumn> columns;
        public ulong rows;

        public Table Table()
        {
            return new Table(columns.ToArray().Select(c => c.Column()).ToArray(), rows);
        }
    }

    public unsafe struct DllArray
    {
        public void* start;
        public ulong len;

        public DllArray(void* start, ulong len)
        {
            this.start = start;
            this.len = len;
        }

        public T[] ToArray<T>()
            where T : unmanaged
        {
            T[] values = new T[len];
            for (ulong i = 0; i < len; i++)
            {
                values[i] = *(((T*)start) + i);
            }
            return values;
        }
    }

    public unsafe struct DllArray<T>
        where T : unmanaged
    {
        public T* start;
        public ulong len;

        public DllArray(T* start, ulong len)
        {
            this.start = start;
            this.len = len;
        }

        public T[] ToArray()
        {
            T[] values = new T[len];
            for (ulong i = 0; i < len; i++)
            {
                values[i] = *(start + i);
            }
            return values;
        }
    }

    public unsafe struct DllString
    {
        public char* start;
        public ulong len;

        public DllString(char* start, ulong len)
        {
            this.start = start;
            this.len = len;
        }

        public string String()
        {
            string a = "";
            for (ulong i = 0; i < len; i++)
            {
                a += *(start + i);
            }
            return a;
        }
    }
}
