namespace RedBigDataTests
{
    using RedBigData;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [TestClass]
    public class RedBigDataTests
    {
        public const string TestPath = @"..\..\..\Databases";

        [TestMethod]
        public void RedBigDataTest()
        {
            if (Directory.Exists(TestPath))
                Directory.Delete(TestPath, true);
            Assert.ThrowsException<SEHException>(() => new RedBigData(TestPath));
            Directory.CreateDirectory(TestPath);
            using RedBigData redBigData = new RedBigData(TestPath);
            Assert.IsNotNull(redBigData);

            //databases
            string[] databases = redBigData.GetDatabases().ToArray();
            Assert.AreEqual(0, databases.Length);
            redBigData.CreateDatabase("test");
            databases = redBigData.GetDatabases().ToArray();
            Assert.AreEqual(1, databases.Length);
            Assert.AreEqual("test", Path.GetFileName(databases[0]));
            Assert.ThrowsException<SEHException>(() => redBigData.GetDatabase());
            Assert.ThrowsException<SEHException>(() => redBigData.CreateTable("tableA", Array.Empty<Column>()));
            Assert.ThrowsException<SEHException>(() => redBigData.GetTables());
            Assert.ThrowsException<SEHException>(() => redBigData.GetTable("tableA"));
            Assert.ThrowsException<SEHException>(() => redBigData.GetTable("tableB"));

            redBigData.UseDatabase("test");

            Assert.AreEqual("test", redBigData.GetDatabase());

            //tables
            string[] tables = redBigData.GetTables().ToArray();
            Assert.AreEqual(0, tables.Length);
            redBigData.CreateTable("tableA", Array.Empty<Column>());
            Assert.ThrowsException<SEHException>(() => redBigData.CreateTable("tableA", Array.Empty<Column>()));
            tables = redBigData.GetTables().ToArray();
            Assert.AreEqual(1, tables.Length);
            Assert.AreEqual("tableA", Path.GetFileName(tables[0]));

            Table tableA = redBigData.GetTable("tableA");
            Assert.AreEqual(0, tableA.columns.Length);
            Assert.AreEqual(0uL, tableA.rows);

            redBigData.CreateTable("tableB", new Column[] { new Column("num", 4ul) });
            Assert.ThrowsException<SEHException>(() => redBigData.CreateTable("tableB", Array.Empty<Column>()));
            tables = redBigData.GetTables().ToArray();
            Assert.AreEqual(2, tables.Length);
            Assert.AreEqual("tableB", Path.GetFileName(tables[1]));

            Table tableB = redBigData.GetTable("tableB");
            Assert.AreEqual(1, tableB.columns.Length);
            Assert.AreEqual("num", tableB.columns[0].name);
            Assert.AreEqual(4ul, tableB.columns[0].byteLen);
            Assert.AreEqual(0uL, tableB.rows);

            int[] data = redBigData.SelectColumn("tableB", "num").GetData<int>();
            Assert.AreEqual(0, data.Length);

            redBigData.InsertColumn("tableB", "num", 0, BitConverter.GetBytes(5));
            data = redBigData.SelectColumn("tableB", "num").GetData<int>();
            Assert.AreEqual(1, data.Length);

            redBigData.InsertColumn("tableB", "num", 1, BitConverter.GetBytes(25));
            data = redBigData.SelectColumn("tableB", "num").GetData<int>();
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(5, data[0]);
            Assert.AreEqual(25, data[1]);

            redBigData.InsertColumn("tableB", "num", 0, BitConverter.GetBytes(2));
            data = redBigData.SelectColumn("tableB", "num").GetData<int>();
            Assert.AreEqual(2, data.Length);
            Assert.AreEqual(2, data[0]);
            Assert.AreEqual(25, data[1]);

            SelectColumn arr = redBigData.SelectColumn("tableB", "num");
            arr.data.len = 8;
            byte[] byteData = arr.GetData<byte>();
            Assert.AreEqual(8, byteData.Length);
            Assert.AreEqual(2, byteData[0]);
            for (int i = 1; i <= 3; i++)
                Assert.AreEqual(0, byteData[i]);
            Assert.AreEqual(25, byteData[4]);
            for (int i = 5; i <= 7; i++)
                Assert.AreEqual(0, byteData[i]);
        }
    }
}