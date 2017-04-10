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
	public class GroupTests
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
		public void Or()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new ConditionGroup(ConditionGroupType.Or, new Condition("Boolean", Expression.Equal, true), new Condition("Boolean", Expression.Equal, false)));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(4, objects.Count);
		}

		[Test]
		public void And()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new ConditionGroup(ConditionGroupType.And, new Condition("Boolean", Expression.Equal, true), new Condition("Boolean", Expression.Equal, false)));

			ServerObjectCollection objects = transaction.Select(typeof(SimpleConstraintTestObject), query);

			Assert.AreEqual(0, objects.Count);
		}

		[Test]
		public void AndParents()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new ConditionGroup(ConditionGroupType.And, new Condition("Parent.Id", Expression.Equal, 3), new Condition("ObjData", Expression.NotEqual, "A")));

			ServerObjectCollection objects = transaction.Select(typeof(IdentityChildTestObject), query);

			Assert.AreEqual(1, objects.Count);
		}
	}
}
