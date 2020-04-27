using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public abstract class ComparisonBase : StatementBase,IComparison
	{
		protected ComparisonBase(
		 ILog log,
		 Options options,
		 UserOnMatch fUserOnMatch = null) : base(log,options,fUserOnMatch)
		{
		}

		protected ComparisonBase(ILog log, UserOnMatch fUserOnMatch = null) : base(log, fUserOnMatch)
		{
		}

		public bool Compare(
		 string input,
		 int pos,
		 int depth,
		 RunState runState)
		{
			base.PriorToPerform(input, pos, depth);
			bool matched=CompareImp(input,pos, depth, runState);
			base.AfterPerform(matched, input, pos, depth, runState, null);
			return matched;
		}

		abstract protected bool CompareImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState);
	}
}
