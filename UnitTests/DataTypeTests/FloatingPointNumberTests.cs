using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class FloatingPointNumberTests : ServicedComponent
	{
		private const string MaxValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string MinValues = "{168CED7B-0D67-4FE1-BA4C-5E7DE49523E6}";
		private const string ZeroValues = "{E59CAA75-3A0F-4688-8685-682D744AD37F}";
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
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Select(typeof(FloatingPointNumberTestObject), MaxValues) as FloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(Decimal.MaxValue, test.Decimal);
			Assert.AreEqual(Decimal.MaxValue, test.Numeric);
			Assert.AreEqual(Double.MaxValue, test.Float);
			Assert.AreEqual(Single.MaxValue, test.Real);
			Assert.AreEqual(922337203685477.5807m, test.Money);
			Assert.AreEqual(214748.3647m, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Select(typeof(FloatingPointNumberTestObject), MinValues) as FloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(Decimal.MinValue, test.Decimal);
			Assert.AreEqual(Decimal.MinValue, test.Numeric);
			Assert.AreEqual(Double.MinValue, test.Float);
			Assert.AreEqual(Single.MinValue, test.Real);
			Assert.AreEqual(-922337203685477.5807m, test.Money);
			Assert.AreEqual(-214748.3647m, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void SelectZeroValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Select(typeof(FloatingPointNumberTestObject), ZeroValues) as FloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(ZeroValues), test.Id);
			Assert.AreEqual(0, test.Decimal);
			Assert.AreEqual(0, test.Numeric);
			Assert.AreEqual(0, test.Float);
			Assert.AreEqual(0, test.Real);
			Assert.AreEqual(0, test.Money);
			Assert.AreEqual(0, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Select(typeof(FloatingPointNumberTestObject), DoesNotExistValues) as FloatingPointNumberTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Create(typeof(FloatingPointNumberTestObject)) as FloatingPointNumberTestObject;

			test.Decimal = 5.1m;
			test.Numeric = 6.6m;
			test.Float = -9.6;
			test.Real = 8.56f;
			test.Money = 45.65m;
			test.SmallMoney = -56.1m;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.FloatingPointNumberTestObject.Real has no value")]
		public void InsertMissingValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test = transaction.Create(typeof(FloatingPointNumberTestObject)) as FloatingPointNumberTestObject;

			test.Decimal = 5.1m;
			test.Numeric = 6.6m;
			test.Float = -9.6;
			test.Money = 45.65m;
			test.SmallMoney = -56.1m;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			FloatingPointNumberTestObject test1 = transaction.Select(typeof(FloatingPointNumberTestObject), UpdateValue) as FloatingPointNumberTestObject;
	
			Assert.AreEqual(new Guid(UpdateValue), test1.Id);
			Assert.AreEqual(1, test1.Decimal);
			Assert.AreEqual(1, test1.Numeric);
			Assert.AreEqual(1, test1.Float);
			Assert.AreEqual(1, test1.Real);
			Assert.AreEqual(1, test1.Money);
			Assert.AreEqual(1, test1.SmallMoney);

			test1.Decimal = 2m;
			test1.Numeric = 6m;
			test1.Float = -6.9;
			test1.Real = -8.56f;
			test1.Money = 69.69m;
			test1.SmallMoney = -45.45m;

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			FloatingPointNumberTestObject test2 = transaction2.Select(typeof(FloatingPointNumberTestObject), UpdateValue) as FloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(UpdateValue), test2.Id);
			Assert.AreEqual(2, test2.Decimal);
			Assert.AreEqual(6, test2.Numeric);
			Assert.AreEqual(-6.9, test2.Float);
			Assert.AreEqual(-8.56, test2.Real);
			Assert.AreEqual(69.69m, test2.Money);
			Assert.AreEqual(-45.45m, test2.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("FloatingPointNumbers");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			FloatingPointNumberTestObject test1 = transaction1.Create(typeof(FloatingPointNumberTestObject)) as FloatingPointNumberTestObject;

			test1.Decimal = 1.565m;
			test1.Numeric = 565m;
			test1.Float = -1.25;
			test1.Real = 126;
			test1.Money = -65.65m;
			test1.SmallMoney = 23.545m;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("FloatingPointNumbers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			FloatingPointNumberTestObject test2 = transaction2.Select(typeof(FloatingPointNumberTestObject), test1.Id) as FloatingPointNumberTestObject;

			Assert.AreEqual(2, test2.Decimal);
			Assert.AreEqual(565, test2.Numeric);
			Assert.AreEqual(-1.25, test2.Float);
			Assert.AreEqual(126, test2.Real);
			Assert.AreEqual(-65.65m, test2.Money);
			Assert.AreEqual(23.545m, test2.SmallMoney);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("FloatingPointNumbers"));
		}
	}
}
