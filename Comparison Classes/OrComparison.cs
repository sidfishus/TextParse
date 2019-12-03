using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class OrComparison : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		List<IComparisonWithAdvance> m_ComparisonList = new List<IComparisonWithAdvance>();
		//~V

		public OrComparison(
		 ILog log) : base(log)
		{
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// The order in which you add is very important because it is one which matches that controls how far to advance
		// after the match. Comparisons are executed in the order in which they are added.
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public void Add(
		 IComparisonWithAdvance comp)
		{
			m_ComparisonList.Add(comp);
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			// Find any which match
			int depthPlusOne = depth + 1;
			foreach (IComparisonWithAdvance comp in m_ComparisonList)
			{
				if (comp.CompareAndAdvance(
				 str,
				 firstIndex,
				 depthPlusOne,
				 runState,
				 out index))
				{
					return true;
				}
			}
			index = -1;
			return false;
		}

		public override string ToString()
		{
			return "OrComparison";
		}

		public int Count
		{
			get
			{
				return m_ComparisonList.Count;
			}
		}
	}
}
