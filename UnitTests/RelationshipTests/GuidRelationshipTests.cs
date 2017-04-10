using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

namespace UnitTests.RelationshipTests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class GuidRelationshipTests : ServicedComponent
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
			int count = DataUtil.CountRows("GuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			GuidParentTestObject parent = transaction.Select(typeof(GuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as GuidParentTestObject;
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), parent.Id);	
			Assert.AreEqual("Y", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
		
			Assert.AreEqual(count, DataUtil.CountRows("GuidParents"));
		}

		[Test]
		public void SelectParentWithChildren()
		{
			int count = DataUtil.CountRows("GuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			GuidParentTestObject parent = transaction.Select(typeof(GuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as GuidParentTestObject;
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), parent.Id);	
			Assert.AreEqual("X", parent.ObjData);
			Assert.AreEqual(2, parent.ChildObjects.Count);
	
			GuidChildTestObject obj1 = parent.ChildObjects[0] as GuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), obj1.Id);
			Assert.AreEqual("1", obj1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), obj1.Parent.Id);
			Assert.AreEqual("X", obj1.Parent.ObjData);
			Assert.AreEqual(2, obj1.Parent.ChildObjects.Count);

			GuidChildTestObject obj2 = parent.ChildObjects[1] as GuidChildTestObject;
			Assert.AreEqual(new Guid("{D513049E-13CC-42DA-83D7-4848AF7E3D0E}"), obj2.Id);
			Assert.AreEqual("0", obj2.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), obj2.Parent.Id);
			Assert.AreEqual("X", obj2.Parent.ObjData);
			Assert.AreEqual(2, obj2.Parent.ChildObjects.Count);
	
			Assert.AreEqual(count, DataUtil.CountRows("GuidParents"));
		}

		[Test]
		public void SelectChild()
		{
			int count = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidChildTestObject child = transaction.Select(typeof(GuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as GuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child.Id);	
			Assert.AreEqual("1", child.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child.Parent.Id);	
			Assert.IsTrue(child.Parent.ChildObjects.Contains(child));

			Assert.AreEqual(count, DataUtil.CountRows("GuidChildren"));
		}

		[Test]
		public void InsertParent()
		{
			int count = DataUtil.CountRows("GuidParents");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			GuidParentTestObject parent = transaction.Create(typeof(GuidParentTestObject)) as GuidParentTestObject;
			parent.ObjData = "test";
			Assert.AreEqual(0, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(count + 1, DataUtil.CountRows("GuidParents"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "UnitTests.TestObjects.GuidChildTestObject.Parent has no value")]
		public void InsertChildWithNoParent()
		{
			ObjectTransaction transaction = manager.BeginTransaction();
			
			GuidChildTestObject child = transaction.Create(typeof(GuidChildTestObject)) as GuidChildTestObject;
			child.ObjData = "test";

			transaction.Commit();
		}

		[Test]
		public void AddChild()
		{
			int parentCount = DataUtil.CountRows("GuidParents");
			int childrenCount = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();
			
			GuidParentTestObject parent = transaction.Select(typeof(GuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as GuidParentTestObject;
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), parent.Id);	
			Assert.AreEqual("Y", parent.ObjData);
			Assert.AreEqual(0, parent.ChildObjects.Count);	
	
			GuidChildTestObject child = transaction.Create(typeof(GuidChildTestObject)) as GuidChildTestObject;
			child.ObjData = "test";
			child.Parent = parent;

			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child.Parent.Id);
			Assert.AreEqual(1, parent.ChildObjects.Count);

			Assert.IsTrue(parent.ChildObjects.Contains(child));

			transaction.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount + 1, DataUtil.CountRows("GuidChildren"));
		}

		[Test]
		public void UpdateChild()
		{
			int parentCount = DataUtil.CountRows("GuidParents");
			int childrenCount = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			GuidChildTestObject child1 = transaction1.Select(typeof(GuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as GuidChildTestObject;
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("1", child1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));

			child1.ObjData = "X";

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("GuidChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			GuidChildTestObject child2 = transaction2.Select(typeof(GuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as GuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child2.Id);	
			Assert.AreEqual("X", child2.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child2.Parent.Id);	
			Assert.IsTrue(child2.Parent.ChildObjects.Contains(child2));

			Assert.AreEqual(parentCount, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("GuidChildren"));
		}

		[Test]
		public void ShiftParents()
		{
			int parentCount = DataUtil.CountRows("GuidParents");
			int childrenCount = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction1 = manager.BeginTransaction();

			GuidParentTestObject parent1 = transaction1.Select(typeof(GuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as GuidParentTestObject;
			GuidParentTestObject parent2 = transaction1.Select(typeof(GuidParentTestObject), "{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}") as GuidParentTestObject;

			GuidChildTestObject child1 = transaction1.Select(typeof(GuidChildTestObject), "{0F455A52-C339-4A3C-9DCD-2620A5998295}") as GuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("1", child1.ObjData);
			Assert.AreEqual(new Guid("{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent1, child1.Parent);
			Assert.IsTrue(parent1.ChildObjects.Contains(child1));
			Assert.IsFalse(parent2.ChildObjects.Contains(child1));

			child1.Parent = parent2;
			
			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child1.Id);	
			Assert.AreEqual("1", child1.ObjData);
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child1.Parent.Id);	
			Assert.IsTrue(child1.Parent.ChildObjects.Contains(child1));
			Assert.AreEqual(parent2, child1.Parent);
			Assert.IsTrue(parent2.ChildObjects.Contains(child1));
			Assert.IsFalse(parent1.ChildObjects.Contains(child1));

			transaction1.Commit();

			Assert.AreEqual(parentCount, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("GuidChildren"));

			ObjectTransaction transaction2 = manager.BeginTransaction();

			GuidChildTestObject child2 = transaction2.Select(typeof(GuidChildTestObject), new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}")) as GuidChildTestObject;

			Assert.AreEqual(new Guid("{0F455A52-C339-4A3C-9DCD-2620A5998295}"), child2.Id);	
			Assert.AreEqual("1", child2.ObjData);
			Assert.AreEqual(new Guid("{F9643D1E-9FAC-4C01-B535-D2B0D2F582E7}"), child2.Parent.Id);

			Assert.AreEqual(parentCount, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount, DataUtil.CountRows("GuidChildren"));
		}

		[Test]
		public void InsertParentChild()
		{
			int parentCount = DataUtil.CountRows("GuidParents");
			int childrenCount = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidParentTestObject parent = transaction.Create(typeof(GuidParentTestObject)) as GuidParentTestObject;
			parent.ObjData = "XXX";
			Assert.AreEqual(0, parent.ChildObjects.Count);

			for(int i = 0; i < 10; i++)
			{
				GuidChildTestObject child = transaction.Create(typeof(GuidChildTestObject)) as GuidChildTestObject;
				child.ObjData = i.ToString();
				child.Parent = parent;
			}

			Assert.AreEqual(10, parent.ChildObjects.Count);

			transaction.Commit();

			Assert.AreEqual(parentCount + 1, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount + 10, DataUtil.CountRows("GuidChildren"));
		}

		[Test]
		[ExpectedException(typeof(ObjectServerException), "Could not delete, UnitTests.TestObjects.GuidChildTestObject.Parent has DeleteAction.Throw")]
		public void DeleteParentWithoutCascade()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			GuidParentTestObject parent = transaction.Select(typeof(GuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as GuidParentTestObject;

			transaction.Delete(parent);
		}

		[Test]
		public void DeleteParent()
		{
			int parentCount = DataUtil.CountRows("GuidParents");
			int childrenCount = DataUtil.CountRows("GuidChildren");

			ObjectTransaction transaction = manager.BeginTransaction();

			GuidParentTestObject parent = transaction.Select(typeof(GuidParentTestObject), "{C9ECFD97-F8BF-4075-A654-C62F6BD750D9}") as GuidParentTestObject;

			foreach(GuidChildTestObject child in parent.ChildObjects)
			{
				transaction.Delete(child);
			}

			transaction.Delete(parent);

			transaction.Commit();

			Assert.AreEqual(parentCount - 1, DataUtil.CountRows("GuidParents"));
			Assert.AreEqual(childrenCount - 2, DataUtil.CountRows("GuidChildren"));
		}
	}
}
