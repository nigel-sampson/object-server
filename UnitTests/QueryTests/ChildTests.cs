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
	public class ChildTests : ServicedComponent
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
		public void EmptyConditions()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new SetCondition("ChildObjects"));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityParentTestObject), query);

			Assert.AreEqual(1, objects.Count);
		}

		[Test]
		public void SimpleTest()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new SetCondition("ChildObjects", new Condition("ObjData", Expression.Equal, "A")));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityParentTestObject), query);

			Assert.AreEqual(1, objects.Count);
		}

		[Test]
		public void SimpleTestNoResults()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new SetCondition("ChildObjects", new Condition("ObjData", Expression.Equal, "X")));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityParentTestObject), query);

			Assert.AreEqual(0, objects.Count);
		}

		[Test]
		public void ParentChildTest()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new SetCondition("Parent.ChildObjects", new Condition("ObjData", Expression.Equal, "A")));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);			

			Assert.AreEqual(2, objects.Count);
		}

		[Test]
		public void MultipleSetConditions()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new SetCondition("ChildObjects", new Condition("ObjData", Expression.Equal, "A"), new Condition("ObjData", Expression.Equal, "X")));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityParentTestObject), query);

			Assert.AreEqual(0, objects.Count);
		}
	}
}
