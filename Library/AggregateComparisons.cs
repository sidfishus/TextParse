
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	// Comparisons that are an aggregation of other comparisons
	public static class AggregateComparisons
	{
		//TODO: this doesn't work!
		//TODO: this only understands '*' at the moment. '?' would be useful
		public static IComparisonWithAdvance WildcardComparison(
			string wildcardStr,
			ILog log,
			Options options)
		{
			// The list of parser statements
			StatementList mainStatements = new StatementList(log);

			// Length of the input string
			var len= wildcardStr.Length;

			// The current fixed string that we are building up
			var currentString = new StringBuilder();

			// Add the current string as a string comparison
			Action ConditionalAddCurrentString = () =>
			{
				if (currentString.Length > 0)
				{
					mainStatements.Add(new StringComparison(log, options, currentString.ToString(), null));
					currentString.Clear();
				}
			};

			for (int i = 0; i < len; ++i) {

				var chr= wildcardStr[i];

				// * means match all
				if (chr == '*')
				{

					// Escape an asterix?
					if (++i < len)
					{
						var nextChar = wildcardStr[i];
						if (nextChar == '*')
						{
							currentString.Append('*');
						}

						else
						{
							// Match the string so far
							ConditionalAddCurrentString();
							// Skip till we find the next character in the sequence (nextChr)
							mainStatements.Add(new AdvanceUntilComparison(log, new CharComparison(log, options, nextChar)));
						}
					}
					else
					{
						// We've reached the end of the wild character string and the last character is an asterix
						// All we can do in this case is advance until the end of the string
						// You wouldn't really use this if you had additional comparisons after this, because it wouldn't
						// work as expected (it wouldn't match because after this you would be at the end of the string)
						//sidtodo not tested
						mainStatements.Add(new AdvanceToTheEnd(log));
					}
				}

				else
				{
					currentString.Append(chr);
				}
			}

			ConditionalAddCurrentString();

			return mainStatements;
		}

		// Get a value between two comparisons
		public static IComparisonWithAdvance GetValuesBetweenComp(
			ILog log,
			IComparisonWithAdvance begin,
			IComparisonWithAdvance after)
		{
			StatementList mainComp = new StatementList(null);

			// Find the beginning
			mainComp.Add(begin);
			// Remember the position
			mainComp.Add(new StorePosAsVariable(log, "ValueBegin"));

			// Find the end and store the preceeding position
			mainComp.Add(new AdvanceUntilComparison(log, new CompareNoAdvance(log, after)));
			mainComp.Add(new StorePosAsVariable(log, "ValueEnd"));

			return mainComp;
		}
	}

	// This is the newer version but it doesn't work
	////TODO: this doesn't work! DON'T USE
	////TODO: this only understands '*' at the moment. '?' would be useful
	//public static IComparisonWithAdvance WildcardComparison(
	//	string wildcardStr,
	//	ILog log,
	//	Options options,
	//	string name = null)
	//{
	//	// The list of parser statements
	//	StatementList mainStatements = new StatementList(log);
	//	mainStatements.Name = name;

	//	// Length of the input string
	//	var len = wildcardStr.Length;

	//	// Index of the start of the current string. Referenced outside of the loop due to AddString using it
	//	int startOfString = 0;

	//	// Are we within a match all expression?
	//	bool withinMatchAll = false;

	//	Action<int?> AddMatch = (int? endPos) =>
	//	{
	//		string str = ((endPos.HasValue) ? wildcardStr.Substring(startOfString, endPos.Value - startOfString) : wildcardStr.Substring(startOfString));
	//		if (withinMatchAll)
	//		{
	//			// Loop until we match this string
	//			//TODO: this doesn't work
	//			mainStatements.Add(new AdvanceUntilComparison(log, new StringComparison(log, options, str)));
	//		}

	//		else
	//		{
	//			// Match this string
	//			mainStatements.Add(new StringComparison(log, options, str, null));
	//		}
	//	};

	//	for (; ; )
	//	{
	//		if (startOfString >= len)
	//		{
	//			// Done
	//			AddMatch(null);
	//			break;
	//		}

	//		char[] specialChars = { '?', '*' };

	//		int specialPos = wildcardStr.IndexOfAny(specialChars, startOfString);

	//		if (specialPos == -1)
	//		{
	//			// Done
	//			AddMatch(null);
	//			break;
	//		}

	//		char chr = wildcardStr[specialPos];

	//		switch (chr)
	//		{
	//			case '?':
	//				// INCOMP
	//				// Add match any single character
	//				break;

	//			case '*':
	//				{
	//					int specialPosPlusOne = specialPos + 1;
	//					if (specialPosPlusOne >= len)
	//					{
	//						// We've reached the end of the wild character string and the last character is an asterix
	//						// All we can do in this case is advance until the end of the string
	//						// You wouldn't really use this if you had additional comparisons after this, because it wouldn't
	//						// work as expected (it wouldn't match because after this you would be at the end of the string)

	//						//sidtodo not tested
	//						mainStatements.Add(new AdvanceToTheEnd(log));
	//					}

	//					if (wildcardStr[specialPosPlusOne] == '*')
	//					{
	//						// Escaped asterix: add the current string and continue searching after the second asterix
	//						//_ASSERTE(!withinMatchAll);
	//						AddMatch(specialPosPlusOne);
	//						startOfString = specialPosPlusOne + 1;
	//					}
	//					else
	//					{
	//						AddMatch(specialPos);

	//						withinMatchAll = !withinMatchAll;

	//						startOfString = specialPosPlusOne;
	//					}

	//				}
	//				break;
	//		}

	//	}

	//	return mainStatements;
	//}
}
