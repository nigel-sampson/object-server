using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class IntegerTests : ServicedComponent
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
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Select(typeof(IntegerTestObject), MaxValues) as IntegerTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			Assert.AreEqual(true, test.Boolean);
			Assert.AreEqual(Byte.MaxValue, test.TinyInt);
			Assert.AreEqual(Int16.MaxValue, test.SmallInt);
			Assert.AreEqual(Int32.MaxValue, test.Int);
			Assert.AreEqual(Int64.MaxValue, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Select(typeof(IntegerTestObject), MinValues) as IntegerTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(Byte.MinValue, test.TinyInt);
			Assert.AreEqual(Int16.MinValue, test.SmallInt);
			Assert.AreEqual(Int32.MinValue, test.Int);
			Assert.AreEqual(Int64.MinValue, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void SelectZeroValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Select(typeof(IntegerTestObject), ZeroValues) as IntegerTestObject;

			Assert.AreEqual(new Guid(ZeroValues), test.Id);
			Assert.AreEqual(false, test.Boolean);
			Assert.AreEqual(0, test.TinyInt);
			Assert.AreEqual(0, test.SmallInt);
			Assert.AreEqual(0, test.Int);
			Assert.AreEqual(0, test.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Select(typeof(IntegerTestObject), DoesNotExistValues) as IntegerTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Create(typeof(IntegerTestObject)) as IntegerTestObject;

			test.Boolean = false;
			test.TinyInt = 1;
			test.Int = 123;
			test.SmallInt = 56;
			test.BigInt = 169;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Integers"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.IntegerTestObject.BigInt has no value")]
		public void InsertMissingValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test = transaction.Create(typeof(IntegerTestObject)) as IntegerTestObject;

			test.TinyInt = 1;
			test.Int = 123;
			test.SmallInt = 56;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction = manager.BeginTransaction();

			IntegerTestObject test1 = transaction.Select(typeof(IntegerTestObject), UpdateValue) as IntegerTestObject;
	
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

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			IntegerTestObject test2 = transaction2.Select(typeof(IntegerTestObject), UpdateValue) as IntegerTestObject;

			Assert.AreEqual(false, test2.Boolean);
			Assert.AreEqual(2, test2.TinyInt);
			Assert.AreEqual(3, test2.SmallInt);
			Assert.AreEqual(4, test2.Int);
			Assert.AreEqual(5, test2.BigInt);

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("Integers");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			IntegerTestObject test1 = transaction1.Create(typeof(IntegerTestObject)) as IntegerTestObject;

			test1.Boolean = false;
			test1.TinyInt = 1;
			test1.Int = 123;
			test1.SmallInt = 56;
			test1.BigInt = 169;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Integers"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			IntegerTestObject test2 = transaction2.Select(typeof(IntegerTestObject), test1.Id) as IntegerTestObject;

			Assert.AreEqual(false, test2.Boolean);
			Assert.AreEqual(1, test2.TinyInt);
			Assert.AreEqual(123, test2.Int);
			Assert.AreEqual(56, test2.SmallInt);
			Assert.AreEqual(169, test2.BigInt);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Integers"));
		}
	}
}
