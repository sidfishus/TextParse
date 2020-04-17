using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	// Originally this was an operation but that caused issues. It has to be a comparison, in case the condition never
	// matches in which case the match should fail overall
	public class AdvanceUntilComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		IComparisonWithAdvance m_Comparison;
		// Can be used to trigger this comparison to stop searching. For example when searching backwards
		IComparisonWithAdvance m_ContinueComp;
		bool m_Forwards;
		//~V

		public AdvanceUntilComparison(
			ILog log,
			IComparisonWithAdvance comp,
			bool forwards=true,
			IComparisonWithAdvance continueComp = null) : base(log)
		{
			m_Forwards = forwards;
			m_Comparison = comp;
			m_ContinueComp = continueComp;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = -1;
			// Basically just loop until the comparison returns true
			bool end = false;
			int depthPlusOne = depth + 1;
			for (index = pos; ;)
			{
				// Continue??
				if (m_ContinueComp != null)
				{
					if (!m_ContinueComp.Compare(input, index, depthPlusOne, runState))
					{
						// No.
						return false;
					}
				}

				int previousPos = index;
				end = m_Comparison.CompareAndAdvance(input, index, depthPlusOne, runState, out index);
				if (end)
				{
					return true;
				}

				// If the comparison fails and we are at the end of the string (last char + 1) then cease to continue
				if(previousPos == input.Length)
				{
					return false;
				}
				
				// Advance by 1 and try again
				index = previousPos + ((m_Forwards)?1: -1);
			}
		}

		public override string ToString()
		{
			return "AdvanceUntilComparison";
		}
	}
}
