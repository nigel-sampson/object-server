using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class MiscTests : ServicedComponent
	{
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
		public void PreloadSchemas()
		{
			ObjectManager.PreloadSchemas("UnitTests");
		}

		[Test]
		public void DoubleCommit()
		{
			ObjectTransaction transaction1 = manager.BeginTransaction();
			SimpleConstraintTestObject obj1 = transaction1.Select(typeof(SimpleConstraintTestObject), 1) as SimpleConstraintTestObject;
			obj1.Varchar = "XXX";
			transaction1.Commit();
			obj1.Varchar = "YYY";
			transaction1.Commit();

			ObjectTransaction transaction2 = manager.BeginTransaction();
			SimpleConstraintTestObject obj2 = transaction2.Select(typeof(SimpleConstraintTestObject), 1) as SimpleConstraintTestObject;

			Assert.AreEqual("YYY", obj2.Varchar);
		}
	}
}
