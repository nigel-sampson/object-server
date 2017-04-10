using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullableDateTimeTests : ServicedComponent
	{
		private const string MaxValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string MinValues = "{168CED7B-0D67-4FE1-BA4C-5E7DE49523E6}";
		private const string UpdateValue = "{0F89267D-615B-48D1-BAE9-B990C37C4B15}";
		private const string DoesNotExistValues = "{1F89267D-615B-48D1-BAE9-B990C37C4B15}";
		private const string NullValues = "{8320B9E8-64B0-415E-9973-08FBDECAF842}";

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
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Select(typeof(NullableDateTimeTestObject), MaxValues) as NullableDateTimeTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(new DateTime(9999, 12, 31), test.Date);
			Assert.AreEqual(new DateTime(2079, 6, 6), test.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Select(typeof(NullableDateTimeTestObject), MinValues) as NullableDateTimeTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(new DateTime(1753, 1, 1), test.Date);
			Assert.AreEqual(new DateTime(1900, 1, 1), test.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void SelectNullValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");
			Assert.IsTrue(DataUtil.IsRowNull("NullableDateTimes", "id", NullValues));

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Select(typeof(NullableDateTimeTestObject), NullValues) as NullableDateTimeTestObject;

			Assert.AreEqual(new Guid(NullValues), test.Id);
			Assert.AreEqual(new DateTime(2004, 1, 1), test.Date);
			Assert.AreEqual(new DateTime(1981, 7, 11), test.SmallDate);
			
			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Select(typeof(NullableDateTimeTestObject), DoesNotExistValues) as NullableDateTimeTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void CreateNullValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Create(typeof(NullableDateTimeTestObject)) as NullableDateTimeTestObject;

			Assert.AreEqual(new DateTime(2004, 1, 1), test.Date);
			Assert.AreEqual(new DateTime(1981, 7, 11), test.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test = transaction.Create(typeof(NullableDateTimeTestObject)) as NullableDateTimeTestObject;

			test.Date = DateTime.Now.AddDays(1);
			test.SmallDate = DateTime.Now.AddDays(-1);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void InsertNullImplicitValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableDateTimeTestObject test1 = transaction1.Create(typeof(NullableDateTimeTestObject)) as NullableDateTimeTestObject;

			transaction1.Commit();

			Assert.IsTrue(DataUtil.IsRowNull("NullableDateTimes", "id", test1.Id));
			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableDateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableDateTimeTestObject test2 = transaction2.Select(typeof(NullableDateTimeTestObject), test1.Id) as NullableDateTimeTestObject;

			Assert.AreEqual(new DateTime(2004, 1, 1), test2.Date);
			Assert.AreEqual(new DateTime(1981, 7, 11), test2.SmallDate);
		}

		[Test]
		public void InsertNullExplicitValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableDateTimeTestObject test1 = transaction1.Create(typeof(NullableDateTimeTestObject)) as NullableDateTimeTestObject;

			test1.Date = new DateTime(2004, 1, 1);
			test1.SmallDate = new DateTime(1981, 7, 11);

			transaction1.Commit();

			Assert.IsTrue(DataUtil.IsRowNull("NullableDateTimes", "id", test1.Id));
			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableDateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableDateTimeTestObject test2 = transaction2.Select(typeof(NullableDateTimeTestObject), test1.Id) as NullableDateTimeTestObject;

			Assert.AreEqual(new DateTime(2004, 1, 1), test2.Date);
			Assert.AreEqual(new DateTime(1981, 7, 11), test2.SmallDate);
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableDateTimeTestObject test1 = transaction.Select(typeof(NullableDateTimeTestObject), UpdateValue) as NullableDateTimeTestObject;
	
			Assert.AreEqual(new DateTime(2003, 10, 23), test1.Date);
			Assert.AreEqual(new DateTime(2003, 10, 23), test1.SmallDate);
			
			test1.Date = test1.Date.AddDays(-6);
			test1.SmallDate = test1.SmallDate.AddYears(1);

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableDateTimeTestObject test2 = transaction2.Select(typeof(NullableDateTimeTestObject), UpdateValue) as NullableDateTimeTestObject;

			Assert.AreEqual(new DateTime(2003, 10, 17), test2.Date);
			Assert.AreEqual(new DateTime(2004, 10, 23), test2.SmallDate);

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("NullableDateTimes");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			NullableDateTimeTestObject test1 = transaction1.Create(typeof(NullableDateTimeTestObject)) as NullableDateTimeTestObject;

			test1.Date = DateTime.Today.AddDays(-30);
			test1.SmallDate = DateTime.Today.AddMonths(1);

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableDateTimes"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableDateTimeTestObject test2 = transaction2.Select(typeof(NullableDateTimeTestObject), test1.Id) as NullableDateTimeTestObject;

			Assert.AreEqual(DateTime.Today.AddDays(-30), test2.Date);
			Assert.AreEqual(DateTime.Today.AddMonths(1), test2.SmallDate);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableDateTimes"));
		}
	}
}
