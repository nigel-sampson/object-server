using System;

namespace UnitTests.Chelsea
{
	internal sealed class Constants
	{
		private Constants()
		{
			
		}

		public const string UrlValidationExpression = @"http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
		public const string EmailValidationExpression =  @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
	}
}
