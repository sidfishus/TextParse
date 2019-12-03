using System;
using System.Collections.Generic;
using System.Text;
using Sid.Log;

//TODO this class needs a better more general name: "built in comparisons", "pre-created comparisons", "comparison library"
namespace Sid.Parse.TextPatternParser
{
	// This is like a helper class, to save having to type this out every time a token match is required
	public static class TokenComparison
	{
		readonly static string[] VBKeywords = new string[]
		{
			"if",
			"then",
			"else",
			"end",
			"wend",
			"do",
			"while",
			"to",
			"step",
			"exit",
			"for",
			"loop",
			"select",
			"case",
			"dim",
			"redim",
			"function",
			"sub",
			"call",
			"option",
			"and",
			"or",
			"mod",
		};

		// E.g. Test123 matches
		// 1Test123 doesn't (cann't lead with a number)
		// test"123 doesn't match (can only contain letters and numbers)
		public static IComparisonWithAdvance CreateIdentifier(
		 Options options,
		 IComparisonWithAdvance end,
		 IComparison exclusion,
		 string name=null,
		 ILog log=null)
		{
			StatementList stmtList = new StatementList(log);
			stmtList.Name = name;

			PatternComparison patternComp = new PatternComparison(log,options);
			patternComp.EndComparison = end;
			patternComp.MinLength = 1;

			// First character is a letter
			var firstCharacter = new ComparisonRange<IComparisonWithAdvance>();
			firstCharacter.Range.Min = 1;
			firstCharacter.Range.Max = 1;
			var isLetter = new CharDelegateComparison(log,Char.IsLetter);
			firstCharacter.Comparison = isLetter;
			patternComp.AddComparisonRange(firstCharacter);

			// Subsequent (2+) characters can be a letter or digit (0-9)
			var subsequentCharacters = new ComparisonRange<IComparisonWithAdvance>();
			subsequentCharacters.Range.Min = 2;
			var isLetterOrDigit = new CharDelegateComparison(log,Char.IsLetterOrDigit);
			subsequentCharacters.Comparison = isLetterOrDigit;
			patternComp.AddComparisonRange(subsequentCharacters);

			stmtList.Add(patternComp);

			stmtList.Exclusion = exclusion;

			return stmtList;
		}

		public static IComparisonWithAdvance CreateListOfTokens(
		 Options options,
		 IComparisonWithAdvance token,
		 IComparisonWithAdvance segment,
		 string name = null,
		 ILog log = null)
		{
			DelimitedListComparison list = new DelimitedListComparison(log,options, token, segment);
			list.MinAmount = 1;
			list.Name = name;

			return list;
		}

		public static IComparisonWithAdvance CreateNumber(
		 Options options,
		 IComparisonWithAdvance end,
		 string name = null,
		 ILog log = null)
		{
			//// List of 1 or 2 sets of numbers seperated by a .
			StatementList stmtList = new StatementList(log);
			stmtList.Name = name;

			OrComparison isDotOrEnd = new OrComparison(log);
			isDotOrEnd.Add(end);
			ICharComparison isDot = new CharComparison(log, options, '.');
			isDotOrEnd.Add(isDot);
			PatternComparison patternComp = new PatternComparison(log,options);
			patternComp.MinLength = 1;
			patternComp.EndComparison = isDotOrEnd;

			// All characters must be between 0 and 9
			var allCharacters = new ComparisonRange<IComparisonWithAdvance>();
			allCharacters.Range.Min = 1;
			var isNumber = new CharDelegateComparison(log,Char.IsNumber);
			allCharacters.Comparison = isNumber;
			patternComp.AddComparisonRange(allCharacters);

			// List consisting of 1 or 2 items seperated by .
			DelimitedListComparison functionName = new DelimitedListComparison(log,options, patternComp, isDot);
			functionName.MinAmount = 1;
			functionName.MaxAmount = 2;
			stmtList.Add(functionName);
			return stmtList;
		}

