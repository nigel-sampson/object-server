using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.KeyTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class DefinedKeyTests : ServicedComponent
	{
		private ObjectManager manager;

		[SetUp]
		public void Setup()
		{
			manager = new ObjectManager(ServerType.SqlServer, Constants.ConnectionString);
		}

		[TearDown]
		public void TearDown()
		{
			if(ContextUtil.IsInTransaction)
				ContextUtil.SetAbort();
		}

		[Test]
		public void Select()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			DefinedKeyTestObject test = transaction.Select(typeof(DefinedKeyTestObject), "defined1") as DefinedKeyTestObject;

			Assert.AreEqual("defined1", test.Id);
			Assert.AreEqual(1, test.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));
		}

		[Test]
		public void SelectDoesNotExist()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			DefinedKeyTestObject test = transaction.Select(typeof(DefinedKeyTestObject), "defined64") as DefinedKeyTestObject;

			Assert.IsNull(test);

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.DefinedKeyTestObject KeyType is PrimaryKey.Defined and no has been key passed to ObjectTransaction.Create")]
		public void CreateWithoutKey()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			DefinedKeyTestObject test = transaction.Create(typeof(DefinedKeyTestObject)) as DefinedKeyTestObject;

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));
		}

		[Test]
		public void Insert()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			DefinedKeyTestObject test = transaction.Create(typeof(DefinedKeyTestObject), "defined3") as DefinedKeyTestObject;

			test.ObjData = 69;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DefinedKeys"));
		}

		[Test]
		public void Update()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			DefinedKeyTestObject test1 = transaction1.Select(typeof(DefinedKeyTestObject), "defined2") as DefinedKeyTestObject;

			Assert.AreEqual("defined2", test1.Id);
			Assert.AreEqual(2, test1.ObjData);

			test1.ObjData = 13;
			transaction1.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			DefinedKeyTestObject test2 = transaction2.Select(typeof(DefinedKeyTestObject), "defined2") as DefinedKeyTestObject;

			Assert.AreEqual("defined2", test2.Id);
			Assert.AreEqual(13, test2.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));
		}

		[Test]
		public void Delete()
		{
			int count = DataUtil.CountRows("DefinedKeys");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			DefinedKeyTestObject test1 = transaction1.Create(typeof(DefinedKeyTestObject), "defined10") as DefinedKeyTestObject;

			Assert.AreEqual("defined10", test1.Id);

			test1.ObjData = 101;
			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DefinedKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			DefinedKeyTestObject test2 = transaction2.Select(typeof(DefinedKeyTestObject), "defined10") as DefinedKeyTestObject;

			Assert.AreEqual("defined10", test2.Id);
			Assert.AreEqual(101, test2.ObjData);

			transaction2.Delete(test2);

			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("DefinedKeys"));
		}
	}
}
