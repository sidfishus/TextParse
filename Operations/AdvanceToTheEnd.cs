using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class AdvanceToTheEnd : OperationBase, IOperation
	{
		public AdvanceToTheEnd(
		 ILog log) : base(log)
		{
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = input.Length;
		}

		public override string ToString()
		{
			return "AdvanceToTheEnd";
		}
	}
}
