using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public abstract class OperationBase : StatementBase, IOperation
	{
		public OperationBase(
		 ILog log,
		 Options options) : base(log,options)
		{
		}

		public OperationBase(
		 ILog log) : base(log)
		{
		}

		public void Perform(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			base.PriorToPerform(input, pos, depth);
			PerformImp(input, pos, depth, runState, out index);
			base.AfterPerform(null, input, pos, depth, index);
		}

		// Convert an operation into a comparison
		public IComparisonWithAdvance AsComparisonWithAdvance()
		{
			var list = new StatementList(Log);
			list.Add(this);
			return list;
		}

		abstract protected void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index);
	}
}
