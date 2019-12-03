using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

//TODO: should this be made more generalised rather than always checking the first character.
// What about an offset parameter?
namespace Sid.Parse.TextPatternParser
{
	public class StartOfInputStringComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		public StartOfInputStringComparison(
		 ILog log) : base(log)
		{
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			//sidtodo: shouldn't the index be incremented? and then this be used with CompareNoAdvance?
			index = pos;
			return (pos == 0);
		}

		public override string ToString()
		{
			return "StartOfStringComparison";
		}
	}
}
