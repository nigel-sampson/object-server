using System;

namespace UnitTests.Chelsea
{
	internal sealed class Utility
	{
		private Utility()
		{
			
		}

		public static string LimitString(int limit, string text)
		{
			if(limit >= text.Length)
				return text;

			int space = text.IndexOf(" ", limit);

			if(space >= text.Length || space == -1)
				return text;

			return text.Substring(0, space);
		}
	}
}
