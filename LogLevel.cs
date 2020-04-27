using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sid.Parse.TextPatternParser
{
	// Bit fields
	public enum eLogLevel
	{
		MatchResult = 1,							// When a statement is executed
		SuccessfulMatch = 2,						// When a comparison matches
		AssertionNotRelevant=4,						// Log if an assertion is not relevant

	}
}