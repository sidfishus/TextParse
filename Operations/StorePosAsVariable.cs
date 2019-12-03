using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	public class StorePosAsVariable : SetVariable
	{
		public StorePosAsVariable(
			ILog log,
			string varName) : base(log,varName,(input,pos,runState) => pos)
		{
		}

		public override string ToString()
		{
			return "StorePosAsVariable";
		}
	}
}