		public static IComparisonWithAdvance CreateVBScriptQuotedString(
		 Options options,
		 string name = null,
		 ILog log=null)
		{
			// We can use a pattern comparison to do this
			// First character is a quote
			// Subsequent characters can either be anything BUT a single ", or double "" but advance 2.
			// Although it seems un-intuitive this gives us the behaviour we require: skip double quotes.

			var isQuote = new CharComparison(log,options, '\"');

			// This requires more than one statement
			StatementList statementList = new StatementList(log);
			statementList.Name = name;

			// The pattern comparison that will do the work
			PatternComparison patternComp = new PatternComparison(log,options);
			statementList.Add(patternComp);
			patternComp.MinLength = 2;

			// First character is open quote
			var first = new ComparisonRange<IComparisonWithAdvance>();
			first.Range.Min = 1;
			first.Range.Max = 1;
			first.Comparison = isQuote;
			patternComp.AddComparisonRange(first);

			// Subsequent characters
			var subsequent = new ComparisonRange<IComparisonWithAdvance>();
			subsequent.Range.Min = 2;
			OrComparison isNotSingleQuoteOrIsDoubleQuotesAndAdvance2 = new OrComparison(log);
			var notQuote = new NotCharComparison(log,isQuote);
			isNotSingleQuoteOrIsDoubleQuotesAndAdvance2.Add(notQuote);
			var doubleQuote = new StringComparison(log,options, "\"\"");
			isNotSingleQuoteOrIsDoubleQuotesAndAdvance2.Add(doubleQuote);
			subsequent.Comparison = isNotSingleQuoteOrIsDoubleQuotesAndAdvance2;
			patternComp.AddComparisonRange(subsequent);

			// End when find the terminating "
			patternComp.EndComparison = isQuote;

			// Skip the trailing quote by advancing by 1
			statementList.Add(new OffsetAdvance(log,1));
			return statementList;
		}

		public static IOperation SkipWhitespace(
		 ILog log)
		{
			
			AdvanceWhileOperation skipWhitespace = new AdvanceWhileOperation(log, IsWhitespace(log));
			return skipWhitespace;
		}

		public static ICharComparison IsWhitespace(
		 ILog log)
		{
			CharDelegateComparison isWhitespace = new CharDelegateComparison(log, Char.IsWhiteSpace);
			return isWhitespace;
		}

		public static IComparisonWithAdvance CreateVBScriptSub(
		 Options options,
		 string name=null,
		 ILog log=null)
		{
			// This requires more than one statement
			StatementList statementList = new StatementList(log);
			statementList.Name = name;

			// Function end: . , ", or whitespace
			CharComparison isDot = new CharComparison(log,options, '.');
			CharComparison isQuote = new CharComparison(log,options, '\"');
			CharDelegateComparison isWhitespace = new CharDelegateComparison(log,Char.IsWhiteSpace);
			OrComparison funcNameEnd = new OrComparison(log);
			funcNameEnd.Add(isDot);
			funcNameEnd.Add(isQuote);
			funcNameEnd.Add(isWhitespace);

			// Identifier but not including classes/namespaces
			var identifier = TokenComparison.CreateIdentifier(
			 options,
			 end: funcNameEnd,
			 exclusion:null,
			 name:null,
			 log:log);

			// List of identifiers seperated by . which will include classes/namespaces
			var sub = TokenComparison.CreateListOfTokens(
			 options,
			 token: identifier,
			 segment: isDot,
			 name:null,
			 log:null);
			statementList.Add(sub);

			return statementList;
		}

