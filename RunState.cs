using System;
using System.Collections.Generic;
using Sid.Log;

namespace Sid.Parse.TextPatternParser
{
	// This class is to hold any state related information such as variables / functions
	public class RunState
	{
		private Dictionary<string, object> Variables = new Dictionary<string, object>();
		private Dictionary<string, fOperand<object>> Functions=new Dictionary<string,fOperand<object>>();
		// A list of assertions per statement
		private Dictionary<string,IList<IAssertion>> Assertions=new Dictionary<string,IList<IAssertion>>();

		public int BeginPos
		{
			get;
			set;
		}

		public void Clear()
		{
			Variables.Clear();
			BeginPos = -1;
			// Don't clear the functions!
		}

		public void SetVariable(
			string varName,
			object value)
		{
			Variables[varName] = value;
		}

		public object GetVariable(
			string varName)
		{
			object rv;
			if (!Variables.TryGetValue(varName, out rv))
			{
				throw new Exception($"Variable name '{varName}' does not exist.");
			}

			return rv;
		}

		// Functions arn't intended to be mutable. The recommended useage is that they are initialised prior to parsing
		// by using the InitRunState callback.
		public void SetFunction<T>(
			string funcName,
			fOperand<T> func
		) {
			// Have to convert the function to one which returns an object unfortunately
			Functions[funcName] = (int pos, string str, RunState runState) => func(pos, str, runState);
		}

		public T CallFunction<T>(
			string funcName,
			int pos,
			string str)
		{
			fOperand<object> func;
			if (!Functions.TryGetValue(funcName, out func))
			{
				throw new Exception($"Function with name '{funcName}' does not exist.");
			}

			return (T)func(pos, str, this);
		}

		public void AddAssertion(
			string statementName,
			IAssertion assertion
		) {
			IList<IAssertion> assertionList;
			if(!Assertions.TryGetValue(statementName, out assertionList)) {
				assertionList=new List<IAssertion>();
				Assertions.Add(statementName,assertionList);
			}

			assertionList.Add(assertion);
		}

		public void ExecutePostPerformAssertions(
			string statementName,
			string input,
			int beginPos,
			int? afterPos,
			bool? matched,
			ILog log
		) {

			const string emphasiseLine="******************************";

			IList<IAssertion> assertionList;
			if(Assertions.TryGetValue(statementName, out assertionList)) {
				for(int i=0;i<assertionList.Count;++i) {
					var assertion=assertionList[i];

					string errorMsg;
					bool? assertRv=assertion.Assert(
						input,
						beginPos,
						afterPos,
						this,
						matched,
						out errorMsg
					);

					if(assertRv== null) {
						if((log.GetLevel() & (int)eLogLevel.AssertionNotRelevant)>0) {
							log.LogLine(emphasiseLine);
							log.LogLine($"Assertion with name '{assertion.Name}' not relevant: beginPos={beginPos} "+
								$"({Parser.DisplayPartOfInputString(input,beginPos)}), afterPos={afterPos} "+
								$"({Parser.DisplayPartOfInputString(input,afterPos)})");
							log.LogLine(emphasiseLine);
						}
					}

					else if(assertRv.Value) {
						log.LogLine(emphasiseLine);
						log.LogLine($"Assertion with name '{assertion.Name}' succeeded: beginPos={beginPos}, "+
							$"({Parser.DisplayPartOfInputString(input,beginPos)}), afterPos={afterPos} " +
							$"({Parser.DisplayPartOfInputString(input,afterPos)})");
						log.LogLine(emphasiseLine);
					}

					else {
						log.LogLine(emphasiseLine);
						log.LogLine($"Assertion with name '{assertion.Name}' failed: {errorMsg}");
						log.LogLine(emphasiseLine);
						System.Diagnostics.Debug.Assert(false);
					}
				}
			}
		}
	}
}
