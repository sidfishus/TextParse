using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class CharComparison : ComparisonWithAdvanceBase, ICharComparison
	{
		char m_Char;
		public CharComparison(
		 ILog log,
		 Options options,
		 char chr,
		 UserOnMatch fUserOnMatch = null) : base(log, options, fUserOnMatch)
		{
			m_Char = chr;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			bool rv;
			char chr = input[pos];
			if (m_Options.CaseSensitive)
			{
				rv = Char.ToUpper(m_Char) == Char.ToUpper(chr);
			}
			else
			{
				rv = (m_Char == chr);
			}
			index = pos + 1;
			return rv;
		}

		public override string ToString()
		{
			return string.Format("CharComparison {0}",m_Char);
		}
	}
}
