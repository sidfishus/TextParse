using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	// This class is to hold any state related information such as variables / functions
	public class RunState
	{
		private Dictionary<string, object> Variables = new Dictionary<string, object>();
		private Dictionary<string, fOperand<object>> Functions=new Dictionary<string,fOperand<object>>();

		public int BeginPos
		{
			get;
			set;
		}

		public void Clear()
		{
			Variables.Clear();
			BeginPos = -1;
			// Don't clear the functions!
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

		// Functions arn't intended to be mutable. The recommended useage is that they are initialised prior to parsing
		// by using the InitRunState callback.
		public void SetFunction<T>(
			string funcName,
			fOperand<T> func
		) {
			// Have to convert the function to one which returns an object unfortunately
			Functions[funcName] = (int pos, string str, RunState runState) => func(pos, str, runState);
		}

		public T CallFunction<T>(
			string funcName,
			int pos,
			string str)
		{
			fOperand<object> func;
			if (!Functions.TryGetValue(funcName, out func))
			{
				throw new Exception($"Function with name '{funcName}' does not exist.");
			}

			return (T)func(pos, str, this);
		}
	}
}
