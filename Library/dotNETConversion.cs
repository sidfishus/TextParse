using System;
using Sid;
using Sid.Log;
using System.Collections.Generic;
using System.Text;

namespace Sid.Parse.TextPatternParser
{
	public static class dotNETConversion
	{
		// Only parse when inside <% %> and not inside double quotes
		public static List<State> VBScriptState(
		 ILog log,
		 Options parserOptions)
		{
			List<State> stateList = new List<State>();

			// Page directive state
			StringComparison insidePageDirective = new StringComparison(log, parserOptions, "<%");
			StringComparison outsidePageDirective = new StringComparison(log, parserOptions, "%>");
			State insidePageDirectiveState = new State(insidePageDirective, outsidePageDirective, false);
			stateList.Add(insidePageDirectiveState);

			// Not inside quotes state
			CharComparison isQuote = new CharComparison(log, parserOptions, '\"');
			State notInsideQuotesState = new State(isQuote, isQuote, true);
			notInsideQuotesState.Name = "QuoteState";
			stateList.Add(notInsideQuotesState);

			return stateList;
		}

		// From "response.write blah"
		// To "response.write(blah)"
		public static string AddParenthesisToFunctionCalls(
		 ILog log,
		 string input,
		 out int numMatches)
		{
			// The parser object that will do the work
			Parser parser = new Parser(log);

			//// Parser options
			Options parserOptions = new Options(log);
			parserOptions.CaseSensitive = false;

			CharDelegateComparison isWhitespace = new CharDelegateComparison(log, Char.IsWhiteSpace);
			IOperation skipWhitespace = TokenComparison.SkipWhitespace(log);

			// Line wrap
			parserOptions.LineWrap = TokenComparison.VBScriptLineWrap(
			 log,
			 parserOptions);

			// State
			List<State> stateList = VBScriptState(log, parserOptions);

			// The main statement list
			StatementList mainStatements=new StatementList(log);

			// Capturing
			Dictionary<string, Capture> capturing = new Dictionary<string, Capture>();

			// Function name capture
			Capture funcNameCapture = new Capture(log);
			funcNameCapture.Name = "FunctionName";
			capturing.Add("funcName", funcNameCapture);
			mainStatements.Add(funcNameCapture);

			// Function name capture statements
			var vbScriptKeywords = TokenComparison.VBScriptKeywords(log, parserOptions);
			StatementList funcNameCaptureStatements = new StatementList(log);
			funcNameCapture.Comparison = funcNameCaptureStatements;
			funcNameCaptureStatements.Exclusion = vbScriptKeywords;

			//// Starts with whitespace or is the start of the input string
			StartOfInputStringComparison isStartOfString = new StartOfInputStringComparison(log);
			OrComparison startsWithWhitespaceOrIsStartOfInputString = new OrComparison(log);
			startsWithWhitespaceOrIsStartOfInputString.Add(isStartOfString);
			startsWithWhitespaceOrIsStartOfInputString.Add(isWhitespace);
			funcNameCaptureStatements.Add(startsWithWhitespaceOrIsStartOfInputString);

			// Skip whitespace
			funcNameCaptureStatements.Add(skipWhitespace);
			
			//// Function/sub name.
			//// For example: class1.Func or class1.Func"string arg"
			funcNameCaptureStatements.Add(TokenComparison.CreateVBScriptSub(parserOptions, name: null, log: log));

			//// Arguments list
			// Skip whitespace and don't capture it
			mainStatements.Add(skipWhitespace);

			// Capture
			Capture argListCapture = new Capture(log);
			capturing.Add("funcArgs", argListCapture);
			mainStatements.Add(argListCapture);

			// Function argument list capture statements
			StatementList argListCaptureStatements = new StatementList(log);
			argListCapture.Comparison = argListCaptureStatements;

			// Skip the line wrap (and capture it)
			argListCaptureStatements.Add(parserOptions.SkipLineWrapOperation);

			// Concatenated single arguments
			// Delimited by whitespace, comma & or +
			OrComparison vbScriptConcatCommaOrWhitespace = new OrComparison(log);
			vbScriptConcatCommaOrWhitespace.Name = "Args concat or delimiter";
			vbScriptConcatCommaOrWhitespace.Add(isWhitespace);
			CharComparison isAmpersand = new CharComparison(log, parserOptions, '&');
			CharComparison isPlus = new CharComparison(log, parserOptions, '+');
			CharComparison isComma = new CharComparison(log, parserOptions, ',');
			vbScriptConcatCommaOrWhitespace.Add(isAmpersand);
			vbScriptConcatCommaOrWhitespace.Add(isPlus);
			vbScriptConcatCommaOrWhitespace.Add(isComma);

			// Numbers
			var numberComparison =
			 TokenComparison.CreateNumber(parserOptions, vbScriptConcatCommaOrWhitespace, "Args number", log: log);
			// VB script quoted strings
			var quotedText = TokenComparison.CreateVBScriptQuotedString(parserOptions, name: "Args quoted text", log: log);
			// VB script function which could include arguments
			var vbScriptFunc = TokenComparison.CreateVBScriptFunction(
			 parserOptions,
			 vbScriptConcatCommaOrWhitespace,
			 vbScriptKeywords,
			 "Args Function",
			 log: log);

			// Individual arguments
			// Types
			OrComparison individualArgumentTypes = new OrComparison(log);
			individualArgumentTypes.Add(numberComparison);
			individualArgumentTypes.Add(quotedText);
			individualArgumentTypes.Add(vbScriptFunc);

			// List delimiter for the individual arguments
			OrComparison vbScriptConcactOrComma = new OrComparison(log);
			vbScriptConcactOrComma.Add(isAmpersand);
			vbScriptConcactOrComma.Add(isPlus);
			vbScriptConcactOrComma.Add(isComma);

			DelimitedListComparison individualArgumentList = new
			 DelimitedListComparison(log, parserOptions, individualArgumentTypes, seperator: vbScriptConcactOrComma);
			individualArgumentList.MinAmount = 1;
			individualArgumentList.ItemTrim = skipWhitespace;

			// The argument list - comma seperated list of function arguments
			DelimitedListComparison argumentList = new DelimitedListComparison(log, parserOptions, individualArgumentList, seperator: isComma);
			argumentList.Name = "ArgumentList";
			argumentList.MinAmount = 1;
			argumentList.ItemTrim = skipWhitespace;
			argListCaptureStatements.Add(argumentList);

			const string replaceWith = "'funcName'('funcArgs')";
			string replaced = parser.Replace(input, replaceWith, mainStatements, capturing, stateList, out numMatches);
			return replaced;
		}

