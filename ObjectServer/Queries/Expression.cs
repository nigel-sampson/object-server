using System;

namespace Nichevo.ObjectServer.Queries
{
	public enum Expression
	{
		GreaterThan,
		GreaterThanOrEqualTo,
		LessThan,
		LessThanOrEqualTo,
		Like,
		NotLike,
		Equal,
		NotEqual,
		IsNull,
		IsNotNull,
		Between,
		NotBetween,
		In,
		NotIn
	}
}
