namespace Terminal
{
    using RedBigData;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal class Program
    {
        public static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            using RedBigData redBigData = new RedBigData(@"..\..\..\Databases");

            string? command;
            while (!string.IsNullOrWhiteSpace(command = Console.ReadLine()))
            {
                string[] args = command.Split(" ");
                switch (args[0])
                {
                    case "show":
                        switch (args[1])
                        {
                            case "database":
                                Console.WriteLine(redBigData.GetDatabase());
                                break;
                            case "databases":
                                IEnumerable<string> databases = redBigData.GetDatabases();
                                if (databases.Any())
                                {
                                    foreach (string database in databases)
                                    {
                                        Console.WriteLine("  -" + Path.GetFileName(database));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No Databases");
                                }
                                break;
                            case "table":
                                {
                                    Table table = redBigData.GetTable(args[2]);
                                    Console.WriteLine($"Table: {args[2]} - {table.rows} rows");
                                    if (table.columns.Length == 0)
                                    {
                                        Console.WriteLine("No Columns");
                                    }
                                    else
                                    {
                                        foreach (Column column in table.columns)
                                        {
                                            Console.WriteLine($"  -{column.name} : {column.byteLen}");
                                        }
                                    }
                                }
                                break;
                            case "tables":
                                IEnumerable<string> tables = redBigData.GetTables();
                                if (tables.Any())
                                {
                                    foreach (string table in tables)
                                    {
                                        Console.WriteLine("  -" + Path.GetFileName(table));
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No Tables");
                                }
                                break;
                        }
                        break;
                    case "use":
                        redBigData.UseDatabase(args[1]);
                        break;
                    case "create":
                        switch (args[1])
                        {
                            case "database":
                                redBigData.CreateDatabase(args[2]);
                                break;
                            case "table":
                                Column[] columns = new Column[(args.Length - 3) / 2];
                                for (int i = 3; i < args.Length; i += 2)
                                {
                                    columns[(i - 2) / 2] = new Column(args[i], ulong.Parse(args[i + 1]));
                                }
                                redBigData.CreateTable(args[2], columns);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            }
        }
    }
}
