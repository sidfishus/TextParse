using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Parse.TextPatternParser
{
	public interface IComparisonWithAdvance : IComparison
	{
		bool CompareAndAdvance(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index);
	}
}
