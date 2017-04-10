using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.KeyTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class GuidKeyTests : ServicedComponent
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
			int count = DataUtil.CountRows("GuidKeys");
			Guid id =  new Guid("{DAB20E73-806B-42F6-9A59-8240487848A4}");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidKeyTestObject test = transaction.Select(typeof(GuidKeyTestObject), id) as GuidKeyTestObject;

			Assert.AreEqual(id, test.Id);
			Assert.AreEqual(1, test.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("GuidKeys"));
		}

		[Test]
		public void SelectDoesNotExist()
		{
			int count = DataUtil.CountRows("GuidKeys");
			Guid id =  new Guid("{EAB20E73-806B-42F6-9A59-8240487848A4}");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidKeyTestObject test = transaction.Select(typeof(GuidKeyTestObject), id) as GuidKeyTestObject;

			Assert.IsNull(test);

			Assert.AreEqual(count, DataUtil.CountRows("GuidKeys"));
		}

		[Test]
		public void Insert()
		{
			int count = DataUtil.CountRows("GuidKeys");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidKeyTestObject test = transaction.Create(typeof(GuidKeyTestObject)) as GuidKeyTestObject;

			test.ObjData = 69;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("GuidKeys"));
		}

		[Test]
		public void Update()
		{
			int count = DataUtil.CountRows("GuidKeys");
			Guid id =  new Guid("{EEDD8FC4-9081-4573-B7E6-B3930FBDAA3C}");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			GuidKeyTestObject test1 = transaction1.Select(typeof(GuidKeyTestObject), id) as GuidKeyTestObject;

			Assert.AreEqual(id, test1.Id);
			Assert.AreEqual(2, test1.ObjData);

			test1.ObjData = 13;
			transaction1.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("GuidKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			GuidKeyTestObject test2 = transaction2.Select(typeof(GuidKeyTestObject), id) as GuidKeyTestObject;

			Assert.AreEqual(test1.Id, test2.Id);
			Assert.AreEqual(13, test2.ObjData);

			Assert.AreEqual(count, DataUtil.CountRows("GuidKeys"));
		}

		[Test]
		public void Delete()
		{
			int count = DataUtil.CountRows("GuidKeys");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			GuidKeyTestObject test1 = transaction1.Create(typeof(GuidKeyTestObject)) as GuidKeyTestObject;

			test1.ObjData = 169;
			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("GuidKeys"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			GuidKeyTestObject test2 = transaction2.Select(typeof(GuidKeyTestObject), test1.Id) as GuidKeyTestObject;

			Assert.AreEqual(test1.Id, test2.Id);
			Assert.AreEqual(169, test2.ObjData);

			transaction2.Delete(test2);

			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("GuidKeys"));
		}
	}
}
