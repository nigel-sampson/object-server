using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.KeyTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class IdentityKeyTests : ServicedComponent
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
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			IdentityKeyTestObject test = transaction.Select(typeof(IdentityKeyTestObject), 1) as IdentityKeyTestObject;

			Assert.AreEqual(1, test.Id);
			Assert.AreEqual("data1", test.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));
		}

		[Test]
		public void SelectDoesNotExist()
		{
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			IdentityKeyTestObject test = transaction.Select(typeof(IdentityKeyTestObject), -1) as IdentityKeyTestObject;

			Assert.IsNull(test);

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));
		}
		
		[Test]
		public void Create()
		{
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			IdentityKeyTestObject test1 = transaction.Create(typeof(IdentityKeyTestObject)) as IdentityKeyTestObject;
			IdentityKeyTestObject test2 = transaction.Create(typeof(IdentityKeyTestObject)) as IdentityKeyTestObject;

			Assert.AreEqual(-1, test1.Id);
			Assert.AreEqual(-2, test2.Id);

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));
		}

		[Test]
		public void Insert()
		{
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			IdentityKeyTestObject test1 = transaction.Create(typeof(IdentityKeyTestObject)) as IdentityKeyTestObject;
			Assert.AreEqual(-1, test1.Id);

			test1.ObjData = "test";

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("IdentityKeys"));

			IdentityKeyTestObject test2 = transaction.Create(typeof(IdentityKeyTestObject)) as IdentityKeyTestObject;
			Assert.AreEqual(-2, test2.Id);

			test2.ObjData = "test1";

			transaction.Commit();

			Assert.AreEqual(test2.Id, test1.Id + 1);

			Assert.AreEqual(count + 2, DataUtil.CountRows("IdentityKeys"));
		}

		[Test]
		public void Update()
		{
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			IdentityKeyTestObject test1 = transaction1.Select(typeof(IdentityKeyTestObject), 2) as IdentityKeyTestObject;

			Assert.AreEqual(2, test1.Id);
			Assert.AreEqual("data2", test1.ObjData);

			test1.ObjData = "datatest";
			transaction1.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			IdentityKeyTestObject test2 = transaction2.Select(typeof(IdentityKeyTestObject), 2) as IdentityKeyTestObject;

			Assert.AreEqual(2, test2.Id);
			Assert.AreEqual("datatest", test2.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));
		}

		[Test]
		public void Delete()
		{
			int count = DataUtil.CountRows("IdentityKeys");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			IdentityKeyTestObject test1 = transaction1.Create(typeof(IdentityKeyTestObject)) as IdentityKeyTestObject;

			Assert.AreEqual(-1, test1.Id);

			test1.ObjData = "blah";
			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("IdentityKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			IdentityKeyTestObject test2 = transaction2.Select(typeof(IdentityKeyTestObject), test1.Id) as IdentityKeyTestObject;

			Assert.AreEqual(test1.Id, test2.Id);
			Assert.AreEqual("blah", test2.ObjData);

			transaction2.Delete(test2);

			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("IdentityKeys"));
		}
	}
}
