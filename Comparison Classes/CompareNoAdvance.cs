using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

// Compare but don't advance - useful for finding the end of statements without advancing past
namespace Sid.Parse.TextPatternParser
{
	public class CompareNoAdvance : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		IComparison m_Comparison;
		//~V

		public CompareNoAdvance(
		 ILog log,
		 IComparison comp,
		 string name=null) : base(log)
		{
			m_Comparison = comp;
			Name = name;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
#if DEBUG
			if (pos == 1720)
			{
			}
#endif
			index = pos;
			bool rv= m_Comparison.Compare(input, pos, depth, runState);
			if (rv)
			{
			}
			return rv;
		}
	}
}
