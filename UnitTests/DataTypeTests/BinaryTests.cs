using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class BinaryTests : ServicedComponent
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
			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Select(typeof(BinaryTestObject), MaxValues) as BinaryTestObject;

			Assert.AreEqual(new Guid(MaxValues), test.Id);
			
			Assert.AreEqual(8, test.Binary.Length);
			foreach(byte b in test.Binary)
			{
				Assert.AreEqual(Byte.MaxValue, b);
			}

			Assert.AreEqual(16, test.VarBinary.Length);
			foreach(byte b in test.VarBinary)
			{
				Assert.AreEqual(Byte.MaxValue, b);
			}

			Assert.AreEqual(32, test.Image.Length);
			foreach(byte b in test.Image)
			{
				Assert.AreEqual(Byte.MaxValue, b);
			}

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void SelectMinValues()
		{
			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Select(typeof(BinaryTestObject), ZeroValues) as BinaryTestObject;

			Assert.AreEqual(new Guid(ZeroValues), test.Id);
			
			Assert.AreEqual(8, test.Binary.Length);
			foreach(byte b in test.Binary)
			{
				Assert.AreEqual(Byte.MinValue, b);
			}

			Assert.AreEqual(16, test.VarBinary.Length);
			foreach(byte b in test.VarBinary)
			{
				Assert.AreEqual(Byte.MinValue, b);
			}

			Assert.AreEqual(32, test.Image.Length);
			foreach(byte b in test.Image)
			{
				Assert.AreEqual(Byte.MinValue, b);
			}

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void SelectZeroValues()
		{
			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Select(typeof(BinaryTestObject), MinValues) as BinaryTestObject;

			Assert.AreEqual(new Guid(MinValues), test.Id);
			
			Assert.AreEqual(8, test.Binary.Length);
			foreach(byte b in test.Binary)
			{
				Assert.AreEqual(0, b);
			}

			Assert.AreEqual(16, test.VarBinary.Length);
			foreach(byte b in test.VarBinary)
			{
				Assert.AreEqual(0, b);
			}

			Assert.AreEqual(32, test.Image.Length);
			foreach(byte b in test.Image)
			{
				Assert.AreEqual(0, b);
			}

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Select(typeof(BinaryTestObject), DoesNotExistValues) as BinaryTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void InsertRandomValues()
		{
			Random random = new Random();

			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Create(typeof(BinaryTestObject)) as BinaryTestObject;

			test.Binary = new byte[8];
			random.NextBytes(test.Binary);

			test.VarBinary = new byte[16];
			random.NextBytes(test.VarBinary);

			test.Image = new byte[2];
			random.NextBytes(test.Image);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Binary"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.BinaryTestObject.Image has no value")]
		public void InsertMissingValues()
		{
			Random random = new Random();

			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test = transaction.Create(typeof(BinaryTestObject)) as BinaryTestObject;

			test.Binary = new byte[8];
			random.NextBytes(test.Binary);

			test.VarBinary = new byte[16];
			random.NextBytes(test.VarBinary);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void UpdateRandomValues()
		{
			Random random = new Random();

			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test1 = transaction.Select(typeof(BinaryTestObject), UpdateValue) as BinaryTestObject;

			test1.Binary = new Byte[8];
			random.NextBytes(test1.Binary);

			byte b = test1.Binary[3];

			test1.VarBinary[2] = 16;
			test1.Image[1] = 69;

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			BinaryTestObject test2 = transaction2.Select(typeof(BinaryTestObject), UpdateValue) as BinaryTestObject;

			Assert.AreEqual(b, test2.Binary[3]);
			Assert.AreEqual(16, test2.VarBinary[2]);
			Assert.AreEqual(69, test2.Image[1]);

			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			Random random = new Random();

			int count = DataUtil.CountRows("Binary");

			ObjectTransaction transaction = manager.BeginTransaction();

			BinaryTestObject test1 = transaction.Create(typeof(BinaryTestObject)) as BinaryTestObject;

			test1.Binary = new byte[8];
			random.NextBytes(test1.Binary);

			test1.VarBinary = new byte[5];
			random.NextBytes(test1.VarBinary);

			test1.Image = new byte[1024];
			random.NextBytes(test1.Image);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Binary"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			BinaryTestObject test2 = transaction2.Select(typeof(BinaryTestObject), test1.Id) as BinaryTestObject;

			Assert.AreEqual(8, test2.Binary.Length);
			Assert.AreEqual(5, test2.VarBinary.Length);
			Assert.AreEqual(1024, test2.Image.Length);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Binary"));
		}
	}
}
