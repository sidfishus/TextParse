using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class NestedOpenCloseComparison : ComparisonWithAdvanceBase,IComparisonWithAdvance
	{
		IComparisonWithAdvance m_Open;
		IComparisonWithAdvance m_Close;
		//~V

		public NestedOpenCloseComparison(
		 ILog log,
		 IComparisonWithAdvance open,
		 IComparisonWithAdvance close,
		 string name=null) : base(log)
		{
			m_Open = open;
			m_Close = close;
			Name = name;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			int depthPlusOne = depth + 1;
			// If the start doesn't match the open then this is not a match
			if (m_Open.CompareAndAdvance(input, pos, depthPlusOne, runState, out pos))
			{
				// How far down the rabbit hole are we?
				int nestedDepth = 1;
				for(; ; )
				{
					int currentPos = pos;
					if (m_Open.CompareAndAdvance(input, currentPos, depthPlusOne, runState, out pos))
					{
						++nestedDepth;
					}
					else if (m_Close.CompareAndAdvance(input, currentPos, depthPlusOne, runState, out pos))
					{
						--nestedDepth;
						if(nestedDepth == 0)
						{
							// Found the end
							index = pos;
							return true;
						}
					}
					else
					{
						// Advance by 1
						pos = currentPos + 1;
					}
				}
			}

			index = -1;
			return false;
		}

		public override string ToString()
		{
			return "NestedOpenCloseComparison";
		}
	}
}
