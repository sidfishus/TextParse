using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class ToggleLogStatus : OperationBase, IOperation
	{
		private bool m_Enable;

		public ToggleLogStatus(
		 ILog log,
		 bool enable) : base(log)
		{
			m_Enable = enable;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			if (Log!=null)
			{
				Log.Enable(m_Enable);
			}
			index = pos;
		}

		public override string ToString()
		{
			return "ToggleLogStatus";
		}
	}
}
