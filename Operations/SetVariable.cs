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

		fOperand<int> m_Operand;

		public SetVariable(
		 ILog log,
		 string variableName,
		 fOperand<int> operand) : base(log)
		{
			m_VariableName = variableName;
			m_Operand = operand;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			runState.SetVariable(m_VariableName, m_Operand(pos,input,runState));
			index = pos;
		}

		public override string ToString()
		{
			return "SetVariable";
		}
	}
}