		//TODO this is not fully tested. needs a new feature to allow repeats
		// - "repeated comparison"
		// - also doesn't work inside quotes. needs a state
		//  I think state ment list should have a state member
		// <script ..>function blah end function </script>
		public static string WrapFunctionsInScriptBlock(
		 ILog log,
		 string input)
		{
			// The parser object that will do the work
			Parser parser = new Parser(log);

			//// Parser options
			Options parserOptions = new Options(log);
			parserOptions.CaseSensitive = false;

			// Line wrap
			parserOptions.LineWrap = TokenComparison.VBScriptLineWrap(
			 log,
			 parserOptions);

			// State
			List<State> stateList = VBScriptState(log, parserOptions);

			// The main statement list
			StatementList mainStatements = new StatementList(log);

			// Begins with whitespace (don't capture the space)
			CharDelegateComparison isWhitespace = new CharDelegateComparison(log, Char.IsWhiteSpace);
			mainStatements.Add(isWhitespace);

			// Skip whitespace (don't capture the space)
			mainStatements.Add(TokenComparison.SkipWhitespace(log));

			// Capturing
			Dictionary<string, Capture> capturing = new Dictionary<string, Capture>();

			// The capture
			Capture capture = new Capture(log);
			capture.Name = "capture";
			capturing.Add("functionDefinition", capture);
			mainStatements.Add(capture);

			// Capture statements
			StatementList captureStatements = new StatementList(log);
			capture.Comparison = captureStatements;

			// Function or sub
			OrComparison functionOrSub = new OrComparison(log);
			functionOrSub.Add(new StringComparison(log, parserOptions, "function"));
			functionOrSub.Add(new StringComparison(log, parserOptions, "sub"));
			captureStatements.Add(functionOrSub);

			// Whitespace
			captureStatements.Add(isWhitespace);

			// Line wrap
			captureStatements.Add(parserOptions.SkipLineWrapOperation);

			//// Name of function/sub
			// End is either whitespace or (
			OrComparison whiteSpaceOrOpenParen = new OrComparison(log);
			whiteSpaceOrOpenParen.Add(isWhitespace);
			whiteSpaceOrOpenParen.Add(new CharComparison(log,parserOptions,'('));

			var funcName = TokenComparison.CreateIdentifier(
			 parserOptions,
			 end: whiteSpaceOrOpenParen,
			 exclusion: null,
			 name: null,
			 log: log);

			captureStatements.Add(funcName);

			//// Find the end of the sub/function
			StatementList functionEnd = new StatementList(log);
			// Whitespace
			functionEnd.Add(isWhitespace);

			// 'end'
			functionEnd.Add(new StringComparison(log, parserOptions, "end"));

			// Whitespace
			functionEnd.Add(isWhitespace);

			// Skip the line wrap
			functionEnd.Add(parserOptions.SkipLineWrapOperation);

			// Function or sub
			functionEnd.Add(functionOrSub);

			// Whitespace or %>
			OrComparison whitespaceOrPageDirective = new OrComparison(log);
			whitespaceOrPageDirective.Add(isWhitespace);
			whitespaceOrPageDirective.Add(new CompareNoAdvance(log,new StringComparison(log, parserOptions, "%>")));

			functionEnd.Add(whitespaceOrPageDirective);

			// Skip till find the function name
			captureStatements.Add(new AdvanceUntilComparison(log, functionEnd));

			//const string replaceWith = "'funcName'('funcArgs')";
			string replaceWith = string.Format(
			 "{0}<script language=\"VB\" runat=\"Server\">{0}'functionDefinition'{0}</script>{0}",
			 Environment.NewLine);
			string replaced = parser.Replace(input, replaceWith, mainStatements, capturing, stateList);
			return replaced;
		}
	}
}