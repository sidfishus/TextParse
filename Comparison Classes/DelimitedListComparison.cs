using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class DelimitedListComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		IComparisonWithAdvance m_Comparison;
		IComparisonWithAdvance m_Seperator;
		//~V

		public DelimitedListComparison(
		 ILog log,
		 Options options,
		 IComparisonWithAdvance comp,
		 IComparisonWithAdvance seperator) : base(log,options)
		{
			m_Comparison = comp;
			m_Seperator = seperator;
		}

		public int? MinAmount
		{
			get;
			set;
		}

		public int? MaxAmount
		{
			get;
			set;
		}

		//TODO does it make sense to have a list of 0 items?
		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = -1;
			// 'i' keeps track of how many iterations (list items) we've had
			int i;
			bool reachedEndOfList = false;
			int depthPlusOne = depth + 1;
			for (i=0;;)
			{
#if DEBUG
				int dbgPosAtStart = pos;
#endif
				// Skip the line wrap if there is one before the list item
				m_Options.SkipLineWrap(input, pos, depthPlusOne, runState, out pos);
				if(pos>=input.Length)
				{
					// Reached the end of the input string
					break;
				}

				// Trim left
				if(ItemTrim!=null)
				{
					ItemTrim.Perform(input, pos, depthPlusOne, runState, out pos);
					if (pos >= input.Length)
					{
						// Reached the end of the input string
						break;
					}
				}

				// Do the comparison
				if (!m_Comparison.CompareAndAdvance(input, pos, depthPlusOne, runState, out pos))
				{
					// This list item does not match so overall it is a fail
					index = -1;
					return false;
				}

				// We've had another successful item match
				++i;

				// Skip the line wrap if there is one after the list item
				m_Options.SkipLineWrap(input, pos, depthPlusOne, runState, out pos);
				if (pos >= input.Length)
				{
					// Reached the end of the input string
					break;
				}

				// Trim right
				int beforeRightTrim = pos;
				if (ItemTrim != null)
				{
					ItemTrim.Perform(input, pos, depthPlusOne, runState, out pos);
					if (pos >= input.Length)
					{
						// Reached the end of the input string
						break;
					}
				}

				// Did we reach the seperator/delimiter?
				int afterSeperator;
				if(m_Seperator.CompareAndAdvance(
				 input,
				 pos,
				 depthPlusOne,
				 runState,
				 out afterSeperator))
				{
					// Update the index to be after the seperator
					pos = afterSeperator;
					// Get another list item
				}
				else
				{
					// Must have reached the end of the list
					// Keep the index as the first character of the end - key nuance design decision
					reachedEndOfList = true;

					// If we've reached the end of the list, retreat to before the item trim
					pos = beforeRightTrim;
					break;
				}
			}

			// Do we have the minimum number of items?
			bool rv;
			if (MinAmount.HasValue && i < MinAmount.Value)
			{
				rv = false;
			}
			// Do we have too many items?
			else if (MaxAmount.HasValue && i > MaxAmount.Value)
			{
				rv = false;
			}
			else
			{
				rv = reachedEndOfList;
			}

			index = pos;
			return rv;
		}

		public IOperation ItemTrim
		{
			get;
			set;
		}

		public override string ToString()
		{
			return "ListComparison";
		}
	}
}
