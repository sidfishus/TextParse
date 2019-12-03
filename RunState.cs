using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	// This class is to hold any state related information such as variables
	public class RunState
	{
		private Dictionary<string, object> Variables = new Dictionary<string, object>();

		public int BeginPos
		{
			get;
			set;
		}

		public void Clear()
		{
			Variables.Clear();
			BeginPos = -1;
		}

		public void SetVariable(
			string varName,
			object value)
		{
			Variables[varName] = value;
		}

		public object GetVariable(
			string varName)
		{
			object rv;
			if (!Variables.TryGetValue(varName, out rv))
			{
				throw new Exception($"Variable name '{varName}' does not exist.");
			}

			return rv;
		}
	}
}
