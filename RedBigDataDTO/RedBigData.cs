using System.Runtime.InteropServices;

namespace RedBigData
{
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

        public SelectColumn SelectColumn(string table, string column)
            => SelectColumn(_ptr, table, table.Length, column, column.Length);

        public void InsertColumn(string table, string column, ulong index, byte[] data)
            => InsertColumn(_ptr, table, table.Length, column, column.Length, index, data, data.Length);

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

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?SelectColumn@RedBigData@1@SA?AUDllSelect@1@PEAV11@PEB_SH1H@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern SelectColumn SelectColumn(IntPtr RedBigData, string table, int len, string columnName, int columnNameLen);

        [DllImport(@"..\..\..\x64\Release\RedBigData.dll",
            EntryPoint = "?InsertColumn@RedBigData@1@SAXPEAV11@PEB_SH1H_KPEADH@Z", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern void InsertColumn(IntPtr RedBigData, string table, int len, string columnName, int columnNameLen, ulong index, byte[] data, int dataLen);

        public void Dispose()
        {
            Destructor(_ptr);
            Console.WriteLine($"destructed {_ptr}");
        }
    }
}
