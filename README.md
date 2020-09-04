# TextParse

TextParse is a .NET C# library I created for parsing and replacing text. //sidtodo

# Concept
## Background
I was tasked with a project to convert a large ERP web application from classic ASP to ASP .NET because support from Microsoft was soon to expire. The application consisted of 13 modules including 100's of sourcecode files and after making the changes necessary to convert a single file it was clear that an automated process was needed.

Classic ASP uses the VB script language for dynamic rendering whereas ASP .NET uses VB .NET. After manually converting a couple of code files to ASP .NET there seemed to be approximately 20 syntactical differences between the languages. Effectively what I was looking to achieve was to create a transpiler that converts from VB script to VB .NET. 
I fathomed that if each 'type' of syntax conversion would take 1-3 days to complete it would be an acceptable amount of time to automate the conversion of the entire application.

I started my investigation by trying regular expressions which I've used to do simple parsing in the past. I quickly gave up on this idea because it was clear I would have to become the master of master's in order to achieve what I wanted in regex. I also did not appreciate the unintuitive syntax, lack of readability, as well as the inability to extend or add diagnostic tools or gain any kind of feedback. I also considered using other existing parsing tools but shied away from this approach because I did not want to spend time learning somebody else's work only to be frustrated with it's useage and find it only does 90% of what I needed. If I created my own however I would be completely in control and I could design it from the ground up to do exactly what I need.

My process in theory was to:
1. Choose a module and run the conversion against it.
2. Test that module until a bug/syntax error was found.
3. Fix the parse operation that deals with converting that particular syntax, or create a new operation.
4. Goto step 1 until no more bugs are found.
5. Move on to the next module but include the module(s) already converted.

After each execution of the conversion I could use Microsoft Team Foundation Studio to 'diff' the changed files for changes before checking them in. This would serve as a method of regression testing and unit testing to ensure that any new changes to the library and conversion operations made in that iteration had not broken anything, so I was never going two steps forward and then one step back. The annotate ('blame' in Git) feature would come in handy when errors were found to determine at what iteration a breaking change was introduced and therefore give me a starting point for fixing the issue.

## Theory

### User Created Parse Algorithms (UCPA)

I figured that it should be possible be able to parse and replace/convert anything providing I could describe the routine as a series of steps and checks. E.g. to match against words that contain only a series of lowercase a-z characters you could describe the algorithm in psuedo as follows:
1. Validate that we are at the beginning of the input text, or the preceding character is whitespace. ```// Validate beginning of word``` 
2. Validate that the character at the current position is lowercase and alphabetical. ```// Validate word is at least one character in length```
3. Move until a non lowercase a-z character is found, or we find the end of the string. ```// Find the first non a-z character```
4. Validate that we have reached the end of the text, or the current character is a space. ```// Reached the end of the word```

