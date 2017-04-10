using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullableIntegerTests : ServicedComponent
	{
		private const string MaxValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string MinValues = "{168CED7B-0D67-4FE1-BA4C-5E7DE49523E6}";
		private const string ZeroValues = "{E59CAA75-3A0F-4688-8685-682D744AD37F}";
		private const string NullValues = "{8320B9E8-64B0-415E-9973-08FBDECAF842}";
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
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Select(typeof(NullableIntegerTestObject), MaxValues) as NullableIntegerTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(true, test.Boolean);
			Assert.AreEqual(Byte.MaxValue, test.TinyInt);
			Assert.AreEqual(Int16.MaxValue, test.SmallInt);
			Assert.AreEqual(Int32.MaxValue, test.Int);
			Assert.AreEqual(Int64.MaxValue, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Select(typeof(NullableIntegerTestObject), MinValues) as NullableIntegerTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(Byte.MinValue, test.TinyInt);
			Assert.AreEqual(Int16.MinValue, test.SmallInt);
			Assert.AreEqual(Int32.MinValue, test.Int);
			Assert.AreEqual(Int64.MinValue, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void SelectZeroValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Select(typeof(NullableIntegerTestObject), ZeroValues) as NullableIntegerTestObject;

			Assert.AreEqual(new Guid(ZeroValues), test.Id);
			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(0, test.TinyInt);
			Assert.AreEqual(0, test.SmallInt);
			Assert.AreEqual(0, test.Int);
			Assert.AreEqual(0, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void SelectNullValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");
			Assert.IsTrue(DataUtil.IsRowNull("NullableIntegers", "id", NullValues));

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Select(typeof(NullableIntegerTestObject), NullValues) as NullableIntegerTestObject;

			Assert.AreEqual(new Guid(NullValues), test.Id);
			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(1, test.TinyInt);
			Assert.AreEqual(-1, test.SmallInt);
			Assert.AreEqual(-2, test.Int);
			Assert.AreEqual(-3, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Select(typeof(NullableIntegerTestObject), DoesNotExistValues) as NullableIntegerTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void CreateNullValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Create(typeof(NullableIntegerTestObject)) as NullableIntegerTestObject;

			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(1, test.TinyInt);
			Assert.AreEqual(-1, test.SmallInt);
			Assert.AreEqual(-2, test.Int);
			Assert.AreEqual(-3, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test = transaction.Create(typeof(NullableIntegerTestObject)) as NullableIntegerTestObject;

			test.Boolean = true;
			test.TinyInt = 5;
			test.Int = 12456;
			test.SmallInt = 124;
			test.BigInt = 1234567;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void InsertNullImplicitValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableIntegerTestObject test1 = transaction1.Create(typeof(NullableIntegerTestObject)) as NullableIntegerTestObject;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableIntegers"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableIntegers", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableIntegerTestObject test2 = transaction2.Select(typeof(NullableIntegerTestObject), test1.Id) as NullableIntegerTestObject;

			Assert.AreEqual(false, test2.Boolean);
			Assert.AreEqual(1, test2.TinyInt);
			Assert.AreEqual(-1, test2.SmallInt);
			Assert.AreEqual(-2, test2.Int);
			Assert.AreEqual(-3, test2.BigInt);
		}

		[Test]
		public void InsertNullExplicitValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableIntegerTestObject test1 = transaction1.Create(typeof(NullableIntegerTestObject)) as NullableIntegerTestObject;

			test1.Boolean = false;
			test1.TinyInt = 1;
			test1.SmallInt = -1;
			test1.Int = -2;
			test1.BigInt = -3;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableIntegers"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableIntegers", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableIntegerTestObject test2 = transaction2.Select(typeof(NullableIntegerTestObject), test1.Id) as NullableIntegerTestObject;


			Assert.AreEqual(false, test2.Boolean);
			Assert.AreEqual(1, test2.TinyInt);
			Assert.AreEqual(-1, test2.SmallInt);
			Assert.AreEqual(-2, test2.Int);
			Assert.AreEqual(-3, test2.BigInt);
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			NullableIntegerTestObject test1 = transaction1.Create(typeof(NullableIntegerTestObject)) as NullableIntegerTestObject;

			test1.Boolean = true;
			test1.TinyInt = 5;
			test1.Int = 12457;
			test1.SmallInt = 124;
			test1.BigInt = 1234567;
			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableIntegers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableIntegerTestObject test2 = transaction2.Select(typeof(NullableIntegerTestObject), test1.Id) as NullableIntegerTestObject;

			Assert.AreEqual(true, test2.Boolean);
			Assert.AreEqual(5, test2.TinyInt);
			Assert.AreEqual(124, test2.SmallInt);
			Assert.AreEqual(12457, test1.Int);
			Assert.AreEqual(1234567, test2.BigInt);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("NullableIntegers");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableIntegerTestObject test1 = transaction.Select(typeof(NullableIntegerTestObject), UpdateValue) as NullableIntegerTestObject;
	
			Assert.AreEqual(true, test1.Boolean);
			Assert.AreEqual(1, test1.TinyInt);
			Assert.AreEqual(1, test1.SmallInt);
			Assert.AreEqual(1, test1.Int);
			Assert.AreEqual(1, test1.BigInt);

			test1.Boolean = false;
			test1.TinyInt = 2;
			test1.SmallInt = 3;
			test1.Int = 4;
			test1.BigInt = 5;

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableIntegerTestObject test2 = transaction2.Select(typeof(NullableIntegerTestObject), UpdateValue) as NullableIntegerTestObject;

			Assert.AreEqual(false, test2.Boolean);
			Assert.AreEqual(2, test2.TinyInt);
			Assert.AreEqual(3, test2.SmallInt);
			Assert.AreEqual(4, test2.Int);
			Assert.AreEqual(5, test2.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("NullableIntegers"));
		}
	}
}
