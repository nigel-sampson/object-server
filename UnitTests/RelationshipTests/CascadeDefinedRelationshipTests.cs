using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class CascadeDefinedRelationshipTests  : ServicedComponent
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
			int count = DataUtil.CountRows("DefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			CascadeDefinedParentTestObject parent = transaction.Select(typeof(CascadeDefinedParentTestObject), "Key2") as CascadeDefinedParentTestObject;
			Assert.AreEqual("Key2", parent.Id);	
			Assert.AreEqual("Data2", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
		
			Assert.AreEqual(count, DataUtil.CountRows("DefinedParents"));
		}

		[Test]
		public void SelectParentWithChildren()
		{
			int count = DataUtil.CountRows("DefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			CascadeDefinedParentTestObject parent = transaction.Select(typeof(CascadeDefinedParentTestObject), "Key1") as CascadeDefinedParentTestObject;
			Assert.AreEqual("Key1", parent.Id);	
			Assert.AreEqual("Data1", parent.ObjData);
			Assert.AreEqual(2, parent.ChildObjects.Count);
	
			CascadeDefinedChildTestObject obj1 = parent.ChildObjects[0] as CascadeDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", obj1.Id);
			Assert.AreEqual("CData1", obj1.ObjData);
			Assert.AreEqual("Key1", obj1.Parent.Id);
			Assert.AreEqual("Data1", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			CascadeDefinedChildTestObject obj2 = parent.ChildObjects[1] as CascadeDefinedChildTestObject;
			Assert.AreEqual("ChildKey2", obj2.Id);
			Assert.AreEqual("CData2", obj2.ObjData);
			Assert.AreEqual("Key1", obj2.Parent.Id);
			Assert.AreEqual("Data1", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
	
			Assert.AreEqual(count, DataUtil.CountRows("DefinedParents"));
		}

		[Test]
		public void SelectChild()
		{
			int count = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			CascadeDefinedChildTestObject child = transaction.Select(typeof(CascadeDefinedChildTestObject), "ChildKey1") as CascadeDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", child.Id);	
			Assert.AreEqual("CData1", child.ObjData);
			Assert.AreEqual("Key1", child.Parent.Id);	
			Assert.IsTrue(child.Parent.ChildObjects.Contains(child));

			Assert.AreEqual(count, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("DefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			CascadeDefinedParentTestObject parent = transaction.Create(typeof(CascadeDefinedParentTestObject), "testkey1") as CascadeDefinedParentTestObject;
			parent.ObjData = "test";
			Assert.AreEqual(0, parent.ChildObjects.Count);
			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("DefinedParents"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.CascadeDefinedChildTestObject.Parent has no value")]
		public void InsertChildWithNoParent()
		{
			ObjectTransaction transaction = manager.BeginTransaction();
			
			CascadeDefinedChildTestObject child = transaction.Create(typeof(CascadeDefinedChildTestObject), "TestKey1") as CascadeDefinedChildTestObject;
			child.ObjData = "test";

			transaction.Commit();
		}

		[Test]
		public void AddChild()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			CascadeDefinedParentTestObject parent = transaction.Select(typeof(CascadeDefinedParentTestObject), "Key2") as CascadeDefinedParentTestObject;
			Assert.AreEqual("Key2", parent.Id);	
			Assert.AreEqual("Data2", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);
	
			CascadeDefinedChildTestObject child = transaction.Create(typeof(CascadeDefinedChildTestObject), "CK1") as CascadeDefinedChildTestObject;
			child.ObjData = "test";
			child.Parent = parent;

			Assert.AreEqual("Key2", child.Parent.Id);
			Assert.AreEqual(1, parent.ChildObjects.Count);

			Assert.IsTrue(parent.ChildObjects.Contains(child));

			transaction.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount + 1, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void UpdateChild()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			CascadeDefinedChildTestObject child1 = transaction1.Select(typeof(CascadeDefinedChildTestObject), "ChildKey1") as CascadeDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", child1.Id);	
			Assert.AreEqual("CData1", child1.ObjData);
			Assert.AreEqual("Key1", child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.ObjData = "X";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("DefinedChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			CascadeDefinedChildTestObject child2 = transaction2.Select(typeof(CascadeDefinedChildTestObject), "ChildKey1") as CascadeDefinedChildTestObject;

			Assert.AreEqual("ChildKey1", child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.AreEqual("Key1", child2.Parent.Id);	
			Assert.IsTrue(child2.Parent.ChildObjects.Contains(child2));

			Assert.AreEqual(parentCount, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void ShiftParents()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			CascadeDefinedParentTestObject parent1 = transaction1.Select(typeof(CascadeDefinedParentTestObject), "Key1") as CascadeDefinedParentTestObject;
			CascadeDefinedParentTestObject parent2 = transaction1.Select(typeof(CascadeDefinedParentTestObject), "Key2") as CascadeDefinedParentTestObject;

			CascadeDefinedChildTestObject child1 = transaction1.Select(typeof(CascadeDefinedChildTestObject), "ChildKey1") as CascadeDefinedChildTestObject;

			Assert.AreEqual("ChildKey1", child1.Id);	
			Assert.AreEqual("CData1", child1.ObjData);
			Assert.AreEqual("Key1", child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent1, child1.Parent);
			Assert.IsTrue(parent1.ChildObjects.Contains(child1));
			Assert.IsFalse(parent2.ChildObjects.Contains(child1));

			child1.Parent = parent2;
			
			Assert.AreEqual("ChildKey1", child1.Id);	
			Assert.AreEqual("CData1", child1.ObjData);
			Assert.AreEqual("Key2", child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent2, child1.Parent);
			Assert.IsTrue(parent2.ChildObjects.Contains(child1));
			Assert.IsFalse(parent1.ChildObjects.Contains(child1));

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("DefinedChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			CascadeDefinedChildTestObject child2 = transaction2.Select(typeof(CascadeDefinedChildTestObject), "ChildKey1") as CascadeDefinedChildTestObject;

			Assert.AreEqual("ChildKey1", child2.Id);	
			Assert.AreEqual("CData1", child2.ObjData);
			Assert.AreEqual("Key2", child2.Parent.Id);	

			Assert.AreEqual(parentCount, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void InsertParentChild()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			CascadeDefinedParentTestObject parent = transaction.Create(typeof(CascadeDefinedParentTestObject), "Key6") as CascadeDefinedParentTestObject;
			parent.ObjData = "XXX";
			Assert.AreEqual(0, parent.ChildObjects.Count);
			for(int i = 0; i < 10; i++)
			{
				CascadeDefinedChildTestObject child = transaction.Create(typeof(CascadeDefinedChildTestObject), i) as CascadeDefinedChildTestObject;
				child.ObjData = i.ToString();
				child.Parent = parent;
			}
			Assert.AreEqual(10, parent.ChildObjects.Count);
			transaction.Commit();

			Assert.AreEqual(parentCount + 1, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount + 10, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void DeleteParentWithCascade()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			CascadeDefinedParentTestObject parent = transaction.Select(typeof(CascadeDefinedParentTestObject), "Key1") as CascadeDefinedParentTestObject;

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("DefinedChildren"));
		}

		[Test]
		public void DeleteParent()
		{
			int parentCount = DataUtil.CountRows("DefinedParents");
			int childrenCount = DataUtil.CountRows("DefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			CascadeDefinedParentTestObject parent = transaction.Select(typeof(CascadeDefinedParentTestObject), "Key1") as CascadeDefinedParentTestObject;

			foreach(CascadeDefinedChildTestObject child in parent.ChildObjects)
			{
				transaction.Delete(child);
			}

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("DefinedParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("DefinedChildren"));
		}
	}
}
