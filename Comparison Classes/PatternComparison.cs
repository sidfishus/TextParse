using System;
using System.Collections.Generic;
using System.Text;
using tComparisonRange =
	Sid.Parse.TextPatternParser.ComparisonRange<Sid.Parse.TextPatternParser.IComparisonWithAdvance>;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class PatternComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		List<tComparisonRange> m_ComparisonRange =new List<tComparisonRange>();
		bool m_SortedRanges;
		//~V

		public PatternComparison(
		 ILog log,
		 Options options) : base(log,options)
		{
			m_SortedRanges = false;
		}

		public PatternComparison(
		 ILog log) : this(log,null)
		{
		}

		public int? MinLength
		{
			get;
			set;
		}

		public int? MaxLength
		{
			get;
			set;
		}

		// If this is set and returns true, stop the match. For example can be used for word comparison: find the
		//  terminating whitespace
		//TODO perhaps this could use IComparisonWithAdvance/ComparisonNoAdvance instead
		public IComparison EndComparison
		{
			get;
			set;
		}

		public void AddComparisonRange(tComparisonRange range)
		{
			//TODO check it doesn't clash with an existing range?
			m_ComparisonRange.Add(range);
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			if(EndComparison==null && !MaxLength.HasValue)
			{
				Parser.ThrowParseError(string.Format("Invalid combination of parameters. "+
				 "Both the end comparison and max length values have not been specified. "+
				 "How can we tell when the pattern match has completed?"));
			}

			index = -1;
			// If the minimum value is less than the length of the input string then it can never match
			int numCharsLeftInStr = str.Length - firstIndex;
			if(MinLength.HasValue && MinLength.Value> numCharsLeftInStr)
			{
				return false;
			}

			// Sort the ranges
			if (!m_SortedRanges)
			{
				// A speedup by only doing it once
				m_SortedRanges = true;
				m_ComparisonRange.Sort(tComparisonRange.CompareByRange);
			}

			//// Iterate the string
			// The relevant range
			tComparisonRange range = null;
			// The index of the relevant range
			int rangeIndex = 0;
			// The index into the input string
			int i;

			int depthPlusOne = depth + 1;

			bool matched = true;
			for (i= firstIndex; matched && i<str.Length;)
			{
				// The current length
				int thisIndex1Based = i - firstIndex + 1;

				// Check the maximum length
				if (EndComparison!=null)
				{
					// If the end comparison has been specified but we've exceeded the maximum length then this is not a match
					if (MaxLength.HasValue && thisIndex1Based > MaxLength.Value)
					{
						matched = false;
					}
				}
				else
				{
					// If there is no end comparison and we've reached the maximum length then this is a successful match
					//  and stop trying to match any further
					if (thisIndex1Based > MaxLength.Value)
					{
						break;
					}
				}

				if(matched)
				{
					// Check the current range is still relevant
					if (range != null)
					{
						if (!range.Range.IsInRange(thisIndex1Based))
						{
							rangeIndex += 1;
							range = null;
						}
					}

					// Find/update the appropriate range
					if (range == null)
					{
						for (int iRange = rangeIndex; iRange < m_ComparisonRange.Count; ++iRange)
						{
							// Is it in range?
							tComparisonRange iterRange = m_ComparisonRange[iRange];
							if (iterRange.Range.IsInRange(thisIndex1Based))
							{
								// Update the range in use
								range = iterRange;
								rangeIndex = iRange;
								break;
							}
						}

						if (range == null)
						{
							string text = string.Format("No range specified for character at index {0}.", i);
							Parser.ThrowParseError(text);
						}
					}

					// Do the comparison
					// If the comparison is not specified then simply advance by 1 if we have not reached the end.
					// Can be used to specify match on everything
					bool endMatches;
					if (EndComparison!=null)
					{
						endMatches = EndComparison.Compare(str, i, depthPlusOne, runState);
					}
					else
					{
						endMatches = false;
					}
					if (range.Comparison != null)
					{
						int afterMatch;
						matched = range.Comparison.CompareAndAdvance(str, i, depthPlusOne, runState, out afterMatch);
						// This is a key nuance with the way this works. DO NOT STOP if both the end comparisons and range
						//  comparisons succeed. This is a design decision to make it behave in this manner
						if (endMatches && !matched)
						{
							matched = true;
							break;
						}

						// Advance to after the successful match
						i = afterMatch;
					}
					else
					{
						if(endMatches)
						{
							// Stop if we found the end
							break;
						}
						++i;
					}
				}
			}

			if (matched)
			{
				index = i;
				// Check the length
				int length = i - firstIndex;
				bool isMinimumLength= (!MinLength.HasValue || (MinLength.HasValue && length >= MinLength.Value));
				matched=(isMinimumLength);
			}
			return matched;
		}

		public override string ToString()
		{
			return "PatternComparison";
		}
	}
}
