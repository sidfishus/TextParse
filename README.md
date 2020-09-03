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

Now I could describe what I wanted to achieve, I needed a way of conveying that to an application so that it could be translated and executed but at the same time remain human readable to a computer programmer. My first idea was to create a psuedo language that could be parsed and converted to a list of parse statement objects (I was heavily into OO at the time) that would make up the parse statement list (UCPA) that would be passed to the OLPA along with the input text. The parse statement classes would all derive from a common interface and leverage polymorphism to allow them to be called via a reference to the interface and interface method. The statement types would be one of 2 categories:
- Comparison
- Operation



Note: <distinction not necessary, only need something that's given an input index and returns an output index or -1 to indicate fail/stop.>


not terse enough

provided the names of the statement types were descriptive and terse





C# class syntax.

Probably more pertinent names for this library would be "Parse What You See", or "Linear Parser".

... incomp

notes:

2 days per operation, as well as extending the parser, and for further use.

since used it to parse HTML and XML.

 
idea would be to run the conversion. test. find errors. create new parse routine. re-run parse conversion, find the next type of error.

project wasn't finished.

human readable

borrowed the idea of 'capturing' and look arounds (change the position via 'advance') from regex.

linear

parse what you see. describe.

use parse statement to build more complex.

IComparisonWithAdvance base classes.

evolved.

focus on parse accuracy rather than parse efficiency.

each individual set of 'statements' could be unit tested.

NOTE: REPLACE THE GITHUB CLEAR TEXT PASSWORD IN NUGET.CONFIG WITH 'b0aa87c9b44616466778a5379a3dcaf0212dc28b' otherwise this will not work. issues with pushing that live.
