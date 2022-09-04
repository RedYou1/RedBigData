namespace RedBigDataTests
{
    using RedBigData;
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
        }
    }
}