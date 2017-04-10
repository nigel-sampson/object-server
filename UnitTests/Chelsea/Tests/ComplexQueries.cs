using System;
using NUnit.Framework;
using Nichevo.ObjectServer;
using System.EnterpriseServices;
using UnitTests.TestObjects;

using Nichevo.ObjectServer.Queries;

namespace UnitTests.Chelsea.Tests
{
	[TestFixture]
	[Transaction(TransactionOption.RequiresNew)]
	public class ComplexQueries : ServicedComponent
	{
		private ObjectManager manager;

		[SetUp]
		public void Setup()
		{
			manager = new ObjectManager(ServerType.SqlServer, UnitTests.Constants.ChelseaConnectionString);
		}

		[TearDown]
		public void TearDown()
		{
			if(ContextUtil.IsInTransaction)
				ContextUtil.SetAbort();
		}

		[Test]
		public void SelectValentines()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Occasion valentines = transaction.Select(typeof(Occasion), 11) as Occasion;

			Query query = new Query(new SetCondition("OccasionMaps", new Condition("Occasion", Expression.Equal, valentines.Id)));

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Assert.AreEqual(23, recipes.Count);
		}
		
		[Test]
		public void SelectRecipesWithNoOccasions()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new NotCondition(new SetCondition("OccasionMaps")));

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Assert.AreEqual(49, recipes.Count);
		}
	}
}
