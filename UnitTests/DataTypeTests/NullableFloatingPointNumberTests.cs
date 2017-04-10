using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullableFloatingPointNumberTests : ServicedComponent
	{
		private const string MaxValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string MinValues = "{168CED7B-0D67-4FE1-BA4C-5E7DE49523E6}";
		private const string ZeroValues = "{E59CAA75-3A0F-4688-8685-682D744AD37F}";
		private const string UpdateValue = "{0F89267D-615B-48D1-BAE9-B990C37C4B15}";
		private const string NullValues = "{8320B9E8-64B0-415E-9973-08FBDECAF842}";
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
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Select(typeof(NullableFloatingPointNumberTestObject), MaxValues) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(Decimal.MaxValue, test.Decimal);
			Assert.AreEqual(Decimal.MaxValue, test.Numeric);
			Assert.AreEqual(Double.MaxValue, test.Float);
			Assert.AreEqual(Single.MaxValue, test.Real);
			Assert.AreEqual(922337203685477.5807m, test.Money);
			Assert.AreEqual(214748.3647m, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Select(typeof(NullableFloatingPointNumberTestObject), MinValues) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(Decimal.MinValue, test.Decimal);
			Assert.AreEqual(Decimal.MinValue, test.Numeric);
			Assert.AreEqual(Double.MinValue, test.Float);
			Assert.AreEqual(Single.MinValue, test.Real);
			Assert.AreEqual(-922337203685477.5807m, test.Money);
			Assert.AreEqual(-214748.3647m, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void SelectNullValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");
			Assert.IsTrue(DataUtil.IsRowNull("NullableFloatingPointNumbers", "id", NullValues));

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Select(typeof(NullableFloatingPointNumberTestObject), NullValues) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(NullValues), test.Id);
			Assert.AreEqual(-1, test.Decimal);
			Assert.AreEqual(-1, test.Numeric);
			Assert.AreEqual(Double.MinValue, test.Float);
			Assert.AreEqual(Single.MinValue, test.Real);
			Assert.AreEqual(-1, test.Money);
			Assert.AreEqual(-1, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void SelectZeroValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Select(typeof(NullableFloatingPointNumberTestObject), ZeroValues) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(ZeroValues), test.Id);
			Assert.AreEqual(0, test.Decimal);
			Assert.AreEqual(0, test.Numeric);
			Assert.AreEqual(0, test.Float);
			Assert.AreEqual(0, test.Real);
			Assert.AreEqual(0, test.Money);
			Assert.AreEqual(0, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Select(typeof(NullableFloatingPointNumberTestObject), DoesNotExistValues) as NullableFloatingPointNumberTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Create(typeof(NullableFloatingPointNumberTestObject)) as NullableFloatingPointNumberTestObject;

			test.Decimal = 5.8m;
			test.Numeric = 6.9m;
			test.Float = -3.45;
			test.Real = 81.64f;
			test.Money = 56.56m;
			test.SmallMoney = -13.13m;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void CreateNullValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test = transaction.Create(typeof(NullableFloatingPointNumberTestObject)) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(-1, test.Decimal);
			Assert.AreEqual(-1, test.Numeric);
			Assert.AreEqual(Double.MinValue, test.Float);
			Assert.AreEqual(Single.MinValue, test.Real);
			Assert.AreEqual(-1, test.Money);
			Assert.AreEqual(-1, test.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void InsertNullImplicitValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test1 = transaction1.Create(typeof(NullableFloatingPointNumberTestObject)) as NullableFloatingPointNumberTestObject;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableFloatingPointNumbers"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableFloatingPointNumbers", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableFloatingPointNumberTestObject test2 = transaction2.Select(typeof(NullableFloatingPointNumberTestObject), test1.Id) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(-1, test2.Decimal);
			Assert.AreEqual(-1, test2.Numeric);
			Assert.AreEqual(Double.MinValue, test2.Float);
			Assert.AreEqual(Single.MinValue, test2.Real);
			Assert.AreEqual(-1, test2.Money);
			Assert.AreEqual(-1, test2.SmallMoney);
		}

		[Test]
		public void InsertNullExplicitValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test1 = transaction1.Create(typeof(NullableFloatingPointNumberTestObject)) as NullableFloatingPointNumberTestObject;

			test1.Decimal = -1;
			test1.Numeric = -1;
			test1.Float = Double.MinValue;
			test1.Real = Single.MinValue;
			test1.Money = -1;
			test1.SmallMoney = -1;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableFloatingPointNumbers"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableFloatingPointNumbers", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableFloatingPointNumberTestObject test2 = transaction2.Select(typeof(NullableFloatingPointNumberTestObject), test1.Id) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(-1, test2.Decimal);
			Assert.AreEqual(-1, test2.Numeric);
			Assert.AreEqual(Double.MinValue, test2.Float);
			Assert.AreEqual(Single.MinValue, test2.Real);
			Assert.AreEqual(-1, test2.Money);
			Assert.AreEqual(-1, test2.SmallMoney);
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableFloatingPointNumberTestObject test1 = transaction.Select(typeof(NullableFloatingPointNumberTestObject), UpdateValue) as NullableFloatingPointNumberTestObject;
	
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

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableFloatingPointNumberTestObject test2 = transaction2.Select(typeof(NullableFloatingPointNumberTestObject), UpdateValue) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(new Guid(UpdateValue), test2.Id);
			Assert.AreEqual(2, test2.Decimal);
			Assert.AreEqual(6, test2.Numeric);
			Assert.AreEqual(-6.9, test2.Float);
			Assert.AreEqual(-8.56, test2.Real);
			Assert.AreEqual(69.69m, test2.Money);
			Assert.AreEqual(-45.45m, test2.SmallMoney);

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("NullableFloatingPointNumbers");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			NullableFloatingPointNumberTestObject test1 = transaction1.Create(typeof(NullableFloatingPointNumberTestObject)) as NullableFloatingPointNumberTestObject;

			test1.Decimal = 2m;
			test1.Numeric = 565m;
			test1.Float = -1.25;
			test1.Real = 126;
			test1.Money = -65.65m;
			test1.SmallMoney = 23.545m;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableFloatingPointNumbers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableFloatingPointNumberTestObject test2 = transaction2.Select(typeof(NullableFloatingPointNumberTestObject), test1.Id) as NullableFloatingPointNumberTestObject;

			Assert.AreEqual(2, test2.Decimal);
			Assert.AreEqual(565, test2.Numeric);
			Assert.AreEqual(-1.25, test2.Float);
			Assert.AreEqual(126, test2.Real);
			Assert.AreEqual(-65.65m, test2.Money);
			Assert.AreEqual(23.545m, test2.SmallMoney);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableFloatingPointNumbers"));
		}
	}
}
