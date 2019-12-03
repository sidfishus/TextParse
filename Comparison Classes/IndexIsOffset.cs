using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class IndexIsOffsetComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		Func<int, string, int, RunState, bool> m_fOffset;

		public IndexIsOffsetComparison(
			ILog log,
			Func<int /* Current pos */, string /* Str */, int /* Depth */, RunState, bool /* Compares true? */> fOffset) : base(log)
		{
			m_fOffset = fOffset;
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{

			if (m_fOffset(firstIndex, str, depth, runState))
			{
				index = firstIndex;
				return true;
			}

			index = -1;
			return false;

		}
		
	}
}
