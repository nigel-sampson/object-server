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
	public class SimpleTests : ServicedComponent
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
		public void SelectAll()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query();

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(4, objects.Count);
		}

		[Test]
		public void SingleEqualsCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.Equal, true));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(2, object1.Id);
			Assert.AreEqual("GHT", object1.Varchar);
			Assert.AreEqual(5, object1.Integer);
			Assert.AreEqual(true, object1.Boolean);
			Assert.AreEqual(new DateTime(1945, 8, 1), object1.Date);
			Assert.AreEqual(2, object1.NullableInteger);

			SimpleConstraintTestObject object2 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		public void DoubleEqualsCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.Equal, true), new Condition("Id", Expression.Equal, 4));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(1, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object1.Id);
			Assert.AreEqual("kp", object1.Varchar);
			Assert.AreEqual(100, object1.Integer);
			Assert.AreEqual(true, object1.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object1.Date);
			Assert.AreEqual(-1, object1.NullableInteger);
		}

		[Test]
		public void SingleNotEqualsCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.NotEqual, false));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(2, object1.Id);
			Assert.AreEqual("GHT", object1.Varchar);
			Assert.AreEqual(5, object1.Integer);
			Assert.AreEqual(true, object1.Boolean);
			Assert.AreEqual(new DateTime(1945, 8, 1), object1.Date);
			Assert.AreEqual(2, object1.NullableInteger);

			SimpleConstraintTestObject object2 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		public void SingleLikeCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Varchar", Expression.Like, "k%"));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(3, object1.Id);
			Assert.AreEqual("kl", object1.Varchar);
			Assert.AreEqual(3847, object1.Integer);
			Assert.AreEqual(false, object1.Boolean);
			Assert.AreEqual(new DateTime(2004, 9, 1), object1.Date);
			Assert.AreEqual(-1, object1.NullableInteger);

			SimpleConstraintTestObject object2 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		public void SingleNotLikeAndGreaterThanCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Varchar", Expression.NotLike, "k%"), new Condition("Integer", Expression.GreaterThan, 5));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(1, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(1, object1.Id);
			Assert.AreEqual("AX", object1.Varchar);
			Assert.AreEqual(6, object1.Integer);
			Assert.AreEqual(false, object1.Boolean);
			Assert.AreEqual(new DateTime(1981, 7, 11), object1.Date);
			Assert.AreEqual(1, object1.NullableInteger);
		}

		[Test]
		public void SingleIsNullCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("NullableInteger", Expression.IsNull));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object1 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(3, object1.Id);
			Assert.AreEqual("kl", object1.Varchar);
			Assert.AreEqual(3847, object1.Integer);
			Assert.AreEqual(false, object1.Boolean);
			Assert.AreEqual(new DateTime(2004, 9, 1), object1.Date);
			Assert.AreEqual(-1, object1.NullableInteger);

			SimpleConstraintTestObject object2 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "Invalid amount of parameters for operator IsNull")]
		public void InvalidNoValueCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("NullableInteger", Expression.IsNull, 1));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "Invalid amount of parameters for operator Like")]
		public void InvalidValueCondition()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("NullableInteger", Expression.Like));
		}

		[Test]
		public void SingleEqualsConditionWithOrder()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.Equal, true));
			query.Order = "Integer DESC";

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object1 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(2, object1.Id);
			Assert.AreEqual("GHT", object1.Varchar);
			Assert.AreEqual(5, object1.Integer);
			Assert.AreEqual(true, object1.Boolean);
			Assert.AreEqual(new DateTime(1945, 8, 1), object1.Date);
			Assert.AreEqual(2, object1.NullableInteger);

			SimpleConstraintTestObject object2 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		public void SelectTop1()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.Equal, true));
			query.Order = "Id DESC";
			query.Top = 1;

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(1, objects.Count);

			SimpleConstraintTestObject object2 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);
		}

		[Test]
		public void SelectTop10()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Boolean", Expression.Equal, true));
			query.Order = "Id DESC";
			query.Top = 10;

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);

			SimpleConstraintTestObject object2 = objects[0] as SimpleConstraintTestObject;
			Assert.AreEqual(4, object2.Id);
			Assert.AreEqual("kp", object2.Varchar);
			Assert.AreEqual(100, object2.Integer);
			Assert.AreEqual(true, object2.Boolean);
			Assert.AreEqual(new DateTime(2004, 10, 1), object2.Date);
			Assert.AreEqual(-1, object2.NullableInteger);

			SimpleConstraintTestObject object1 = objects[1] as SimpleConstraintTestObject;
			Assert.AreEqual(2, object1.Id);
			Assert.AreEqual("GHT", object1.Varchar);
			Assert.AreEqual(5, object1.Integer);
			Assert.AreEqual(true, object1.Boolean);
			Assert.AreEqual(new DateTime(1945, 8, 1), object1.Date);
			Assert.AreEqual(2, object1.NullableInteger);
		}

		[Test]
		public void Between()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Integer", Expression.Between, 5.5, 150));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);
		}

		[Test]
		public void In()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Integer", Expression.In, 5, 100));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects.Count);
		}

		[Test]
		public void Literal()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new LiteralCondition("Integer", " + 1 = 7"));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(1, objects.Count);
		}

		[Test]
		public void QueryUsedTwice()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Integer", Expression.In, 5, 100));

			ServerObjectCollection objects1 = transaction.Select(typeof(SimpleConstraintTestObject), query);
			ServerObjectCollection objects2 = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(2, objects1.Count);
			Assert.AreEqual(2, objects2.Count);
		}
	}
}
