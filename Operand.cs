using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	delegate T fOperand<T>(int pos, string str, RunState runState);

	class Operand
	{
		public static fOperand<T> StaticValue<T>(T value)
		{
			return (int pos, string str, RunState runState) => value;
		}

		public static fOperand<T> Variable<T>(string varName)
		{
			return (int pos, string str, RunState runState) => (T)runState.GetVariable(varName);
		}
	}
}