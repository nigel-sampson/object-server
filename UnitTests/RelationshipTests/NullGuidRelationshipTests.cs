using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullGuidRelationshipTests : ServicedComponent
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
		public void SelectParentWithoutChildren()
		{
			int count = DataUtil.CountRows("NullGuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullGuidParentTestObject parent = transaction.Select(typeof(NullGuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as NullGuidParentTestObject;
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), parent.Id);	
			Assert.AreEqual("J", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
		
			Assert.AreEqual(count, DataUtil.CountRows("NullGuidParents"));
		}

		[Test]
		public void SelectParentWithChildren()
		{
			int count = DataUtil.CountRows("NullGuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullGuidParentTestObject parent = transaction.Select(typeof(NullGuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as NullGuidParentTestObject;
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), parent.Id);	
			Assert.AreEqual("I", parent.ObjData);
			Assert.AreEqual(2, parent.ChildObjects.Count);
	
			NullGuidChildTestObject obj1 = parent.ChildObjects[0] as NullGuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), obj1.Id);
			Assert.AreEqual("P", obj1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), obj1.Parent.Id);
			Assert.AreEqual("I", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			NullGuidChildTestObject obj2 = parent.ChildObjects[1] as NullGuidChildTestObject;
			Assert.AreEqual(new Guid("{D513049E-13CC-42DA-83D7-4848AF7E3D0E}"), obj2.Id);
			Assert.AreEqual("Q", obj2.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), obj2.Parent.Id);
			Assert.AreEqual("I", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
	
			Assert.AreEqual(count, DataUtil.CountRows("NullGuidParents"));
		}

		[Test]
		public void SelectChild()
		{
			int count = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullGuidChildTestObject child = transaction.Select(typeof(NullGuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as NullGuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child.Id);	
			Assert.AreEqual("P", child.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child.Parent.Id);	
			Assert.IsTrue(child.Parent.ChildObjects.Contains(child));

			Assert.AreEqual(count, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void SelectChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullGuidChildTestObject child = transaction.Select(typeof(NullGuidChildTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as NullGuidChildTestObject;
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child.Id);	
			Assert.AreEqual("R", child.ObjData);
			Assert.IsNull(child.Parent);	

			Assert.AreEqual(count, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("NullGuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullGuidParentTestObject parent = transaction.Create(typeof(NullGuidParentTestObject)) as NullGuidParentTestObject;
			parent.ObjData = "test";
			Assert.AreEqual(0, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullGuidParents"));
		}

		[Test]
		public void InsertChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullGuidChildTestObject child = transaction.Create(typeof(NullGuidChildTestObject)) as NullGuidChildTestObject;
			child.ObjData = "test";

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void AddChild()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullGuidParentTestObject parent = transaction.Select(typeof(NullGuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as NullGuidParentTestObject;
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), parent.Id);	
			Assert.AreEqual("J", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
	
			NullGuidChildTestObject child = transaction.Create(typeof(NullGuidChildTestObject)) as NullGuidChildTestObject;
			child.ObjData = "test";
			child.Parent = parent;

			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child.Parent.Id);
			Assert.AreEqual(1, parent.ChildObjects.Count);

			Assert.IsTrue(parent.ChildObjects.Contains(child));

			transaction.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount + 1, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void UpdateChild()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullGuidChildTestObject child1 = transaction1.Select(typeof(NullGuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as NullGuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("P", child1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.ObjData = "X";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullGuidChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullGuidChildTestObject child2 = transaction2.Select(typeof(NullGuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as NullGuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child2.Parent.Id);	
			Assert.IsTrue(child2.Parent.ChildObjects.Contains(child2));

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void ShiftParents()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullGuidParentTestObject parent1 = transaction1.Select(typeof(NullGuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as NullGuidParentTestObject;
			NullGuidParentTestObject parent2 = transaction1.Select(typeof(NullGuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as NullGuidParentTestObject;

			NullGuidChildTestObject child1 = transaction1.Select(typeof(NullGuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as NullGuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("P", child1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent1, child1.Parent);
			Assert.IsTrue(parent1.ChildObjects.Contains(child1));
			Assert.IsFalse(parent2.ChildObjects.Contains(child1));

			child1.Parent = parent2;
			
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("P", child1.ObjData);
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent2, child1.Parent);
			Assert.IsTrue(parent2.ChildObjects.Contains(child1));
			Assert.IsFalse(parent1.ChildObjects.Contains(child1));

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullGuidChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullGuidChildTestObject child2 = transaction2.Select(typeof(NullGuidChildTestObject), new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}")) as NullGuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child2.Id);	
			Assert.AreEqual("P", child2.ObjData);
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child2.Parent.Id);

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void InsertParentChild()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullGuidParentTestObject parent = transaction.Create(typeof(NullGuidParentTestObject)) as NullGuidParentTestObject;
			parent.ObjData = "XXX";

			Assert.AreEqual(0, parent.ChildObjects.Count);

			for(int i = 0; i < 10; i++)
			{
				NullGuidChildTestObject child = transaction.Create(typeof(NullGuidChildTestObject)) as NullGuidChildTestObject;
				child.ObjData = i.ToString();
				child.Parent = parent;
			}

			Assert.AreEqual(10, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(parentCount + 1, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount + 10, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void DeleteParentWithCascade()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullGuidParentTestObject parent = transaction.Select(typeof(NullGuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as NullGuidParentTestObject;

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullGuidChildren"));
		}

		[Test]
		public void DeleteParent()
		{
			int parentCount = DataUtil.CountRows("NullGuidParents");
			int childrenCount = DataUtil.CountRows("NullGuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullGuidParentTestObject parent = transaction.Select(typeof(NullGuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as NullGuidParentTestObject;

			foreach(NullGuidChildTestObject child in parent.ChildObjects)
			{
				transaction.Delete(child);
			}

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullGuidParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("NullGuidChildren"));
		}
	}
}
