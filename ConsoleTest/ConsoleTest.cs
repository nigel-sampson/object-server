using System;
using UnitTests;
using UnitTests.TestObjects;
using UnitTests.Chelsea;
using Nichevo.ObjectServer;
using Nichevo.ObjectServer.Queries;

namespace ConsoleTest
{
	class ConsoleTest
	{
		[STAThread]
		static void Main(string[] args)
		{
			ObjectManager sql = new ObjectManager(ServerType.SqlServer, @"Integrated Security=SSPI; Persist Security Info=False; Initial Catalog=Chelsea; Data Source=Azrael;");

			ObjectTransaction transaction = sql.BeginTransaction();

			Query query = new Query(
				new Condition("Author.Id", Expression.IsNotNull)
			);

			query.Order = "Author.firstname";

			ServerObjectCollection recipes = transaction.Select(typeof(Recipe), query);

			foreach(Recipe recipe in recipes)
			{
				Console.WriteLine("{0} - {1}", recipe.Author.Firstname, recipe.Name);
			}
		}
	}
}
	