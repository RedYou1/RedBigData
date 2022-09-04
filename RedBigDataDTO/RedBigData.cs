using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace RedBigData
{
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

    internal struct DllColumn
    {
        public DllString name;
        // 0 = dynamic
        public ulong byteLen;

        public Column Column()
        {
            return new Column(name.String(), byteLen);
        }
    }

    internal struct DllTable
    {
        public DllArray<DllColumn> columns;
        public ulong rows;

        public Table Table()
        {
            return new Table(columns.ToArray().Select(c => c.Column()).ToArray(), rows);
        }
    }

    internal unsafe struct DllArray<T>
        where T : unmanaged
    {
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

        public T* start { get; }
        public ulong len { get; }
    }

    internal unsafe struct DllString
    {
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

        public char* start { get; }
        public ulong len { get; }
    }

    public sealed class RedBigData : IDisposable
    {
        private IntPtr _ptr;

        public RedBigData(string path)
        {
            _ptr = Constructor(path, path.Length);
            Console.WriteLine($"constructed {_ptr} {GetPath(_ptr).String()}");
        }

        public IEnumerable<string> GetDatabases()
            => GetDatabases(_ptr).ToArray().Select(a => a.String());

        public void UseDatabase(string database)
            => UseDatabase(_ptr, database, database.Length);

        public void CreateDatabase(string database)
            => CreateDatabase(_ptr, database, database.Length);

        public string GetDatabase()
            => Path.GetFileName(GetDatabase(_ptr).String());

        public IEnumerable<string> GetTables()
            => GetTables(_ptr).ToArray().Select(a => a.String());

        public Table GetTable(string table)
            => GetTable(_ptr, table, table.Length).Table();

        public void CreateTable(string table, Column[] columns)
            => CreateTable(_ptr, table, table.Length,
                columns.Select(c => c.name).ToArray(), columns.Select(c => c.name.Length).ToArray(),
                columns.Select(c => c.byteLen).ToArray(), columns.Length);


        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?Constructor@RedBigData@1@SAPEAV11@PEB_SH@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern IntPtr Constructor(string path, int len);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?Destructor@RedBigData@1@SAXPEAV11@@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void Destructor(IntPtr RedBigData);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?GetPath@RedBigData@1@SA?AUDllString@1@PEAV11@@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern DllString GetPath(IntPtr RedBigData);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?GetDatabases@RedBigData@1@SA?AUDllArray@1@PEAV11@@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern DllArray<DllString> GetDatabases(IntPtr RedBigData);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?CreateDatabase@RedBigData@1@SAXPEAV11@PEB_SH@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void CreateDatabase(IntPtr RedBigData, string database, int len);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?UseDatabase@RedBigData@1@SAXPEAV11@PEB_SH@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void UseDatabase(IntPtr RedBigData, string database, int len);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?GetDatabase@RedBigData@1@SA?AUDllString@1@PEAV11@@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern DllString GetDatabase(IntPtr RedBigData);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?GetTables@RedBigData@1@SA?AUDllArray@1@PEAV11@@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern DllArray<DllString> GetTables(IntPtr RedBigData);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?GetTable@RedBigData@1@SA?AUDllTable@1@PEAV11@PEB_SH@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern DllTable GetTable(IntPtr RedBigData, string table, int len);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?CreateTable@RedBigData@1@SAXPEAV11@PEB_SHPEAPEB_SPEAHPEA_K_K@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void CreateTable(IntPtr RedBigData, string table, int len, string[] columnsNames, int[] columnsNamesLen, ulong[] byteLen, int columnsLen);

        public void Dispose()
        {
            Destructor(_ptr);
            Console.WriteLine($"destructed {_ptr}");
        }
    }
}
