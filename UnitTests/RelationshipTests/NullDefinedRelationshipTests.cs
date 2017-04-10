using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class NullDefinedRelationshipTests  : ServicedComponent
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
			int count = DataUtil.CountRows("NullDefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullDefinedParentTestObject parent = transaction.Select(typeof(NullDefinedParentTestObject), "Key2") as NullDefinedParentTestObject;
			Assert.AreEqual("Key2", parent.Id);	
			Assert.AreEqual("Data2", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
		
			Assert.AreEqual(count, DataUtil.CountRows("NullDefinedParents"));
		}

		[Test]
		public void SelectParentWithChildren()
		{
			int count = DataUtil.CountRows("NullDefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullDefinedParentTestObject parent = transaction.Select(typeof(NullDefinedParentTestObject), "Key1") as NullDefinedParentTestObject;
			Assert.AreEqual("Key1", parent.Id);	
			Assert.AreEqual("Data1", parent.ObjData);
			Assert.AreEqual(2, parent.ChildObjects.Count);
	
			NullDefinedChildTestObject obj1 = parent.ChildObjects[0] as NullDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", obj1.Id);
			Assert.AreEqual("CData1", obj1.ObjData);
			Assert.AreEqual("Key1", obj1.Parent.Id);
			Assert.AreEqual("Data1", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			NullDefinedChildTestObject obj2 = parent.ChildObjects[1] as NullDefinedChildTestObject;
			Assert.AreEqual("ChildKey2", obj2.Id);
			Assert.AreEqual("CData2", obj2.ObjData);
			Assert.AreEqual("Key1", obj2.Parent.Id);
			Assert.AreEqual("Data1", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
	
			Assert.AreEqual(count, DataUtil.CountRows("NullDefinedParents"));
		}

		[Test]
		public void SelectChild()
		{
			int count = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullDefinedChildTestObject child = transaction.Select(typeof(NullDefinedChildTestObject), "ChildKey1") as NullDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", child.Id);	
			Assert.AreEqual("CData1", child.ObjData);
			Assert.AreEqual("Key1", child.Parent.Id);	
			Assert.IsTrue(child.Parent.ChildObjects.Contains(child));

			Assert.AreEqual(count, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void SelectChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullDefinedChildTestObject child = transaction.Select(typeof(NullDefinedChildTestObject), "ChildKey3") as NullDefinedChildTestObject;
			Assert.AreEqual("ChildKey3", child.Id);	
			Assert.AreEqual("CData3", child.ObjData);
			Assert.IsNull(child.Parent);	

			Assert.AreEqual(count, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("NullDefinedParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullDefinedParentTestObject parent = transaction.Create(typeof(NullDefinedParentTestObject), "testkey1") as NullDefinedParentTestObject;
			parent.ObjData = "test";
			Assert.AreEqual(0, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullDefinedParents"));
		}

		[Test]
		public void InsertChildWithNoParent()
		{
			int count = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullDefinedChildTestObject child = transaction.Create(typeof(NullDefinedChildTestObject), "TestKey1") as NullDefinedChildTestObject;
			child.ObjData = "test";

			transaction.Commit();

			Assert.IsNull(child.Parent);

			Assert.AreEqual(count + 1, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void AddChild()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			NullDefinedParentTestObject parent = transaction.Select(typeof(NullDefinedParentTestObject), "Key2") as NullDefinedParentTestObject;
			Assert.AreEqual("Key2", parent.Id);	
			Assert.AreEqual("Data2", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);
	
			NullDefinedChildTestObject child = transaction.Create(typeof(NullDefinedChildTestObject), "CK1") as NullDefinedChildTestObject;
			child.ObjData = "test";
			child.Parent = parent;

			Assert.AreEqual("Key2", child.Parent.Id);
			Assert.AreEqual(1, parent.ChildObjects.Count);

			Assert.IsTrue(parent.ChildObjects.Contains(child));

			transaction.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount + 1, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void UpdateChild()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullDefinedChildTestObject child1 = transaction1.Select(typeof(NullDefinedChildTestObject), "ChildKey1") as NullDefinedChildTestObject;
			Assert.AreEqual("ChildKey1", child1.Id);	
			Assert.AreEqual("CData1", child1.ObjData);
			Assert.AreEqual("Key1", child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.ObjData = "X";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullDefinedChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullDefinedChildTestObject child2 = transaction2.Select(typeof(NullDefinedChildTestObject), "ChildKey1") as NullDefinedChildTestObject;

			Assert.AreEqual("ChildKey1", child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.AreEqual("Key1", child2.Parent.Id);	
			Assert.IsTrue(child2.Parent.ChildObjects.Contains(child2));

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void ShiftParents()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			NullDefinedParentTestObject parent1 = transaction1.Select(typeof(NullDefinedParentTestObject), "Key1") as NullDefinedParentTestObject;
			NullDefinedParentTestObject parent2 = transaction1.Select(typeof(NullDefinedParentTestObject), "Key2") as NullDefinedParentTestObject;

			NullDefinedChildTestObject child1 = transaction1.Select(typeof(NullDefinedChildTestObject), "ChildKey1") as NullDefinedChildTestObject;

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

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullDefinedChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			NullDefinedChildTestObject child2 = transaction2.Select(typeof(NullDefinedChildTestObject), "ChildKey1") as NullDefinedChildTestObject;

			Assert.AreEqual("ChildKey1", child2.Id);	
			Assert.AreEqual("CData1", child2.ObjData);
			Assert.AreEqual("Key2", child2.Parent.Id);	

			Assert.AreEqual(parentCount, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void InsertParentChild()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullDefinedParentTestObject parent = transaction.Create(typeof(NullDefinedParentTestObject), "Key6") as NullDefinedParentTestObject;
			parent.ObjData = "XXX";
			Assert.AreEqual(0, parent.ChildObjects.Count);

			for(int i = 0; i < 10; i++)
			{
				NullDefinedChildTestObject child = transaction.Create(typeof(NullDefinedChildTestObject), i) as NullDefinedChildTestObject;
				child.ObjData = i.ToString();
				child.Parent = parent;
			}
			Assert.AreEqual(10, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(parentCount + 1, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount + 10, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void DeleteParentWithoutCascade()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullDefinedParentTestObject parent = transaction.Select(typeof(NullDefinedParentTestObject), "Key1") as NullDefinedParentTestObject;

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("NullDefinedChildren"));
		}

		[Test]
		public void DeleteParent()
		{
			int parentCount = DataUtil.CountRows("NullDefinedParents");
			int childrenCount = DataUtil.CountRows("NullDefinedChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			NullDefinedParentTestObject parent = transaction.Select(typeof(NullDefinedParentTestObject), "Key1") as NullDefinedParentTestObject;

			foreach(NullDefinedChildTestObject child in parent.ChildObjects)
			{
				transaction.Delete(child);
			}

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("NullDefinedParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("NullDefinedChildren"));
		}
	}
}
