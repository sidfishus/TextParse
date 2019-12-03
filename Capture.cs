using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class Capture : ComparisonWithAdvanceBase, IComparisonWithAdvance
	{
		public Capture(ILog log) : base(log)
		{
		}

		public IComparisonWithAdvance Comparison
		{
			get;
			set;
		}

		protected override bool CompareAndAdvanceImp(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index)
		{
			bool rv = Comparison.CompareAndAdvance(input, pos, depth+1,runState,out index);
			if (rv)
			{
				// Do the capture
				Length = index - pos;
				Captured = input.Substring(pos, Length);
				BeginPos = pos;
				EndPos = index;
			}

			return rv;
		}

		public string Captured
		{
			get;
			private set;
		}

		public int BeginPos
		{
			get;
			private set;
		}

		public int EndPos
		{
			get;
			private set;
		}

		public int Length
		{
			get;
			private set;
		}

		public override string ToString()
		{
			return "Capture";
		}
	}
}
