using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class CharacterTests : ServicedComponent
	{
		private const string RandomValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string EmptyValues = "{E59CAA75-3A0F-4688-8685-682D744AD37F}";
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
		public void SelectRandomValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test = transaction.Select(typeof(CharacterTestObject), RandomValues) as CharacterTestObject;

			Assert.AreEqual(new Guid(RandomValues), test.Id);
			Assert.AreEqual("A", test.Char);
			Assert.AreEqual("B", test.NChar);
			Assert.AreEqual("CDEF", test.VChar);
			Assert.AreEqual("GHIJ", test.NVChar);
			Assert.AreEqual("LMNOP", test.Text);
			Assert.AreEqual("QRSTUV", test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("Characters"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test = transaction.Select(typeof(CharacterTestObject), DoesNotExistValues) as CharacterTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("Characters"));
		}

		[Test]
		public void SelectEmptyValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test = transaction.Select(typeof(CharacterTestObject), EmptyValues) as CharacterTestObject;

			Assert.AreEqual(new Guid(EmptyValues), test.Id);
			Assert.AreEqual(" ", test.Char);
			Assert.AreEqual(" ", test.NChar);
			Assert.AreEqual(String.Empty, test.VChar);
			Assert.AreEqual(String.Empty, test.NVChar);
			Assert.AreEqual(String.Empty, test.Text);
			Assert.AreEqual(String.Empty, test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("Characters"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.CharacterTestObject.Char has no value")]
		public void InsertMissingValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test = transaction.Create(typeof(CharacterTestObject)) as CharacterTestObject;

			test.NChar = "X";
			test.VChar = "Y";
			test.NVChar = "Z";
			test.Text = "BGH";
			test.NText = "VFG";

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Characters"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test = transaction.Create(typeof(CharacterTestObject)) as CharacterTestObject;

			test.Char = "C";
			test.NChar = "X";
			test.VChar = "Y";
			test.NVChar = "Z";
			test.Text = "BGH";
			test.NText = "VFG";

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Characters"));
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction = manager.BeginTransaction();

			CharacterTestObject test1 = transaction.Select(typeof(CharacterTestObject), UpdateValue) as CharacterTestObject;
	
			Assert.AreEqual(new Guid(UpdateValue), test1.Id);
			Assert.AreEqual("X", test1.Char);
			Assert.AreEqual("X", test1.NChar);
			Assert.AreEqual("XXX", test1.VChar);
			Assert.AreEqual("XXX", test1.NVChar);
			Assert.AreEqual("XXX", test1.Text);
			Assert.AreEqual("XXX", test1.NText);

			test1.Char = "Y";
			test1.NChar = "Y";
			test1.VChar = "YYY";
			test1.NVChar = "YYY";
			test1.Text = "YYY";
			test1.NText = "YYY";

			transaction.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Characters"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			CharacterTestObject test2 = transaction2.Select(typeof(CharacterTestObject), UpdateValue) as CharacterTestObject;

			Assert.AreEqual(new Guid(UpdateValue), test2.Id);
			Assert.AreEqual("Y", test2.Char);
			Assert.AreEqual("Y", test2.NChar);
			Assert.AreEqual("YYY", test2.VChar);
			Assert.AreEqual("YYY", test2.NVChar);
			Assert.AreEqual("YYY", test2.Text);
			Assert.AreEqual("YYY", test2.NText);

			Assert.AreEqual(count, DataUtil.CountRows("Characters"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("Characters");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			CharacterTestObject test1 = transaction1.Create(typeof(CharacterTestObject)) as CharacterTestObject;

			test1.Char = "z";
			test1.NChar = "z";
			test1.VChar = "zzz";
			test1.NVChar = "zzz";
			test1.Text = "zzz";
			test1.NText = "zzz";

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("Characters"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			CharacterTestObject test2 = transaction2.Select(typeof(CharacterTestObject), test1.Id) as CharacterTestObject;

			Assert.AreEqual(test1.Id, test2.Id);
			Assert.AreEqual("z", test2.Char);
			Assert.AreEqual("z", test2.NChar);
			Assert.AreEqual("zzz", test2.VChar);
			Assert.AreEqual("zzz", test2.NVChar);
			Assert.AreEqual("zzz", test2.Text);
			Assert.AreEqual("zzz", test2.NText);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("Characters"));
		}
	}
}
