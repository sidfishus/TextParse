using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public abstract class ComparisonWithAdvanceBase : ComparisonBase,IComparisonWithAdvance
	{
		public ComparisonWithAdvanceBase(
		 ILog log,
		 UserOnMatch fUserOnMatch = null) : base(log,fUserOnMatch)
		{
		}

		public ComparisonWithAdvanceBase(
		 ILog log,
		 Options options,
		 UserOnMatch fUserOnMatch = null) : base(log,options, fUserOnMatch)
		{

		}

		public bool CompareAndAdvance(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			base.PriorToPerform(input, pos, depth);
			bool matched=CompareAndAdvanceImp(
			 input,
			 pos,
			 depth,
			 runState,
			 out index);
			base.AfterPerform(matched, input, pos, depth, runState, index);
			return matched;
		}

		protected override bool CompareImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState)
		{
			int unused;
			return CompareAndAdvanceImp(
			 input,
			 pos,
			 depth,
			 runState,
			 out unused);
		}

		protected abstract bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index);
	}
}
