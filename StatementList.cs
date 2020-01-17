using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class StatementList : ComparisonWithAdvanceBase,IComparisonWithAdvance
	{
		List<IStatement> m_Statements = new List<IStatement>();
		//~V

		public StatementList(
		 ILog log) : base(log)
		{
		}

		public override string ToString()
		{
			return "StatementList";
		}

		public void Add(
		 IStatement stmt)
		{
			m_Statements.Add(stmt);
		}

		public int Count
		{
			get
			{
				return m_Statements.Count;
			}
		}

		public IComparison Exclusion
		{
			get;
			set;
		}

		protected override bool CompareAndAdvanceImp(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = -1;
			// Statement index
			int stmtIndex;
			bool failedMatch = false;
			int pos;
			int depthPlusOne = depth + 1;
			for (stmtIndex = 0, pos = firstIndex; !failedMatch && stmtIndex < m_Statements.Count;
			 ++stmtIndex)
			{
				// Key nuance design decision. It's OK to perform operations/comparisons even if we have reached the end of
				//  the string. This is necessary for things like setting variables, comparing against the end of the
				//  string e.t.c. No doubt there will be bugs presuming this isn't the case but I've only realised
				//  retrospectively that this is a requirement.
				IStatement stmt = m_Statements[stmtIndex];
				if (stmt is IOperation)
				{
					IOperation op = (IOperation)stmt;
					op.Perform(str, pos, depthPlusOne, runState, out pos);
				}
				else
				{
					if (stmt is IComparisonWithAdvance)
					{
						IComparisonWithAdvance comp = (IComparisonWithAdvance)stmt;
						int previousPos = pos;
						if (!comp.CompareAndAdvance(str, pos, depthPlusOne, runState, out pos))
						{
							// This part of the pattern match failed. Start again (using the outer loop/index) but at
							//  the next character
							failedMatch = true;
						}
					}
					else if (stmt is IComparison)
					{
						//sidtodo not sure about this - I'm not sure the design of the statement classes is right
						IComparison comp = (IComparison)stmt;
						if (!comp.Compare(str, pos, depthPlusOne, runState))
						{
							failedMatch = true;
						}
					}
				}
			}

			// Got this far without a match?
			if (!failedMatch)
			{
				// Did we execute all statements?
				if (stmtIndex == m_Statements.Count)
				{
					// Exclusions?
					if(Exclusion!=null)
					{
						string subStr = str.Substring(firstIndex, pos - firstIndex);
						failedMatch = Exclusion.Compare(subStr, 0, depthPlusOne, runState);
					}
					if (!failedMatch)
					{
						// Successful match
						index = pos;
					}
				}
				else
				{
					// We did not complete all of the comparisons
					// Go to the next character
					// Potentially a speedup here? If we reach the end of the input string but cannot complete all
					//  comparisons then copy the remaining string rather than trying to do any more comparisons?
					failedMatch = true;
				}
			}

			return !failedMatch;
		}
	}
}
