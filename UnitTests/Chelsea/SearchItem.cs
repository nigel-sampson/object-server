using System;
using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	public class SearchItem : IComparable
	{
		private int relevance;
		private ServerObject item;

		public SearchItem(Recipe recipe, string terms)
		{
			relevance = 0;
			item = recipe;

			foreach(string term in terms.Split(' '))
			{
				relevance += (50 * Occurs(recipe.Name, term));
				relevance += (10 * Occurs(recipe.Ingredients, term));
				relevance += (5 * Occurs(recipe.Method, term));
			}

			if(recipe.Author == null)
				relevance += 200;
		}

		public SearchItem(PageContent content, string terms)
		{
			relevance = 0;
			item = content;

			foreach(string term in terms.Split(' '))
			{
				relevance += (200 * Occurs(content.Title, term));
				relevance += (20 * Occurs(content.Text, term));
			}
		}

		public SearchItem(EnvironmentalItem content, string terms)
		{
			relevance = 0;
			item = content;

			foreach(string term in terms.Split(' '))
			{
				relevance += (200 * Occurs(content.Title, term));
				relevance += (20 * Occurs(content.Text, term));
			}
		}

		public SearchItem(Tip tip, string terms)
		{
			relevance = 0;
			item = tip;

			foreach(string term in terms.Split(' '))
			{
				relevance += (200 * Occurs(tip.Title, term));
				relevance += (20 * Occurs(tip.Body, term));
			}
		}

		public SearchItem(GlossaryItem gItem, string terms)
		{
			relevance = 0;
			item = gItem;

			foreach(string term in terms.Split(' '))
			{
				relevance += (200 * Occurs(gItem.Title, term));
				relevance += (20 * Occurs(gItem.Description, term));
			}
		}

		private static int Occurs(string text, string term)
		{
			string lcText = text.ToLower();
			string lcTerm = term.ToLower();

			int count = 0;

			for(int i = 0; i < lcText.Length; i++)
			{
				int found = lcText.IndexOf(lcTerm, i);

				if(found >= 0)
				{
					count++;
					i = found;
				}
				else
					break;
			}

			return count;
		}

		public int Relevance
		{
			get
			{
				return relevance;
			}
		}

		public string Url
		{
			get
			{
				if(item is Recipe)
					return  String.Format("ViewRecipe.aspx?id={0}", ((Recipe)item).Id);
				else if(item is Tip)
					return  String.Format("ViewTip.aspx?id={0}", ((Tip)item).Id);
				else if(item is GlossaryItem)
					return  String.Format("Glossary.aspx?#{0}", ((GlossaryItem)item).Id);
				else if(item is EnvironmentalItem)
					return  String.Format("EnvItems.aspx?#{0}", ((EnvironmentalItem)item).Id);
				else
					return ((PageContent)item).Url;
			}
		}

		public string Image
		{
			get
			{
				if(item is Recipe)
				{
					Recipe recipe = item as Recipe;
					if(recipe.Author == null)
						return  String.Format("RecipeThumbnail.ashx?id={0}", ((Recipe)item).Id);
					else
						return "images/membersubmitted.jpg";
				}
				else if(item is Tip)
					return "images/membersubmitted.jpg";
				else if(item is GlossaryItem)
					return "images/membersubmitted.jpg";
				else if(item is EnvironmentalItem)
					return "images/noimage.jpg";
				else
					return "images/noimage.jpg";
			}
		}

		public string Style
		{
			get
			{
				return "smallpic";
			}
		}

		public string Name
		{
			get
			{
				if(item is Recipe)
					return ((Recipe)item).Name;
				else if(item is Tip)
					return "Baking Tip: " + ((Tip)item).Title;
				else if(item is GlossaryItem)
					return "Glossary: " + ((GlossaryItem)item).Title;
				else if(item is EnvironmentalItem)
					return ((EnvironmentalItem)item).Title;
				else
					return ((PageContent)item).Title;
			}
		}

		public ServerObject Item
		{
			get
			{
				return item;
			}
		}

		public int CompareTo(object obj)
		{
			if(!(obj is SearchItem))
				throw new ApplicationException("object is not a SearchItem");

			SearchItem item = obj as SearchItem;

			return item.Relevance.CompareTo(Relevance);
		}
	}
}
