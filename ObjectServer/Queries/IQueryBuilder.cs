using System;
using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer.Queries
{
	internal interface IQueryBuilder
	{
		void AddParentJoin(ParentSchema joinSchema);

		IDataContext Context
		{
			get;
		}

		Type QueriedType
		{
			get;
		}

		ParameterCollection Parameters
		{
			get;
		}
	}
}
