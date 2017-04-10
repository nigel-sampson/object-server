using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullParentTests : ServicedComponent
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
		public void SelectParent()
		{
			int count = DataUtil.CountRows("NullParents");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullParentTestObject obj = transaction.Select(typeof(NullParentTestObject), 1) as NullParentTestObject;

			Assert.AreEqual(1, obj.Id);
			Assert.AreEqual(1, obj.Value);
			Assert.AreEqual(2, obj.ChildObjects.Count);

			Assert.AreEqual(count, DataUtil.CountRows("NullParents"));
		}

		[Test]
		public void SelectChildWithParent()
		{
			int count = DataUtil.CountRows("NullParents");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullChildTestObject obj = transaction.Select(typeof(NullChildTestObject), 1) as NullChildTestObject;

			Assert.AreEqual(1, obj.Id);
			Assert.AreEqual(1, obj.Value);
			Assert.AreEqual(1, obj.Parent.Id);

			Assert.AreEqual(count, DataUtil.CountRows("NullParents"));
		}

		[Test]
		public void SelectChildWithoutParent()
		{
			int count = DataUtil.CountRows("NullChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullChildTestObject obj = transaction.Select(typeof(NullChildTestObject), 3) as NullChildTestObject;

			Assert.AreEqual(3, obj.Id);
			Assert.AreEqual(1, obj.Value);
			Assert.IsNull(obj.Parent);

			Assert.AreEqual(count, DataUtil.CountRows("NullChildren"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "Could not delete, UnitTests.TestObjects.NullChildTestObject.Parent has DeleteAction.Throw")]
		public void DeleteParentWithChild()
		{
			int count = DataUtil.CountRows("NullParents");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullParentTestObject obj = transaction.Select(typeof(NullParentTestObject), 1) as NullParentTestObject;

			Assert.AreEqual(1, obj.Id);
			Assert.AreEqual(1, obj.Value);
			Assert.AreEqual(2, obj.ChildObjects.Count);

			transaction.Delete(obj);

			Assert.AreEqual(count, DataUtil.CountRows("NullParents"));
		}

		[Test]
		public void DeleteParentWithoutChild()
		{
			int count = DataUtil.CountRows("NullParents");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullParentTestObject obj = transaction.Select(typeof(NullParentTestObject), 2) as NullParentTestObject;

			Assert.AreEqual(2, obj.Id);
			Assert.AreEqual(2, obj.Value);
			Assert.AreEqual(0, obj.ChildObjects.Count);

			transaction.Delete(obj);

			transaction.Commit();

			Assert.AreEqual(count - 1, DataUtil.CountRows("NullParents"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("NullParents");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullParentTestObject obj = transaction.Create(typeof(NullParentTestObject)) as NullParentTestObject;

			obj.Value = 5;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullParents"));
		}

		[Test]
		public void InsertChildWithParent()
		{
			int count = DataUtil.CountRows("NullChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullChildTestObject obj = transaction.Create(typeof(NullChildTestObject)) as NullChildTestObject;

			obj.Value = 6;
			obj.Parent = transaction.Select(typeof(NullParentTestObject), 1) as NullParentTestObject;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullChildren"));
		}

		[Test]
		public void InsertChildWithoutParent()
		{
			int count = DataUtil.CountRows("NullChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullChildTestObject obj = transaction.Create(typeof(NullChildTestObject)) as NullChildTestObject;

			obj.Value = 8;

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullChildren"));
		}
	}
}
