using System;
using System.Collections;

namespace Nichevo.ObjectServer.Queries
{
	internal class ParameterCollection : CollectionBase
	{
		internal ParameterCollection() : base()
		{
			
		}

		public Parameter this[int index]
		{
			get
			{
				return List[index] as Parameter;
			}
			set
			{
				List[index] = value;
			}
		}

		public Parameter this[string name]
		{
			get
			{
				for(int i = 0; i < List.Count; i++)
				{
					if(((Parameter)List[i]).Name == name)
						return List[i] as Parameter;
				}

				return null;
			}
			set
			{
				for(int i = 0; i < List.Count; i++)
				{
					if(((Parameter)List[i]).Name == name)
						List[i] = value;
				}
			}
		}

		internal string GenerateName()
		{
			int i = 0;

			do
			{
				i++;
			}while(this["@OSParam" + i] != null);

			return "@OSParam" + i;
		}

		internal int Add(Parameter parameter)
		{
			return List.Add(parameter);
		}

		internal int Add(string name, object parameterValue)
		{
			return List.Add(new Parameter(name, parameterValue));
		}

		public int IndexOf(Parameter parameter)
		{
			return List.IndexOf(parameter);
		}

		internal void Insert(int index, Parameter parameter)
		{
			List.Insert(index, parameter);
		}

		internal void Remove(Parameter parameter)
		{
			List.Remove(parameter);
		}

		public bool Contains(Parameter parameter)
		{
			return List.Contains(parameter);
		}

		public void CopyTo(Parameter[] array, int index)
		{
			List.CopyTo(array, index);
		}
	}
}
