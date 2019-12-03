using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid;
using Sid.Log;

// Compare a string but if we encounter whitespace in the input string, skip whitespace in the compare string.
// I.e. these two strings are considered equal for a 'StringComparisonSkipWS' comparison:
// 1. <table x>
// 2. <table
//
//
// x>

namespace Sid.Parse.TextPatternParser
{
	public class StringComparisonSkipWS : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		// String to compare against
		string m_String;

		public StringComparisonSkipWS(
		 ILog log,
		 Options options,
		 string str) : base(log, options)
		{
			m_String = str;
		}

		//sidtodo this is wrong. rather than a comparison, why not parse the string and add a 'skip whitespace'
		//comparison for each word in the input string, and then return that as a parser
		//TODO
		//the parser itself could inherit from 'IComparison' and IComparisonWithAdvance.
		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			// Check the length of the compare string first
			if (firstIndex >= str.Length)
			{
				index = -1;
				return false;
			}

			Str.CharComparisonDel cmp;
			if (m_Options.CaseSensitive)
			{
				cmp = Str.CaseSensitiveComp;
			}
			else
			{
				cmp = Str.CaseInsensitiveComp;
			}

			// Iterate both strings
			for (int memberI = 0, compI = firstIndex; /* Test is inside the loop */; /* Increment is inside the loop */)
			{
				// Member character is whitespace?
				if (Char.IsWhiteSpace(m_String[memberI]))
				{
					// Compare character is whitespace?
					if (!Char.IsWhiteSpace(str[compI]))
					{
						// Comparing whitespace in the member string against non-whitespace in the comparison string
						// = no match
						index = -1;
						return false;
					}

					// Skip both strings until find non-whitespace or the end
					++memberI;
					bool memberEndedInWhitespace = !Str.SkipWhitespace(m_String, ref memberI);
					++compI;
					bool compEndedInWhitespace = !Str.SkipWhitespace(str, ref compI);


					if (memberEndedInWhitespace)
					{
						// Match
						index = compI;
						return true;
					}

					if (compEndedInWhitespace)
					{
						// The compare string is shorter than the member string
						index = -1;
						return false;
					}
				}

				// Compare the characters
				if (!cmp(m_String[memberI], str[compI]))
				{
					// Mismatch in characters
					index = -1;
					return false;
				}

				++memberI;
				++compI;

				// Reached the end of either string?
				bool memberReachedEnd = (memberI == m_String.Length);
				bool compReachedEnd = (compI == str.Length);

				if (memberReachedEnd)
				{
					if (!compReachedEnd)
					{
						// Skip whitespace in the compare string
						Str.SkipWhitespace(str, ref compI);
					}
					index = compI;
					// Match
					return true;
				}

				if (compReachedEnd)
				{
					index = -1;
					return false;
				}

				// Next
			}
		}

		public override string ToString()
		{
			int stringLength = Math.Min(20, m_String.Length);
			return string.Format("StringComparisonWS {0}", m_String.Substring(0, stringLength));
		}
	}
}
