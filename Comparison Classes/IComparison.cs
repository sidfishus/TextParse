using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Parse.TextPatternParser
{
	public interface IComparison : IStatement
	{
		bool Compare(
		 string str,
		 int firstIndex,
		 int depth,
		 RunState runState);
	}
}
