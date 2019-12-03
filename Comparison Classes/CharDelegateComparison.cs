using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class CharDelegateComparison : ComparisonWithAdvanceBase, ICharComparison
	{
		CharDelegate m_Delegate;
		//~V

		public CharDelegateComparison(ILog log, CharDelegate charDelegate) : base(log)
		{
			m_Delegate = charDelegate;
		}

		public delegate bool CharDelegate(char c);

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			char chr = str[firstIndex];
			bool rv = m_Delegate(chr);
			index = firstIndex + 1;
			return rv;
		}

		public override string ToString()
		{
			return string.Format("CharDelegateComparison");
		}
	}
}
