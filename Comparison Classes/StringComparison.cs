using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class StringComparison : ComparisonWithAdvanceBase,IComparisonWithAdvance
	{
		// String to compare against
		string m_String;

		public StringComparison(
		 ILog log,
		 Options options,
		 string str,
		 string name=null,
		 UserOnMatch fUserOnMatch=null) : base(log,options, fUserOnMatch)
		{
			m_String = str;
			Name = name;
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			int lengthOfArgString = str.Length - firstIndex;
			if (lengthOfArgString<m_String.Length)
			{
				// m_String is longer than the argument/comparison string
				index = -1;
				return false;
			}

			if (m_Options.CaseSensitive)
			{
				for (int i = 0; i < m_String.Length; ++i)
				{
					if(str[i+firstIndex]!=m_String[i])
					{
						index = -1;
						return false;
					}
				}
			}
			else
			{
				for (int i = 0; i < m_String.Length; ++i)
				{
					if (Char.ToUpper(str[i + firstIndex]) != Char.ToUpper(m_String[i]))
					{
						index = -1;
						return false;
					}
				}
			}

			index = firstIndex + m_String.Length;
			return true;
		}

		public override string ToString()
		{
			int stringLength = Math.Min(20, m_String.Length);
			return string.Format("StringComparison {0}", m_String.Substring(0, stringLength));
		}
	}
}
