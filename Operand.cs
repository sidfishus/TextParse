using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	public delegate T fOperand<T>(int pos, string str, RunState runState);

	public class Operand
	{
		public static fOperand<T> StaticValue<T>(T value)
		{
			return (int pos, string str, RunState runState) => value;
		}

		public static fOperand<T> Variable<T>(string varName)
		{
			return (int pos, string str, RunState runState) => (T)runState.GetVariable(varName);
		}

		public static fOperand<int> InputStringLength()
		{
			return (int pos, string str, RunState runState) => str.Length;
		}

		public static fOperand<int> CurrentPosition()
		{
			return (int pos, string str, RunState runState) => pos;
		}

		public static fOperand<T> CallFunction<T>(string funcName)
		{
			return (int pos, string str, RunState runState) => (T)runState.CallFunction<T>(funcName, pos, str);
		}
	}
}