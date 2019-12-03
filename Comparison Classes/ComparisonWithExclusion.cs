using System;
using System.Collections.Generic;
using System.Text;

//not sure there is a point in this - use/extend PatternComparison
/*
namespace Sid.Parse.TextPatternParser
{
	public class ComparisonWithExclusion : ComparisonBase,IComparison
	{
		IComparison m_Comparison;
		IComparison m_Exclusion;
		//~V

		public ComparisonWithExclusion(
		 IComparison comp,
		 IComparison exclusion)
		{
			m_Comparison = comp;
			m_Exclusion = exclusion;
		}

		bool IComparison.Compare(
		 string input,
		 int pos,
		 out int index)
		{
			int afterComparison;
			if(m_Comparison.Compare(
			 input,
			 pos,
			 out afterComparison))
			{
				int unused;
				if(!m_Exclusion.Compare(
				 input,
				 pos,
				 out unused))
				{
					index = afterComparison;
					return true;
				}
			}

			index = -1;
			return false;
		}
	}
}
	*/