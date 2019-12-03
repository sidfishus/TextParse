using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class SetVariable : OperationBase, IOperation
	{
		private string m_VariableName;
		private Func<string, int, RunState, object> m_Func;

		public SetVariable(
		 ILog log,
		 string variableName,
		 Func<string,int, RunState, object> func) : base(log)
		{
			m_VariableName = variableName;
			m_Func = func;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			runState.SetVariable(m_VariableName, m_Func(input,pos,runState));
			index = pos;
		}

		public override string ToString()
		{
			return "SetVariable";
		}
	}
}
