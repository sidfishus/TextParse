using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class AdvanceIfOperation : OperationBase, IOperation
	{
		IComparisonWithAdvance m_Comparison;
		//~V

		public AdvanceIfOperation(
		 ILog log,
		 IComparisonWithAdvance comp) : base(log)
		{
			m_Comparison = comp;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			if (!m_Comparison.CompareAndAdvance(input, pos, depth+1, runState, out index))
			{
				// Reset the position back to the beginning if the comparison is false
				index = pos;
			}
		}

		public override string ToString()
		{
			return "AdvanceIfOperation";
		}
	}
}
