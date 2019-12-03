using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class SetLogLevel : OperationBase, IOperation
	{
		private int m_Level;

		public SetLogLevel(
		 ILog log,
		 int level) : base(log)
		{
			m_Level = level;
		}

		protected override void PerformImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			if (Log != null)
			{
				Log.SetLevel(m_Level);
			}
			index = pos;
		}

		public override string ToString()
		{
			return "SetLogLevel";
		}
	}
}
