using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;
using Nichevo.ObjectServer.Queries;

namespace UnitTests.QueryTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class ParentTests : ServicedComponent
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
		public void SingleParentCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Parent.ObjData", Expression.Equal, "A"));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(2, objects.Count);

			IdentityChildTestObject obj1 = objects[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			IdentityChildTestObject obj2 = objects[1] as IdentityChildTestObject;
			Assert.AreEqual(2, obj2.Id);
			Assert.AreEqual("B", obj2.ObjData);
			Assert.AreEqual(3, obj2.Parent.Id);
			Assert.AreEqual("A", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
		}

		[Test]
		public void SelectOnForeignKey()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Parent", Expression.Equal, 3));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(2, objects.Count);

			IdentityChildTestObject obj1 = objects[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			IdentityChildTestObject obj2 = objects[1] as IdentityChildTestObject;
			Assert.AreEqual(2, obj2.Id);
			Assert.AreEqual("B", obj2.ObjData);
			Assert.AreEqual(3, obj2.Parent.Id);
			Assert.AreEqual("A", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
		}

		[Test]
		public void DoubleParentCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Parent.Id", Expression.Equal, 3), new Condition("Parent.ObjData", Expression.Equal, "A"));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(2, objects.Count);

			IdentityChildTestObject obj1 = objects[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			IdentityChildTestObject obj2 = objects[1] as IdentityChildTestObject;
			Assert.AreEqual(2, obj2.Id);
			Assert.AreEqual("B", obj2.ObjData);
			Assert.AreEqual(3, obj2.Parent.Id);
			Assert.AreEqual("A", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
		}

		[Test]
		public void ParentChildCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Parent.ObjData", Expression.Equal, "A"), new Condition("ObjData", Expression.Equal, "A"));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(1, objects.Count);

			IdentityChildTestObject obj1 = objects[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);
		}

		[Test]
		public void Literal()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new LiteralCondition("Parent.ObjData", " + 'X' = 'AX'"));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(2, objects.Count);

			IdentityChildTestObject obj1 = objects[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);
		}

		[Test]
		public void QueryUsedTwice()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new LiteralCondition("Parent.ObjData", " + 'X' = 'AX'"));

			query.Order = "Parent.ObjData";

			ServerObjectCollection objects1 = transaction.Select(typeof(IdentityChildTestObject), query);
			ServerObjectCollection objects2 = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(2, objects1.Count);
			Assert.AreEqual(2, objects2.Count);

			IdentityChildTestObject obj1 = objects1[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(3, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			IdentityChildTestObject obj2 = objects2[0] as IdentityChildTestObject;
			Assert.AreEqual(1, obj2.Id);
			Assert.AreEqual("A", obj2.ObjData);
			Assert.AreEqual(3, obj2.Parent.Id);
			Assert.AreEqual("A", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
		}
	}
}
