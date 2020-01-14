using System;
using System.Collections.Generic;
using System.Text;

namespace Sid.Parse.TextPatternParser
{
	// Serves as the base class for parse statements/comparisons
	public interface IStatement
	{
		string Name {
			get;
			set;
		}
	}
}
