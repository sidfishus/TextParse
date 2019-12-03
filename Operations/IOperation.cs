using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Parse.TextPatternParser
{
	public interface IOperation : IStatement
	{
		void Perform(
		 string input,
		 int pos,
		 int depth,
		 RunState runState,
		 out int index);
	}
}
