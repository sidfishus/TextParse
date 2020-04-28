
using System;
using Sid.Log;

namespace Sid.Parse.TextPatternParser {

    public static class UnitTests {

        public static void MatchVBScriptFunction(ILog log) {
            MatchVBScriptFunction_T1(log);
            MatchVBScriptFunction_T2(log);
        }

        public static void MatchVBScriptFunction_T1(ILog log) {
            const string input="ignorethis Func(1)";
            const string funcStr="Func";

            var statementList=new StatementList(log);

            var mainComp=TokenComparison.CreateVBScriptFunction(null,null,null,null,log);
            statementList.Add(new StorePosAsVariable(log,"BeginPos"));
            statementList.Add(mainComp);

            Action<RunState> InitRunState = (runState) => {
                {
                    var funcIndex=input.IndexOf(funcStr);
                    IAssertion sub=new PositionAssertion(true,funcIndex,funcIndex+funcStr.Length);
                    sub.Name="sub";
                    runState.AddAssertion("sub",sub);
                }

                {
                    var funcIndex=input.IndexOf(funcStr);
                    IAssertion isEndOrArgList=new PositionAssertion(
                        true,funcIndex+funcStr.Length,funcIndex+funcStr.Length+3
                    );
                    isEndOrArgList.Name="IsEndOrArgumentList";
                    runState.AddAssertion("IsEndOrArgumentList",isEndOrArgList);
                }
            };

            Func<RunState,string,int,string> ReplaceFunc = (runState,_input,pos) => {
                var beginPos=(int)runState.GetVariable("BeginPos");
                var str=input.Substring(beginPos,pos-beginPos);
                return str;
            };

            var parser=new Parser(log);
            int numMatches;
            parser.Extract(input,null,statementList,null,null,out numMatches,ReplaceFunc,InitRunState);
            System.Diagnostics.Debug.Assert(numMatches==1);
        }

        public static void MatchVBScriptFunction_T2(ILog log) {
            const string input="ignorethis obj.Func(1)";
            const string funcStr="obj.Func";

            var statementList=new StatementList(log);

            var mainComp=TokenComparison.CreateVBScriptFunction(null,null,null,null,log);
            statementList.Add(new StorePosAsVariable(log,"BeginPos"));
            statementList.Add(mainComp);

            Action<RunState> InitRunState = (runState) => {
                {
                    var funcIndex=input.IndexOf(funcStr);
                    IAssertion sub=new PositionAssertion(true,funcIndex,funcIndex+funcStr.Length);
                    sub.Name="sub";
                    runState.AddAssertion("sub",sub);
                }

                {
                    var funcIndex=input.IndexOf(funcStr);
                    IAssertion isEndOrArgList=new PositionAssertion(
                        true,funcIndex+funcStr.Length,funcIndex+funcStr.Length+3
                    );
                    isEndOrArgList.Name="IsEndOrArgumentList";
                    runState.AddAssertion("IsEndOrArgumentList",isEndOrArgList);
                }
            };

            Func<RunState,string,int,string> ReplaceFunc = (runState,_input,pos) => {
                var beginPos=(int)runState.GetVariable("BeginPos");
                var str=input.Substring(beginPos,pos-beginPos);
                return str;
            };

            var parser=new Parser(log);
            int numMatches;
            parser.Extract(input,null,statementList,null,null,out numMatches,ReplaceFunc,InitRunState);
            System.Diagnostics.Debug.Assert(numMatches==1);
        }

        public static void AddParenthesisToFunctionCalls(ILog log) {
            // This is used within the add parenthesis function
            MatchVBScriptFunction(log);

            AddParenthesisToFunctionCalls_T1(log);
            AddParenthesisToFunctionCalls_T2(log);
        }

        public static void AddParenthesisToFunctionCalls_T1(ILog log) {
            const string input="<% Response.Write 1, Func(1) %>";

            Action<Action<string, IAssertion>> fCreateAssertList = (fAddAssert) => {
            
                const string argsList="1, Func(1)";
                {
                    const string responseWrite=" Response.Write";
                    IAssertion funcNameAssertion=new PositionAssertion(
                        true,
                        input.IndexOf(responseWrite),
                        input.IndexOf(responseWrite)+responseWrite.Length
                    );
                    funcNameAssertion.Name="FunctionName";
                    fAddAssert(
                        "FunctionName",
                        funcNameAssertion
                    );
                }

                {
                    IAssertion argListAssertion=new PositionAssertion(
                        true,
                        input.IndexOf(argsList),
                        input.IndexOf(argsList)+argsList.Length
                    );
                    argListAssertion.Name="ArgumentList";
                    fAddAssert(
                        "ArgumentList",
                        argListAssertion
                    );
                }

                {
                    IAssertion argsNumberAssertion=new PositionAssertion(
                        true,
                        input.IndexOf(argsList),
                        input.IndexOf(argsList)+1
                    );
                    argsNumberAssertion.Name="ArgsNumber";
                    fAddAssert(
                        "ArgsNumber",
                        argsNumberAssertion
                    );
                }

                {
                    const string argsFunc="Func(1)";
                    IAssertion argsFuncWithParamAssert=new PositionAssertion(
                        true,
                        input.IndexOf(argsFunc),
                        input.IndexOf(argsFunc)+argsFunc.Length
                    );

                    argsFuncWithParamAssert.Name="Args Function";
                    fAddAssert(
                        "Args Function",
                        argsFuncWithParamAssert
                    );
                }
            };

            int numMatches;
            dotNETConversion.AddParenthesisToFunctionCalls(log,input,out numMatches,fCreateAssertList);
        }

        public static void AddParenthesisToFunctionCalls_T2(ILog log) {
            //input="<% Response.Write 1, obj.Func(1) %>";
        }
    }
}