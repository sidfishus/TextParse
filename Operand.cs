using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	delegate T Operand<T>(int pos, string str, RunState runState);
}