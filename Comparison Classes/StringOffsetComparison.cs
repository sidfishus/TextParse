using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class StringOffsetComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		fOperand<int> m_Length;
		fOperand<int> m_Offset;
		bool m_Reverse;

		public StringOffsetComparison(
			ILog log,
			Options options,
			fOperand<int> length,
			fOperand<int> offset,
			bool reverse, /* Do a reverse string match */
			string name = null,
			UserOnMatch fUserOnMatch = null) : base(log, options, fUserOnMatch)
		{
			Name = name;
			m_Length = length;
			m_Offset = offset;
			m_Reverse = reverse;
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{

			var length = m_Length(firstIndex, str, runState);
			if (length <= 0)
			{
				Parser.ThrowParseError($"The length used in the StringOffsetComparison with name {Name} is less or equal to 0 ({length}).");
			}

			// The length exceeds what's left of the parse input string.
			if ((firstIndex + length) > str.Length)
			{
				index = -1;
				return false;
			}

			int offsetIndex = m_Offset(firstIndex, str, runState);

			if (offsetIndex < 0)
			{
				Parser.ThrowParseError($"The offset index used in the StringOffsetComparison with name {Name} is less " +
					$"than zero ({offsetIndex}) which is an invalid array index.");
			}

			if (length > (str.Length - offsetIndex))
			{
				Parser.ThrowParseError($"The offset string denoted by the StringOffsetComparison with name {Name} is " +
					$"longer ({length}) than the length of the string remaining at the offset ({str.Length - offsetIndex})");
			}

			//// Do the comparison

			// The function used to compare characters which changes according to the options
			Func<int, bool> fCharCompare;
			if (m_Options.CaseSensitive)
			{
				if (m_Reverse)
				{
					fCharCompare = i => (str[i + firstIndex] == str[offsetIndex + (length - i - 1)]);
				}
				else
				{
					fCharCompare = i => (str[i + firstIndex] == str[offsetIndex + i]);
				}
			}
			else
			{
				if (m_Reverse)
				{
					fCharCompare = i => Char.ToUpper(str[i + firstIndex]) == Char.ToUpper(str[offsetIndex + (length - i - 1)]);
				}
				else
				{
					fCharCompare = i => Char.ToUpper(str[i + firstIndex]) == Char.ToUpper(str[offsetIndex + i]);
				}
			}

			for (int i = 0; i < length; ++i)
			{
				if (!fCharCompare(i))
				{
					index = -1;
					return false;
				}
			}

			index = firstIndex + length;
			return true;
		}
	}
}