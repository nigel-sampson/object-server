using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.DataTypeTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullableCharacterTests : ServicedComponent
	{
		private const string RandomValues = "{C85B6116-54B8-49D3-8544-527C083B64FE}";
		private const string EmptyValues = "{E59CAA75-3A0F-4688-8685-682D744AD37F}";
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
		public void SelectRandomValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Select(typeof(NullableCharacterTestObject), RandomValues) as NullableCharacterTestObject;

			Assert.AreEqual(new Guid(RandomValues), test.Id);
			Assert.AreEqual("A", test.Char);
			Assert.AreEqual("B", test.NChar);
			Assert.AreEqual("CDEF", test.VChar);
			Assert.AreEqual("GHIJ", test.NVChar);
			Assert.AreEqual("LMNOP", test.Text);
			Assert.AreEqual("QRSTUV", test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void SelectNonExistantValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Select(typeof(NullableCharacterTestObject), DoesNotExistValues) as NullableCharacterTestObject;

			Assert.IsNull(test);
			
			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void SelectNullValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");
			Assert.IsTrue(DataUtil.IsRowNull("NullableCharacters", "id", NullValues));

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Select(typeof(NullableCharacterTestObject), NullValues) as NullableCharacterTestObject;

			Assert.AreEqual(new Guid(NullValues), test.Id);
			Assert.AreEqual("x", test.Char);
			Assert.AreEqual("z", test.NChar);
			Assert.AreEqual("<null value>", test.VChar);
			Assert.AreEqual("<null n value>", test.NVChar);
			Assert.AreEqual("<null text value>", test.Text);
			Assert.AreEqual("<null ntext value>", test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void SelectEmptyValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Select(typeof(NullableCharacterTestObject), EmptyValues) as NullableCharacterTestObject;

			Assert.AreEqual(new Guid(EmptyValues), test.Id);
			Assert.AreEqual(" ", test.Char);
			Assert.AreEqual(" ", test.NChar);
			Assert.AreEqual(String.Empty, test.VChar);
			Assert.AreEqual(String.Empty, test.NVChar);
			Assert.AreEqual(String.Empty, test.Text);
			Assert.AreEqual(String.Empty, test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void CreateNullValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Create(typeof(NullableCharacterTestObject)) as NullableCharacterTestObject;

			Assert.AreEqual("x", test.Char);
			Assert.AreEqual("z", test.NChar);
			Assert.AreEqual("<null value>", test.VChar);
			Assert.AreEqual("<null n value>", test.NVChar);
			Assert.AreEqual("<null text value>", test.Text);
			Assert.AreEqual("<null ntext value>", test.NText);

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void InsertRandomValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test = transaction.Create(typeof(NullableCharacterTestObject)) as NullableCharacterTestObject;

			test.Char = "C";
			test.NChar = "X";
			test.VChar = "Y";
			test.NVChar = "Z";
			test.Text = "BGH";
			test.NText = "VFG";

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void InsertNullImplicitValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableCharacterTestObject test1 = transaction1.Create(typeof(NullableCharacterTestObject)) as NullableCharacterTestObject;

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableCharacters"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableCharacters", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableCharacterTestObject test2 = transaction2.Select(typeof(NullableCharacterTestObject), test1.Id) as NullableCharacterTestObject;

			Assert.AreEqual("x", test2.Char);
			Assert.AreEqual("z", test2.NChar);
			Assert.AreEqual("<null value>", test2.VChar);
			Assert.AreEqual("<null n value>", test2.NVChar);
			Assert.AreEqual("<null text value>", test2.Text);
			Assert.AreEqual("<null ntext value>", test2.NText);
		}

		[Test]
		public void InsertNullExplicitValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullableCharacterTestObject test1 = transaction1.Create(typeof(NullableCharacterTestObject)) as NullableCharacterTestObject;

			test1.Char = "x";
			test1.NChar = "z";
			test1.VChar = "<null value>";
			test1.NVChar = "<null n value>";
			test1.Text = "<null text value>";
			test1.NText = "<null ntext value>";

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableCharacters"));
			Assert.IsTrue(DataUtil.IsRowNull("NullableCharacters", "id", test1.Id));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			
			NullableCharacterTestObject test2 = transaction2.Select(typeof(NullableCharacterTestObject), test1.Id) as NullableCharacterTestObject;

			Assert.AreEqual("x", test2.Char);
			Assert.AreEqual("z", test2.NChar);
			Assert.AreEqual("<null value>", test2.VChar);
			Assert.AreEqual("<null n value>", test2.NVChar);
			Assert.AreEqual("<null text value>", test2.Text);
			Assert.AreEqual("<null ntext value>", test2.NText);
		}

		[Test]
		public void UpdateValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullableCharacterTestObject test1 = transaction.Select(typeof(NullableCharacterTestObject), UpdateValue) as NullableCharacterTestObject;
	
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

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableCharacterTestObject test2 = transaction2.Select(typeof(NullableCharacterTestObject), UpdateValue) as NullableCharacterTestObject;

			Assert.AreEqual(new Guid(UpdateValue), test2.Id);
			Assert.AreEqual("Y", test2.Char);
			Assert.AreEqual("Y", test2.NChar);
			Assert.AreEqual("YYY", test2.VChar);
			Assert.AreEqual("YYY", test2.NVChar);
			Assert.AreEqual("YYY", test2.Text);
			Assert.AreEqual("YYY", test2.NText);

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}

		[Test]
		public void DeleteRandomValues()
		{
			int count = DataUtil.CountRows("NullableCharacters");

			ObjectTransaction transaction1 = manager.BeginTransaction();
			NullableCharacterTestObject test1 = transaction1.Create(typeof(NullableCharacterTestObject)) as NullableCharacterTestObject;

			test1.Char = "z";
			test1.NChar = "z";
			test1.VChar = "zzz";
			test1.NVChar = "zzz";
			test1.Text = "zzz";
			test1.NText = "zzz";

			transaction1.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullableCharacters"));

			ObjectTransaction transaction2 = manager.BeginTransaction();
			NullableCharacterTestObject test2 = transaction2.Select(typeof(NullableCharacterTestObject), test1.Id) as NullableCharacterTestObject;

			Assert.AreEqual(test1.Id, test2.Id);
			Assert.AreEqual("z", test2.Char);
			Assert.AreEqual("z", test2.NChar);
			Assert.AreEqual("zzz", test2.VChar);
			Assert.AreEqual("zzz", test2.NVChar);
			Assert.AreEqual("zzz", test2.Text);
			Assert.AreEqual("zzz", test2.NText);

			transaction2.Delete(test2);
			transaction2.Commit();

			Assert.AreEqual(count, DataUtil.CountRows("NullableCharacters"));
		}
	}
}
