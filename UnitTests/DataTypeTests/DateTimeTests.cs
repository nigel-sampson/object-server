using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class DateTimeTests : ServicedComponent
	{
		private const string MaxValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string MinValues = "{168CED7B-0D67-4FE1-BA4C-5E7DE49523E6}";
		private const string UpdateValue = "{0F89267D-615B-48D1-BAE9-B990C37C4B15}";
		private const string DoesNotExistValues = "{1F89267D-615B-48D1-BAE9-B990C37C4B15}";

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
		public void SelectMaxValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test = transaction.Select(typeof(DateTimeTestObject), MaxValues) as DateTimeTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(new DateTime(9999, 12, 31), test.Date);
			Assert.AreEqual(new DateTime(2079, 6, 6), test.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test = transaction.Select(typeof(DateTimeTestObject), MinValues) as DateTimeTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(new DateTime(1753, 1, 1), test.Date);
			Assert.AreEqual(new DateTime(1900, 1, 1), test.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test = transaction.Select(typeof(DateTimeTestObject), DoesNotExistValues) as DateTimeTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test = transaction.Create(typeof(DateTimeTestObject)) as DateTimeTestObject;

			test.Date = DateTime.Now.AddDays(1);
			test.SmallDate = DateTime.Now.AddDays(-1);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.DateTimeTestObject.Date has no value")]
		public void InsertMissingValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test = transaction.Create(typeof(DateTimeTestObject)) as DateTimeTestObject;

			test.SmallDate = DateTime.Now.AddDays(-1);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			DateTimeTestObject test1 = transaction.Select(typeof(DateTimeTestObject), UpdateValue) as DateTimeTestObject;
	
			Assert.AreEqual(new DateTime(2004, 10, 23), test1.Date);
			Assert.AreEqual(new DateTime(2004, 10, 23), test1.SmallDate);
			
			test1.Date = test1.Date.AddDays(1);
			test1.SmallDate = test1.SmallDate.AddDays(-1);

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			DateTimeTestObject test2 = transaction2.Select(typeof(DateTimeTestObject), UpdateValue) as DateTimeTestObject;

			Assert.AreEqual(new DateTime(2004, 10, 24), test2.Date);
			Assert.AreEqual(new DateTime(2004, 10, 22), test2.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("DateTimes");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			DateTimeTestObject test1 = transaction1.Create(typeof(DateTimeTestObject)) as DateTimeTestObject;

			test1.Date = DateTime.Today.AddDays(1);
			test1.SmallDate = DateTime.Today.AddMonths(1);

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			DateTimeTestObject test2 = transaction2.Select(typeof(DateTimeTestObject), test1.Id) as DateTimeTestObject;

			Assert.AreEqual(DateTime.Today.AddDays(1), test2.Date);
			Assert.AreEqual(DateTime.Today.AddMonths(1), test2.SmallDate);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("DateTimes"));
		}
	}
}
