using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullIdentityRelationshipTests : ServicedComponent
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
			int count = DataUtil.CountRows("NullIdentityParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullIdentityParentTestObject parent = transaction.Select(typeof(NullIdentityParentTestObject), 2) as NullIdentityParentTestObject;
			Assert.AreEqual(2, parent.Id);	
			Assert.AreEqual("B", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
		
			Assert.AreEqual(count, DataUtil.CountRows("NullIdentityParents"));
		}

		[Test]
		public void SelectParentWithChildren()
		{
			int count = DataUtil.CountRows("NullIdentityParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullIdentityParentTestObject parent = transaction.Select(typeof(NullIdentityParentTestObject), 1) as NullIdentityParentTestObject;
			Assert.AreEqual(1, parent.Id);	
			Assert.AreEqual("A", parent.ObjData);
			Assert.AreEqual(2, parent.ChildObjects.Count);
	
			NullIdentityChildTestObject obj1 = parent.ChildObjects[0] as NullIdentityChildTestObject;
			Assert.AreEqual(1, obj1.Id);
			Assert.AreEqual("A", obj1.ObjData);
			Assert.AreEqual(1, obj1.Parent.Id);
			Assert.AreEqual("A", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			NullIdentityChildTestObject obj2 = parent.ChildObjects[1] as NullIdentityChildTestObject;
			Assert.AreEqual(2, obj2.Id);
			Assert.AreEqual("B", obj2.ObjData);
			Assert.AreEqual(1, obj2.Parent.Id);
			Assert.AreEqual("A", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
	
			Assert.AreEqual(count, DataUtil.CountRows("NullIdentityParents"));
		}

		[Test]
		public void SelectChild()
		{
			int count = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullIdentityChildTestObject child = transaction.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;
			Assert.AreEqual(1, child.Id);	
			Assert.AreEqual("A", child.ObjData);
			Assert.AreEqual(1, child.Parent.Id);	
			Assert.IsTrue(child.Parent.ChildObjects.Contains(child));

			Assert.AreEqual(count, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void SelectChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullIdentityChildTestObject child = transaction.Select(typeof(NullIdentityChildTestObject), 3) as NullIdentityChildTestObject;
			Assert.AreEqual(3, child.Id);	
			Assert.AreEqual("X", child.ObjData);
			Assert.IsNull(child.Parent);	

			Assert.AreEqual(count, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("NullIdentityParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullIdentityParentTestObject parent = transaction.Create(typeof(NullIdentityParentTestObject)) as NullIdentityParentTestObject;
			parent.ObjData = "test";
			Assert.AreEqual(0, parent.ChildObjects.Count);
			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullIdentityParents"));
		}

		[Test]
		public void InsertChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullIdentityChildTestObject child = transaction.Create(typeof(NullIdentityChildTestObject)) as NullIdentityChildTestObject;
			child.ObjData = "test";

			transaction.Commit();

			Assert.IsNull(child.Parent);

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void AddChild()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullIdentityParentTestObject parent = transaction.Select(typeof(NullIdentityParentTestObject), 2) as NullIdentityParentTestObject;
			Assert.AreEqual(2, parent.Id);	
			Assert.AreEqual("B", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);
	
			NullIdentityChildTestObject child = transaction.Create(typeof(NullIdentityChildTestObject)) as NullIdentityChildTestObject;
			child.ObjData = "test";
			child.Parent = parent;

			Assert.AreEqual(2, child.Parent.Id, 4);
			Assert.AreEqual(1, parent.ChildObjects.Count);

			Assert.IsTrue(parent.ChildObjects.Contains(child));

			transaction.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount + 1, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void UpdateChild()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullIdentityChildTestObject child1 = transaction1.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;
			Assert.AreEqual(1, child1.Id);	
			Assert.AreEqual("A", child1.ObjData);
			Assert.AreEqual(1, child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.ObjData = "X";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullIdentityChildTestObject child2 = transaction2.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;

			Assert.AreEqual(1, child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.AreEqual(1, child2.Parent.Id);	
			Assert.IsTrue(child2.Parent.ChildObjects.Contains(child2));

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void DetachChild()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullIdentityParentTestObject parent1 = transaction1.Select(typeof(NullIdentityParentTestObject), 1) as NullIdentityParentTestObject;
			NullIdentityChildTestObject child1 = transaction1.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;

			Assert.AreEqual(1, child1.Id);	
			Assert.AreEqual("A", child1.ObjData);
			Assert.AreEqual(1, child1.Parent.Id);	
			Assert.AreEqual(child1.Parent, parent1);
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.Parent = null;
			child1.ObjData = "X";

			parent1.ObjData = "x";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullIdentityChildTestObject child2 = transaction2.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;

			Assert.AreEqual(1, child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.IsNull(child2.Parent);

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void ShiftParents()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullIdentityParentTestObject parent1 = transaction1.Select(typeof(NullIdentityParentTestObject), 1) as NullIdentityParentTestObject;
			NullIdentityParentTestObject parent2 = transaction1.Select(typeof(NullIdentityParentTestObject), 2) as NullIdentityParentTestObject;

			NullIdentityChildTestObject child1 = transaction1.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;

			Assert.AreEqual(1, child1.Id);	
			Assert.AreEqual("A", child1.ObjData);
			Assert.AreEqual(1, child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent1, child1.Parent);
			Assert.IsTrue(parent1.ChildObjects.Contains(child1));
			Assert.IsFalse(parent2.ChildObjects.Contains(child1));

			child1.Parent = parent2;
			
			Assert.AreEqual(1, child1.Id);	
			Assert.AreEqual("A", child1.ObjData);
			Assert.AreEqual(2, child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent2, child1.Parent);
			Assert.IsTrue(parent2.ChildObjects.Contains(child1));
			Assert.IsFalse(parent1.ChildObjects.Contains(child1));

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullIdentityChildTestObject child2 = transaction2.Select(typeof(NullIdentityChildTestObject), 1) as NullIdentityChildTestObject;

			Assert.AreEqual(1, child2.Id);	
			Assert.AreEqual("A", child2.ObjData);
			Assert.AreEqual(2, child2.Parent.Id);	

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void InsertParentChild()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullIdentityParentTestObject parent = transaction.Create(typeof(NullIdentityParentTestObject)) as NullIdentityParentTestObject;
			parent.ObjData = "XXX";
			Assert.AreEqual(0, parent.ChildObjects.Count);
			for(int i = 0; i < 10; i++)
			{
				NullIdentityChildTestObject child = transaction.Create(typeof(NullIdentityChildTestObject)) as NullIdentityChildTestObject;
				child.ObjData = i.ToString();
				child.Parent = parent;
			}
			Assert.AreEqual(10, parent.ChildObjects.Count);
			transaction.Commit();

			Assert.AreEqual(parentCount + 1, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount + 10, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void DeleteParentWithoutCascade()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullIdentityParentTestObject parent = transaction.Select(typeof(NullIdentityParentTestObject), 1) as NullIdentityParentTestObject;

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullIdentityChildren"));
		}

		[Test]
		public void DeleteParent()
		{
			int parentCount = DataUtil.CountRows("NullIdentityParents");
			int childrenCount = DataUtil.CountRows("NullIdentityChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullIdentityParentTestObject parent = transaction.Select(typeof(NullIdentityParentTestObject), 1) as NullIdentityParentTestObject;

			foreach(NullIdentityChildTestObject child in parent.ChildObjects)
			{
				transaction.Delete(child);
			}

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullIdentityParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("NullIdentityChildren"));
		}
	}
}
