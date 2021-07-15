# TextParse

TextParse is a .NET C# library I created for parsing and replacing text. The principal behind it is that the user uses C# class syntax to describe a series of steps that encapsulates a parse routine which is then executed iteratively in a linear fashion against the specified input text. The use case for it's creation was to create a parser that: 
1. Can parse complex formats.
1. Is accurate.
1. Terse but easily understandable for a programmer.
1. Gives good feedback and diagnostics.
1. Is easy to reuse.
1. Is expandable.
1. Can be used with automatic unit and regression testing.

See below for more details.

# Concept
## Background
I was tasked with a project to convert a large ERP web application from classic ASP to ASP .NET because support for it from Microsoft was soon to expire. The application consisted of 13 modules including 100's of sourcecode files and after making the changes necessary to convert a single file it was clear that an automated process was needed.

Classic ASP uses the VB script language for dynamic rendering whereas ASP .NET uses VB .NET. After manually converting a couple of code files to ASP .NET there seemed to be approximately 20 syntactical differences between the languages. Effectively what I was looking to achieve was to create a transpiler that converts from VB script to VB .NET. 
I fathomed that if each 'type' of syntax conversion would take 1-3 days to complete it would be an acceptable amount of time to automate the conversion of the entire application considering there are approximately 20 of them.

I started my investigation by trying regular expressions which I've successfully used to do simple parsing in the past. I quickly gave up on this idea because it was clear I would have to become the regex master of master's in order to achieve what I needed. I also did not appreciate the unintuitive syntax, lack of readability, or the inability to extend or add diagnostic tools or gain any kind of feedback. It seemed to me like a black box consisting of only mathematical jargon. I then considered using other existing parsing tools but steered away from this because I did not want to spend time learning somebody else's work only to be frustrated with it's usage and learn it only does 90% of what I need. If I created my own parser however I would be completely in control and could design it from the ground up to work exactly how I wanted it to.

My process in theory was to:
1. Choose a module and run the conversion against it.
1. Test that module until a bug/syntax error was found.
1. Fix the parse operation that deals with converting that particular syntax or create a new one.
1. Goto step 1 until no more bugs are found during testing.
1. Move on to the next module but include the module(s) already converted.

After each execution of the conversion I could use Microsoft Team Foundation Studio to 'diff' the changed files for changes before checking them in. This would serve as a method of regression testing and unit testing to ensure that any new changes to the library and conversion operations made in that iteration had not broken anything, so I was never going two steps forward and then one step back. The annotate ('blame' in Git) feature would come in handy when errors were found to determine at what iteration a breaking change was introduced and therefore give me a starting point for fixing the issue.

The parser library itself was completed but the conversion project was stopped early on after I had successfully completed and tested 2 of the syntax changing operations. I have since used it to parse and transform XML and HTML and other formats of text and it is always my first choice for any parsing project.

## Theory

### User Created Parse Algorithms (UCPA)

I figured that it should be possible be able to parse and replace/convert anything providing I could describe the routine as a series of steps and checks. E.g. to match against words that contain only a series of lowercase a-z characters you could describe the algorithm in psuedo as follows:
1. Validate that we are at the beginning of the input text, or the preceding character is whitespace. ```// Validate beginning of word``` 
1. Validate that the character at the current position is lowercase and is a-z. ```// Validate word is at least one character in length```
1. Move until a character is found that is not lower case a-z, or we find the end of the string. ```// Find the first non a-z character```
1. Validate that we have reached the end of the text, or the current character is a space. ```// Reached the end of the word```

