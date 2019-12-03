using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class OffsetAdvance : OperationBase, IOperation
	{
		int m_Offset; // How far to move
						  //~V

		public OffsetAdvance(
		 ILog log,
		 int offset) : base(log)
		{
			m_Offset = offset;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			//TODO what about going out of range
			index = pos + m_Offset;
		}

		public override string ToString()
		{
			return "OffsetAdvanceOperation";
		}
	}
}
