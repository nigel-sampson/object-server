using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class CircularReferenceTests : ServicedComponent
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
		public void CreateCircularReference()
		{
			int pA = DataUtil.CountRows("ParentA");
			int pB = DataUtil.CountRows("ParentB");
			int cA = DataUtil.CountRows("ChildA");

			ObjectTransaction transaction = manager.BeginTransaction();

			ParentBTestObject parentB = transaction.Create(typeof(ParentBTestObject)) as ParentBTestObject;
			parentB.ObjData = "B";

			ParentATestObject parentA = transaction.Create(typeof(ParentATestObject)) as ParentATestObject;
			parentA.ObjData = "A";
			parentA.ParentB = parentB;

			ChildATestObject childA = transaction.Create(typeof(ChildATestObject)) as ChildATestObject;
			childA.ParentA = parentA;
			childA.ParentB = parentB;

			transaction.Commit();

			Assert.AreEqual(pA + 1, DataUtil.CountRows("ParentA"));
			Assert.AreEqual(pB + 1, DataUtil.CountRows("ParentB"));
			Assert.AreEqual(cA + 1, DataUtil.CountRows("ChildA"));
		}

		[Test]
		public void DeleteCircularReference()
		{
			int pA = DataUtil.CountRows("ParentA");
			int pB = DataUtil.CountRows("ParentB");
			int cA = DataUtil.CountRows("ChildA");

			ObjectTransaction transaction = manager.BeginTransaction();

			ParentBTestObject parentB = transaction.Select(typeof(ParentBTestObject), 40) as ParentBTestObject;

			transaction.Delete(parentB);
			transaction.Commit();

			Assert.AreEqual(pA - 1, DataUtil.CountRows("ParentA"));
			Assert.AreEqual(pB - 1, DataUtil.CountRows("ParentB"));
			Assert.AreEqual(cA - 1, DataUtil.CountRows("ChildA"));
		}

		[Test]
		public void Create2ObjectCircularReference()
		{
			int pA = DataUtil.CountRows("ParentA");
			int pB = DataUtil.CountRows("ParentB");
			int cA = DataUtil.CountRows("ChildA");

			ObjectTransaction transaction = manager.BeginTransaction();

			ParentBTestObject parentB = transaction.Select(typeof(ParentBTestObject), 43) as ParentBTestObject;

			ParentATestObject parentA = transaction.Create(typeof(ParentATestObject)) as ParentATestObject;
			parentA.ObjData = "A";
			parentA.ParentB = parentB;

			ChildATestObject childA = transaction.Create(typeof(ChildATestObject)) as ChildATestObject;
			childA.ParentA = parentA;
			childA.ParentB = parentB;

			transaction.Commit();

			Assert.AreEqual(pA + 1, DataUtil.CountRows("ParentA"));
			Assert.AreEqual(pB, DataUtil.CountRows("ParentB"));
			Assert.AreEqual(cA + 1, DataUtil.CountRows("ChildA"));
		}

		[Test]
		public void Create1ObjectCircularReference()
		{
			int pA = DataUtil.CountRows("ParentA");
			int pB = DataUtil.CountRows("ParentB");
			int cA = DataUtil.CountRows("ChildA");

			ObjectTransaction transaction = manager.BeginTransaction();

			ParentBTestObject parentB = transaction.Select(typeof(ParentBTestObject), 45) as ParentBTestObject;
			ParentATestObject parentA = transaction.Select(typeof(ParentATestObject), 62) as ParentATestObject;

			Assert.IsNotNull(parentB);
			Assert.IsNotNull(parentA);

			ChildATestObject childA = transaction.Create(typeof(ChildATestObject)) as ChildATestObject;
			childA.ParentA = parentA;
			childA.ParentB = parentB;

			transaction.Commit();

			Assert.AreEqual(pA, DataUtil.CountRows("ParentA"));
			Assert.AreEqual(pB, DataUtil.CountRows("ParentB"));
			Assert.AreEqual(cA + 1, DataUtil.CountRows("ChildA"));
		}

		[Test]
		public void UpdateCircularReference()
		{
			int pA = DataUtil.CountRows("ParentA");
			int pB = DataUtil.CountRows("ParentB");
			int cA = DataUtil.CountRows("ChildA");

			ObjectTransaction transaction = manager.BeginTransaction();

			ParentBTestObject parentB = transaction.Select(typeof(ParentBTestObject), 45) as ParentBTestObject;
			parentB.ObjData = "Updated";

			ParentATestObject parentA = transaction.Select(typeof(ParentATestObject), 62) as ParentATestObject;
			parentA.ObjData = "Updated";

			ChildATestObject childA = transaction.Create(typeof(ChildATestObject)) as ChildATestObject;
			childA.ParentA = parentA;
			childA.ParentB = parentB;

			transaction.Commit();

			Assert.AreEqual(pA, DataUtil.CountRows("ParentA"));
			Assert.AreEqual(pB, DataUtil.CountRows("ParentB"));
			Assert.AreEqual(cA + 1, DataUtil.CountRows("ChildA"));
		}
	}
}