		// E.g.
		// class.Function _ \r\n (p1, p2, class.Function2( _\r\n p1,p2 ) )
		// or
		// class.FunctionWithNoArgs
		public static IComparisonWithAdvance CreateVBScriptFunction(
		 Options options,
		 IComparisonWithAdvance end,
		 IComparison vbScriptKeywords,
		 string name = null,
		 ILog log=null)
		{
			if (options.SkipLineWrapOperation==null)
			{
				Parser.ThrowParseError("The line wrap comparison has not been specified. This is mandatory.");
			}

			// This requires more than one statement
			StatementList statementList = new StatementList(log);
			statementList.Name = name;

			// Function
			CharComparison isDot = new CharComparison(log, options, '.');
			OrComparison isDotOrEnd=new OrComparison(log);
			isDotOrEnd.Add(isDot);
			isDotOrEnd.Add(end);
			var identifier = TokenComparison.CreateIdentifier(
			 options,
			 end:isDotOrEnd,
			 exclusion: vbScriptKeywords,
			 name: null,
			 log: log);

			// List of identifiers seperated by . which will include classes/namespaces
			var sub = TokenComparison.CreateListOfTokens(
			 options,
			 token: identifier,
			 segment: isDot,
			 name: null,
			 log: null);
			statementList.Add(sub);

			// Skip whitespace
			statementList.Add(SkipWhitespace(log));

			// Skip the line wrap character
			statementList.Add(options.SkipLineWrapOperation);

			// Function parenthesis and arguments
			CharComparison open = new CharComparison(log, options, '(');
			CharComparison close = new CharComparison(log, options, ')');

			// Either the end (and don't advance past it), or function argument list
			OrComparison isEndOrArgumentList = new OrComparison(log);
			isEndOrArgumentList.Add(new CompareNoAdvance(log,end));
			isEndOrArgumentList.Add(new NestedOpenCloseComparison(log, open, close));

			statementList.Add(isEndOrArgumentList);

			return statementList;
		}

		public static IComparisonWithAdvance VBScriptLineWrap(
		 ILog log,
		 Options options)
		{
			// In VB script you can have the underscore character which signifies wrapping to the next line.
			// E.g:
			// Response.Write _
			//  (test & _
			//  args)
			IOperation skipWhitespace = SkipWhitespace(log);
			StatementList lineWrap = new StatementList(log);
			lineWrap.Add(skipWhitespace);
			var isUnderscore = new CharComparison(log, options, '_');
			lineWrap.Add(isUnderscore);
			lineWrap.Add(skipWhitespace);

			return lineWrap;
		}

		public static IComparisonWithAdvance VBScriptKeywords(
		 ILog log,
		 Options options)
		{
			// Need more than one statement to achieve this
			StatementList statements=new StatementList(log);

			// Start of the string

			//// Starts with whitespace or is the start of the input string
			StartOfInputStringComparison isStartOfString = new StartOfInputStringComparison(log);
			var isWhitespace=IsWhitespace(log);
			OrComparison startsWithWhitespaceOrIsStartOfInputString = new OrComparison(log);
			startsWithWhitespaceOrIsStartOfInputString.Add(isStartOfString);
			startsWithWhitespaceOrIsStartOfInputString.Add(isWhitespace);
			statements.Add(startsWithWhitespaceOrIsStartOfInputString);

			// Trim left - skip whitespace
			var skipWhitespace = SkipWhitespace(log);
			statements.Add(skipWhitespace);

			//// Keywords
			OrComparison orComp = new OrComparison(log);
			statements.Add(orComp);
			for (int i = 0; i < VBKeywords.Length; ++i)
			{
				orComp.Add(new StringComparison(log, options, VBKeywords[i]));
			}

			// Trim right
			statements.Add(skipWhitespace);

			return statements;
		}
		
		public static IComparisonWithAdvance HTMLTag(
		 ILog log,
		 Options options,
		 string tagName,
		 OrComparison attributes,
		 string name=null)
		{
			ICharComparison isWhitespace = IsWhitespace(log);
			IOperation skipWhitespace=SkipWhitespace(log);

			StatementList statementList = new StatementList(log);
			statementList.Name = name;

			statementList.Add(new CharComparison(log, options, '<'));
			statementList.Add(new StringComparison(log, options, tagName));
			statementList.Add(isWhitespace);

			// The reason for the or list is in case the tags appear in a different order on different browsers
			DelimitedListComparison list = new DelimitedListComparison(log, options, attributes, isWhitespace);
			statementList.Add(list);
			list.MinAmount = attributes.Count;
			list.MaxAmount = attributes.Count;

			statementList.Add(skipWhitespace);
			statementList.Add(new CharComparison(log, options, '>'));

			return statementList;
		}
	}
}
