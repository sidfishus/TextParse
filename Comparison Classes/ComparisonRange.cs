using System;
using System.Collections.Generic;
using System.Text;
using Sid;

namespace Sid.Parse.TextPatternParser
{
	public class ComparisonRange<COMP_TYPE>
		where COMP_TYPE : IComparison
	{
		public ComparisonRange()
		{
			Range = new Range<int>();
		}

		public COMP_TYPE Comparison
		{
			get;
			set;
		}

		public Range<int> Range
		{
			get;
			set;
		}

		public static int CompareByRange(
		 ComparisonRange<COMP_TYPE> lhs,
		 ComparisonRange<COMP_TYPE> rhs)
		{
			IComparable comp = lhs.Range;
			return comp.CompareTo(rhs.Range);
		}
	}
}