When described in this manner it is very easy to understand the intention and purpose of each step as well as the algorithm as a whole - at least it seems this way to me as someone who has been programming since 2001. Furthermore, if the four steps are encapsulated into their own sub routine and are given an apt name it could be reused in future. This will reduce bugs (it's already tested) and increase the readability of parse algorithms by making them more terse (remove duplication by turning 4 steps in to 1). It's also trivial to create unit tests to prove the accuracy of the sub routine as well as provide regression testing as the parse library evolves over time.

### Outer Linear Parse Algorithm (OLPA)

The next thing to consider is how the UCPA should be executed and therefore there are 2 algorithms in play:

1. The OLPA which executes in a linear fashion starting at the first character and stops when the end of the input text is found. At each iteration the UCPA is executed from the OLPA's current position. If an iteration results in an unsuccessful match from the UCPA the OLPA will incremement it's last recorded position by 1 and continue iterating from there. However if the iteration results in a successful match from the UCPA, the OLPA records the position of where the UCPA finishes and continues iterating from the new position.
1. The UCPA starts at the position passed to it by the OLPA and executes parse statements in a sequential manner until a validation returns false or there are no more parse statements. Each time a parse statement is executed the resulting position is passed on to the next parse statement in the sequence. The UCPA can move anywhere in the input string.

### Individual Statements

Each individual parse statement would be an instance of a given type of operation/validation that would be passed the same parameters to perform it's specific task. The parameters/result value are:
1. The input text that is being parsed.
1. The position within the input text to begin at.
1. An object that can be used to access and update state information such as UCPA defined variables.
1. The output position within the input text, or a value to indicate stop/unsuccessful match (result).

New statements could be added over time as/when needed to further extend the parse library and make it as reusable as possible. I.e. there would be a built in statement type that acts as a string comparison, and a statement type that acts as the 'OR' operator, for example. And it should be possible to combine statements together to increase their reusability.

## Practical

### Initial Approach ###

Now I could describe what I wanted to achieve, I needed a way of conveying that to an application so that it could be translated and executed but at the same time remain understandable to a computer programmer.

My first idea was to create a psuedo language that could be parsed and converted to a list of parse statement objects (I was heavily into OO at the time) that would make up the parse statement list (UCPA) that would be passed to the OLPA for execution. The parse statement classes would all derive from a common interface (IComparisonWithAdvance) and leverage polymorphism to allow them to be used via a reference to the interface. This is key because it allows any type, combination, or list of parse statements to be used anywhere that a single parse statement is required because everything that can be executed as part of a UCPA derives from the same interface and implements the same parse interface method. For example this is very useful for the 'or' statement which takes a list of parse statements and executes them sequentially until a match is found because it makes the following algorithm possible:
1. Compare against string 'test' (string comparison)
1. OR
   1. Compare against string 'hello'
   1. AND THEN Compare against a single character in the sequence 1, 2, or 3.

The second parse statement of the 'or' is a combination of statements and because the parse statement list class ('StatementList') derives from the same interface as an individual parse statement (IComparsonWithAdvance), the parse statement list type can be used anywhere a parse statement is required. Thus removes the need for the caller to have any knowledge of how the parse statement is implemented. The only contract between the 'or' statement and it's child parse statements is that the child will be given the input text and a starting position and will return an output position alongside a result indicating whether it was a successful match.

The statement types can be 1 of 2 categories:
- Comparison: validate from the given position, return the output position, and return true false to indicate whether the match was successful.
- Operation: do something and return the output position. For example this could be setting a user defined variable where the output position will be returned as the input position, or moving to elsewhere within the input text and returning that position.

The only difference in terms of syntax/execution is that an operation cannot cause the parsing to stop. I've since concluded that this distinction of categories is not necessary and the same result could be achieved by simply returning the output position or -1 to indicate an unsuccessful validation.

### Programming Language/Platform ###

I had already decided that I would be designing this using an object oriented approach. So I decided to create the library in .NET and C# due to the plethora of inbuilt functionality and simple but intuitive OO syntax. Having a gargage collector would also reduce the complexity allowing me to focus on making the parser as accurate and resilient as possible. I was focusing on accuracy and human readability as opposed to performance, however after parsing entire directories containing 100's of files I am more than satisfied with the performance.

### Progress ###

I quickly gave up on the idea of creating a psuedo language because it was taking me too long to parse it and was too verbose and difficult to follow when describing complex parse algorithm's. However the C# class syntax I was already using to create the parse statement objects as a result of parsing the psuedo language seemed appropriate for the task and had the added benefit of removing the need for an intermediate language.

### Simple Example ###

Below is a simple example I created for the purpose of illustrating the syntax of a TextParse program. For more complex examples see the built in examples at https://chrissiddall.azurewebsites.net/textparse:

```
using System;
using Sid.Parse.TextPatternParser;
using StringComparison = Sid.Parse.TextPatternParser.StringComparison;
using Sid.Log;

namespace TextParseTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog log=null;
            
            var stmtList = new StatementList(log);

            var orStmt=new OrComparison(log);

            var options=new Options(log);
            options.CaseSensitive=false;

            var knownGreetings=new string[]{"hello","hi","hey","yo","hiya"};
            Array.ForEach(knownGreetings,(str) => orStmt.Add(new StringComparison(log,options,str)));
            stmtList.Add(orStmt);

            Console.WriteLine("Hello..");
            var userInput=Console.ReadLine();

            var parser=new Parser(log);
            int numMatches;
            parser.Extract(userInput,null,stmtList,null,null,out numMatches,(unused1,unused2,unused3)=>null,null);

            Console.WriteLine("Greeting {0}understood.",((numMatches>=1)?"":"not "));
        }
    }
}
```

### Types / Interfaces / Classes ###

Below is the list of types e.t.c. that a user/developer may need to be concerned with when using the parser:

| Type e.t.c. | Description | Notes |
| ----------- | ----------- | ----- |
| ComparisonWithAdvanceBase | Helper base class for comparison type parse statements | |
| fOperand<T> | A delegate that has access to the input string, current position, and run state. The generic type parameter determines the resulting type | For example used to parameterise parse statements with dynamic values such as the offset in a string offset comparison |
| IAssertion | An interface that all assertions must implement | A given parse statement can have any number of assertions associated with it. These assertions are executed after a parse statement has finished. Used for identifying bugs |
| IComparison | An interface for comparison type parse statements which do not advance | |
| IComparisonWithAdvance | The interface that all comparison parse statements that advance must implement | Parse sub routines should return an instance of this interface |
| IOperation | The interface that all operation type parse statements must implement | |
| IStatement | The root interface for parse statements | Contains declarations for the members required in both comparison and operation sub categories |
| PositionAssertion | Assert for a particular parse statement what the expected output position and result should be in relation to the start position | Used for identifying the individual parse statement(s) that are not working in complex UCPA's that are giving unexpected results |
| RunState | Singleton class which holds the parse state | Is used to access user defined variables and functions |
| StatementBase | Root base class for parse statements | Contains the members required in both comparison and operation sub categories |

### Statement Types ###

Below is the current list of statement types with notes explaining their usage:

| Type | Parameter(s) | Description | Notes |
| ---- | ------------ | ----------- | ----- |
| Advance | Position (fOperand<int>) | Advance to the position denoted by the position operand |  |
| Advance If | Comparison (IComparisonWithAdvance) | Optionally advance if the comparison returns true |  |
| Advance to the End | | Advance to the end of the input text | Can be used to prematurely stop the OLPA for example to improve performance if only a single match is required |
| Advance Until Comparison | Comparison (IComparisonWithAdvance), forwards (bool) | Advance until the comparison returns true. | If the end of the input string is reached without the comparison matching, false is returned (no match) |
| Advance While Operation | Comparison (IComparisonWithAdvance) | Advance whilst the comparison returns true |  |
| Capture | Comparison (IComparsonWithAdvance) | Extract the text which matches 'comparison'. | Captured text is stored against a given name/identifier and is available when the UCPA reaches the end of a successful match. Same idea as the capture feature found in regex |
| Char Comparison | Chr (char) | Compare the character at the current position with 'Chr' | The options parameter specifies case sensitivity |
| Char Delegate Comparison | Char delegate (bool (char)) | Compare the character at the current position against a delegate taking that character as a parameter | |
| Compare No Advance | Comparison (IComparison) | Run a comparison but return the original position. I.e. validate but don't move forward | Can be used as a replacement for the look around feature found in regex |
| Custom Comparison | Custom comparison delegate (bool (int,string,RunState)) | Compare and advance according to a user specified delegate | Only use if an existing statement or sub routine does not achieve your goal and it doesn't make sense to reuse it in the future |
| Delimited List Comparison | Comparison (IComparisonWithAdvance), seperator (IComparisonWithAdvance) | Parse a delimited list where values are compared against 'Comparison' and are delimited by the 'seperator' comparison | It's possible to specify a minimum and maximum number of items expected |
| Match Everything Comparison | | Return the current position plus 1 | This is the same as using the advance operation with a forward value of 1. May be useful in situations where this name describes the algorithm more intuitively |
| Nested Open Close Comparison | Open (IComparisonWithAdvance), close (IComparisonWithAdvance) | This is quite niche. Matches against the open comparison and advances until the close comparison is found. The number of 'close' comparisons must match the number of 'open' comparisons in order to stop - hence 'nested' | For example can be used to validate function calls in text by using '(' as the open comparison and ')' as the close comparison |
| Not Comparison | Comparison (IComparison) | Run a comparison but invert the result | This does not advance |
| Or Comparison | Comparison list (IList<IComparisonWithAdvance>) | Match one of a list of comparisons, simulates an 'or' in programming terms | The comparisons are executed from the first item (index 0) upwards and it is the first matching comparison that controls how far to advance |
| Set Log Level | Level (int) | Update the log level | Can be used to increase/decrease logging verbosity |
| Set Variable | Variable name (string), value (fOperand<int>) | Assign the value to a user defined variable | |
| Start of Input String Comparison | | Returns true if the current position is the beginning of the input text | |
| Statement List | List<IStatement> | Execute a list of parse statements until the end is reached or a comparison returns no match |  |
| Store Position as Variable | Variable name (string) | Store the current position as a variable | For example if you need to refer back to this position at a later date |
| String Comparison | Compare string, options | Compare the string at the current position in the input text against the compare string | The options parameter specifies case sensitivity |
| String Comparison Skip Whitespace | Str (string) | Compare the string at the current position in the input text against 'Str' but ignore any differences in whitespace | This is not necessary as it's own statement type. A sub routine that composes the other statement types could perform the same job |
| String Offset Comparison | Length (fOperand<int>), offset (fOperand<int>), reverse (bool) | Compare the string denoted by the current position in the input text and length parameter against another part of the input string denoted by the offset parameter | Used by the palindrome built in examples |
| Toggle Log Status | Enable (bool) | Enable or disable the log | |

# Text Parse User Interface Application #

I have created a live ASP .NET MVC Core 3 and React web application that is hosted within Azure which demonstrates how the Text Parse library works as well as my skills as a full stack developer: http://chrissiddall.azurewebsites.net/textparse. 
It can be used to create and execute a UCPA visually as opposed to through lines of code and may give a different perspective and deeper understanding.

The code for the entire application can be found at https://github.com/sidfishus/react-spa-demo.

## Usage ##

For each step, a cross or tick is displayed against the relevant section to denote whether all required information has been entered.

1. Create user defined functions and variables if any are required.
1. Create the individual parse statements and arrange them in to the correct order.
1. Enter the parse text in the input section.
1. Select which type of parse routine you wish to use in the parse output section.
1. Press the execute parse button, it will be green or red depending on whether all required information has been entered.
1. The parse result(s) will display in the output section.
1. The .NET/C# application created will be printed to the web browser console.

## Technical ##

All of the information entered by the user for parsing is stored as functions/objects in the browser by React/Javascript/Babel. Upon parse execution in the front end all of this information is converted to the .NET/C# code necessary to execute the parse routine, and HTTP POST'd as a string to the ASP .NET MVC Core controller which handles parsing. The controller, utilising the Rosyln open source compiler assemblies (Microsoft.CodeAnalysis.CSharp), compiles and then executes the C# code which was passed to it, and returns the results of the parse back to the web application. Sending C# code via a HTTP message which is then compiled and executed may seem a security risk, but it is safe to do so in this case because only the assemblies required to run the parse application are referenced in the compiled executable. Also the task of compiling and executing the text parse application is encapsulated in to it's own thread and is given a timeout value. This protects against infinite loops which could cause the HTTP request to crash.

## Features ##

1. The majority of the parse statement types are available for use.
1. User defined functions.
1. User defined variables.
1. A variety of parse algorithms:
   1. Match: display whether the input matches the parse statements.
   1. Extract single: extract and display the first item which matches the parse statements.
   1. Extract all: extract and display all items which match the parse statements.
   1. Replace: replace entries matching the parse statement list according to the replace format and retain any text which does not match
1. 7 fully working built in examples including a complex example which converts all classic ASP procedure calls to the ASP .NET equivalent.
1. The full C# code of the parse application produced is logged to the browser console window.

See https://chrissiddall.azurewebsites.net/portfolio/textparse for a slideshow of images demonstrating the features available in the user interface.

# Notes

1. In order to build this application you will need to replace the clear text password inside the nuget.config file with 'b0aa87c9b44616466778a5379a3dcaf0212dc28b' otherwise the 'dotnet restore' command will fail. If I include the clear text password (PAT) in the released/pushed (to Github) nuget.config file the PAT is automatically deleted. If for whatever reason the PAT I have given above stops working feel free to send me an email at sidnet001@gmail.com to request a new one.


