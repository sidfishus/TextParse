using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class NotComparison : ComparisonBase, IComparison
	{
		IComparison m_Comparison;
		//~V

		public NotComparison(
		 ILog log,
		 IComparison comparison) : base(log)
		{
			m_Comparison = comparison;
		}

		protected override bool CompareImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState)
		{
			bool rv = !m_Comparison.Compare(input, pos, depth+1, runState);
			return rv;
		}

		public override string ToString()
		{
			return "NotComparison";
		}
	}

	public class NotCharComparison : ComparisonWithAdvanceBase, ICharComparison
	{
		ICharComparison m_CharComparison;
		//~V

		public NotCharComparison(
		 ILog log,
		 ICharComparison charComparison) : base(log)
		{
			m_CharComparison = charComparison;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			return !m_CharComparison.CompareAndAdvance(input, pos, depth+1, runState, out index);
		}

		public override string ToString()
		{
			return "NotCharComparison";
		}
	}
}