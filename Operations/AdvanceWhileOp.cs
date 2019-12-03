using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class AdvanceWhileOperation : OperationBase, IOperation
	{
		IComparisonWithAdvance m_Comparison;
		//~V

		public AdvanceWhileOperation(
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
			index = -1;
			// Basically just loop until the comparison returns false
			bool end = false;
			int depthPlusOne = depth + 1;
			for (index = pos; index < input.Length && !end;)
			{
				int previousPos = index;
				end = !m_Comparison.CompareAndAdvance(input, index, depthPlusOne, runState, out index);
				if (end)
				{
					index = previousPos;
				}
			}
		}

		public override string ToString()
		{
			return "AdvanceWhileOperation";
		}
	}
}
