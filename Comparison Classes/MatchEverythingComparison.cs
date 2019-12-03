using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class MatchEverythingComparison : ComparisonWithAdvanceBase, ICharComparison
	{
		public MatchEverythingComparison(
		 ILog log) : base(null)
		{
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = pos + 1;
			return true;
		}

		public override string ToString()
		{
			return "MatchEverythingComparison";
		}
	}
}
