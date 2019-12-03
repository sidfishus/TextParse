using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class Advance : OperationBase, IOperation
	{
		Func<string,int,int,RunState,int> m_fOffset;

		public Advance(
		 ILog log,
		 Func<string, int, int, RunState, int> fOffset) : base(log)
		{
			m_fOffset = fOffset;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			index = m_fOffset(input,pos,depth,runState);
		}

		public override string ToString()
		{
			return "AdvanceOperation";
		}
	}
}
