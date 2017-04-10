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
	public class SimpleQueries : ServicedComponent
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
		public void SelectCakes()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Category", Expression.Equal, 1));

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Recipe recipe = recipes[0] as Recipe;
			Assert.AreEqual(454, recipe.Id);

			Assert.AreEqual(84, recipes.Count);
		}

		[Test]
		public void SelectCakesWithOrder()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Category", Expression.Equal, 1));

			query.Order = "Id";

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Recipe recipe = recipes[0] as Recipe;
			Assert.AreEqual(21, recipe.Id);

			Assert.AreEqual(84, recipes.Count);
		}

		[Test]
		public void SelectChelseaRecipes()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Author", Expression.IsNull));

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Assert.AreEqual(218, recipes.Count);
		}

		[Test]
		public void SelectLimit()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query(new Condition("Author", Expression.IsNull));
			query.Top = 5;

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			Assert.AreEqual(5, recipes.Count);
		}

		[Test]
		public void ParentOrder()
		{
			ObjectTransaction transaction = manager.BeginTransaction();

			Query query = new Query();

			query.Order = "Author.firstname";

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);
		}

	}
}