When described in this manner it is very easy to understand the intention and purpose of each step as well as the algorithm as a whole - at least it seems this way to me as someone who has been programming since 2001. Furthermore, if the four steps are encapsulated into their own sub routine it can be reused in future. This will reduce bugs (it's already tested) and increase the readability of parse algorithms by making them more terse (remove duplication by turning 4 steps in to 1) as long as an apt name and description is used. It's also trivial to create unit tests to prove the accuracy of the routine as well as provide regression testing as the parse library evolves over time.

### Outer Linear Parse Algorithm (OLPA)

The next thing to consider is how the UCPA should be executed, and therefore there are 2 algorithms in play:

1. The OLPA which executes in a linear fashion starting at the first character and stops when the end of the input text is found. At each iteration the UCPA is executed from the OLPA's current position. If an iteration results in an unsuccessful match from the UCPA the OLPA will incremement it's last recorded position by 1 and continue iterating from there. However if the iteration results in a successful match from the UCPA, the OLPA records the position of where the UCPA finishes and continues iterating from the new position.
2. The UCPA starts at the position passed to it by the OLPA and executes parse statements in a sequential manner until a validation returns false or there are no more parse statements. Each time a parse statement is executed the resulting position is passed on to the next parse statement in the sequence. The UCPA can move anywhere in the input string.

### Individual Statements

Each individual parse statement would be an instance of a given type of operation that would be passed the same parameters:
1. The input text that is being parsed.
2. The position within the input text to begin at.
3. The output position within the input text, or a value to indicate stop/unsuccessful match.

New statements could be added over time as/when needed to further extend the parse library and make it as reusable as possible. I.e. there would be a built in statement type for a string comparison, and a statement type that would act as the 'OR' operator, for example. And it should be possible to combine statements together to increase their reuse.

## Practical

### Initial Approach ###

Now I could describe what I wanted to achieve, I needed a way of conveying that to an application so that it could be translated and executed but at the same time remain human readable to a computer programmer.

My first idea was to create a psuedo language that could be parsed and converted to a list of parse statement objects (I was heavily into OO at the time) that would make up the parse statement list (UCPA) that would be passed to the OLPA along with the input text. The parse statement classes would all derive from a common interface (IComparisonWithAdvance) and leverage polymorphism to allow them to be called via a reference to the interface. This is key because it allows any type, combination, or list of parse statements to be used anywhere that a single parse statement is required because everything that can be executed as part of a UCPA derives from the same interface and implements the same parse interface method. For example this is very useful for the 'or' statement which takes a list of parse statements and executes them sequentially until a match is found because it makes the following algorithm possible:
1. Compare against string 'test' (string comparison)
1. OR
   1. Compare against string 'hello'
   1. AND THEN Compare against a single character in the sequence 1, 2, or 3.

The second parse statement of the 'or' is a combination of statements and because the parse statement list class ('StatementList') derives from the same interface as an individual parse statement (IComparsonWithAdvance), the parse statement list type can be used anywhere a parse statement is required. Thus removes the need for the caller to have any knowledge of how the parse statement is implemented. The only contract between the 'or' statement and it's child parse statements is that the child will be given the input text and a starting position and will return an output position alongside a result indicating whether it was a successful match.

The statement types are of one of 2 categories:
- Comparison: validate from the given position, return the output position, and return true false to indicate whether the match was successful.
- Operation: do something and return the output position. For example this could be setting a user defined variable where the output position will be returned as the input position, or moving to elsewhere within the input text and returning that position.

The only difference in terms of syntax/execution is that an operation cannot cause the parsing to stop. I've since concluded that this distinction of categories is not necessary and the only result necessary is the output position which could be returned as -1 to indicate a failed match.

### Programming Language/Platform ###

I had already decided that I would be designing this using an object oriented approach. So I decided to create the library in .NET and C# due to the plethora of inbuilt functionality and intuitive OO syntax. Having a gargage collector would also reduce the complexity allowing me to focus on making the parser as accurate and resilient as possible. I was focusing on accuracy and human readability as opposed to performance, however after parsing entire directories containing 100's of files I am more than satisfied with the speed taken.

### Progress ###

I quickly gave up on the idea of creating a psuedo language because it was taking me too long to parse it and was too verbose and difficult to follow when describing complex parse algorithm's. However the C# class syntax I was already using to create the parse statements as a result of parsing the psuedo language seemed appropriate for the task and had the added benefit of removing the need for an intermediate language.

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

Below is the list of existing types e.t.c. that a user/developer may need to be concerned with when using the parser:

| Type e.t.c. | Description | Notes |
| ----------- | ----------- | ----- |
| ComparisonWithAdvanceBase | Helper base class for comparison parse statements that advance | |
| fOperand<T> | A delegate that has access to the input string, current position, and run state. The generic type parameter determines the resulting type | For example used to parameterise parse statements with dynamic values |
| IAssertion | An interface that all assertions must implement | A given parse statement can have any number of assertions associated with it. These assertions are executed after a parse statement has finished. Used for identifying bugs |
| IComparison | An interface for comparison type parse statements which do not advance | |
| IComparisonWithAdvance | The interface that all comparison parse statements that advance must implement | Sub routines should return an instance of this interface |
| IOperation | The interface that all operation type parse statements must implement | |
| IStatement | The root interface for parse statements | Contains declarations for the members required in both comparison and operator sub categories |
| Position Assertion | Assert for a particular parse statement what the expected output position and result should be in relation to the start position | Used for identifying the individual parse statement(s) that are not working in complex UCPA's that are giving unexpected results |
| RunState | Singleton class which holds the parse state | Is used to access user defined variables and functions |
| StatementBase | Root base class for parse statements | Contains the members required in both comparison and operator sub categories |

### Statement Types ###

Below is the current list of statement types with notes explaining their useage:

| Type | Parameter(s) | Description | Notes |
| ---- | ------------ | ----------- | ----- |
| Advance | Position (fOperand<int>) | Advance to the position denoted by the position operand |  |
| Advance If | Comparison (IComparisonWithAdvance) | Optionally advance if the comparison returns true |  |
| Advance to the End | | Advance to the end of the input text | Can be used to prematurely stop the OLPA for example to improve performance if only a single match is required 
| Advance Until Comparison | Comparison (IComparisonWithAdvance), forwards (bool) | Advance until the comparison returns true. | If the end of the input string is reached without the comparison matching, false is returned (no match) |
| Advance While Operation | Comparison (IComparisonWithAdvance) | Advance whilst the comparison returns true |  |
| Capture | Comparison (IComparsonWithAdvance) | Extract the text which matches 'comparison'. | Captured text is stored against a given name/identifier and is available when the UCPA reaches the end of a successful match |
| Char Comparison | Chr (char) | Compare the character at the current position with 'Chr' | The options parameter specifies case sensitivity |
| Char Delegate Comparison | Char delegate (bool (char)) | |
| Compare No Advance | Comparison (IComparison) | Run a comparison but return the original position. I.e. validate but don't move forward | Can be used as a look around
| Custom Comparison | Custom comparison delegate (bool (int,string,RunState)) | Compare and advance according to a user specified delegate | Only use if an existing statement or sub routine does not achieve your goal and it doesn't make sense to reuse this in the future |
| Delimited List Comparison | Comparison (IComparisonWithAdvance), seperator (IComparisonWithAdvance) | Parse a delimited list where values are compared against 'Comparison' and are delimited by the 'seperator' comparison | It's possible to specify a minimum and maximum number of items expected |
| Match Everything Comparison | | Return the current position plus 1 | This is the same as using the advance operation with a forward value of 1. May be useful in situations where this name describes the algorithm more concisely |
| Nested Open Close Comparison | Open (IComparisonWithAdvance), close (IComparisonWithAdvance) | This is quite niche. Matchs against the open comparison and advances until the close comparison is found. The number of 'close' comparisons must match the number of 'open' comparisons in order for it to stop - hence the 'nested' aspect of the name | For example can be used to parse function arguments by using '(' as the open comparison and ')' as the close comparison |
| Not Comparison | Comparison (IComparison) | Run a comparison but invert the result | This does not advance |
| Or Comparison | Comparison list (IList<IComparisonWithAdvance) | Match one of a list of comparisons, simulates an 'or' in programming terms | The comparisons are executed from the first item (index 0) upwards and it is the first matching comparison that controls how far to advance. |
| Set Log Level | Level (int) | Update the log level | Can be used to increase/decrease logging verbosity |
| Set Variable | Variable name (string), value (fOperand<int>) | Assign the value to a user defined variable | |
| Start of Input String Comparison | | Returns true if the current position is the beginning of the input text | |
| Statement List | List<IStatement> | Execute a list of parse statements until the end is reached or a comparison returns no match |  |
| Store Position as Variable | Variable name (string) | Store the current position as a variable | For example if you need to refer back to this position at a later date |
| String Comparison | Compare string | Compare the string at the current position in the input text against the compare string | The options parameter specifies case sensitivity |
| String Comparison Skip Whitespace | Str (string) | Compare the string at the current position in the input text against 'Str' but ignore any differences in whitespace | This is not necessary as it's own statement type. A sub routine that composes the other statement types could perform the same job |
| String Offset Comparison | Length (fOperand<int>), offset (fOperand<int>), reverse (bool) | Compare the string denoted by the current position in the input text and length parameter against another part of the input string denoted by the offset parameter | Used by the palindrome built in examples |
| Toggle Log Status | Enable (bool) | Enable or disable the log | |

# Text Parse User Interface Application #

I have created a live React web application that is hosted within Azure which demonstrates how the Text Parse library works: http://chrissiddall.azurewebsites.net/textparse. 
It can be used to create and execute a UCPA visually as opposed to through lines of code and may give a different perspective for a deeper understanding.

The code for the entire application can be found at https://github.com/sidfishus/react-spa-demo.

## Useage ##

For each step, a cross or tick is displayed against the relevant sections to denote whether all required information has been entered.

1. Create user defined functions and variables if any are required.
1. Create the individual parse statements and arrange them in to the correct order.
1. Enter the parse text in the input section.
1. Select which type of parse routine you wish to use in the parse output section.
1. Press the execute parse button, it will be green or red depending on whether all required information has been entered.
1. The parse result will display in the output section.
1. The .NET/C# code equivalent of the parse statements will also be printed to the web browser console.

## Technical ##

All of the information entered by the user for parsing is stored as functions/objects in the browser by React/Javascript. Upon parse execution in the front end all of this information is then converted to the .NET/C# code necessary to execute the parse routine, and then HTTP POST'd to the ASP .NET MVC Core controller which handles parsing as a string. The controller, utilising the Rosyln open source compiler (Microsoft.CodeAnalysis.CSharp assembly), compiles and then executes the C# code that was passed to it, and returns the results back to the web application. Sending C# code via a HTTP message which is then compiled and executed may seem a security risk, but it is safe to do so because only the assemblies required to run the parse application are referenced in the compiled application and thus only basic functionality from the .NET platform could be used. Also the task of compiling and executing the text parse application is encapsulated in to it's own thread which has a timeout.

## Features ##

1. The majority of the parse statement types.
1. User defined functions.
1. User defined variables.
1. Different parse algorithms:
   1. Match: display whether the input matches the parse statements.
   1. Extract single: extract and display the first item which matches the parse statements.
   1. Extract all: extract and display all items which matches the parse statements.
   1. Replace: replace entries matching the parse statement list according to the replace format and retain any text which does not match
1. 7 fully working built in examples.
1. The full C# code to the parse application produced is logged to the browser console window to help with identifying bugs in the library or the UCPA.

## Images ##

See https://chrissiddall.azurewebsites.net/portfolio/textparse for a slideshow of images.





... incomp

notes:


since used it to parse HTML and XML.


project wasn't finished.

unit tests

NOTE: REPLACE THE GITHUB CLEAR TEXT PASSWORD IN NUGET.CONFIG WITH 'b0aa87c9b44616466778a5379a3dcaf0212dc28b' otherwise this will not work. issues with pushing that live.
