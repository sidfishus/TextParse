# TextParse
TextParse is a .NET C# library I created for parsing and replacing text. 

# Concept
I was tasked with a project to convert a large ERP web application from classic ASP to ASP .NET because support from Microsoft was soon to expire. The application consisted of 13 modules including 100's of sourcecode files and after making the changes necessary to convert a single file it was clear that an automated process was needed.

Classic ASP uses the VB script language for dynamic rendering whereas ASP .NET uses VB .NET. After manually converting a couple of code files to ASP .NET there seemed to be approximately 20 syntactical differences between the languages. I fathomed that if each 'type' of syntax change would take 1-3 days it would be an acceptable amount of time to automate the conversion of the entire application. My process in theory was to:
1. Choose a module and run the conversion against it.
2. Test that module until a bug/syntax error is found.
3. Fix the parse operation that deals with converting that particular syntax, or create a new operation.
4. Goto steps 1 until no more bugs are found.
5. Move on to the next module but including the module(s) already converted.

After each execution of the conversion I used Microsoft Team Foundation Studio to 'diff' the changed files for changes before checking them in. This would serve as a method of regression and unit testing to ensure that any new changes to the library and conversion operations made in that iteration had not broken anything, so I was never going two steps forward and then one step back. The annotate ('blame' in Git) feature would also come in handy to determine at what iteration a breaking change was introduced.

Effectively what I was looking to achieve was to create a transpiler that converts from VB script to VB .NET.

I began by trying regular expressions which I've used to do simple parsing in the past. I quickly gave up on this idea because it was clear I would have to become the master of master's in order to achieve what I wanted in regex. I also did not appreciate the unintuitive syntax, lack of readability, as well as the inability to extend or add diagnostic tools or gain any kind of feedback. I also considered using other existing parsing tools but shied away from this approach because I did not want to spend time learning somebody else's work only to be frustrated with it's useage and find it only does 90% of what I needed. If I created my own however I would be completely in control and I could design it from the ground up to do exactly what I need.

I figured that I should be able to parse anything providing I could describe

Probably more pertinent names for this library would be "Parse What You See", or "Linear Parser".

... incomp

notes:

2 days per operation, as well as extending the parser, and for further use.

since used it to parse HTML and XML.

 
idea would be to run the conversion. test. find errors. create new parse routine. re-run parse conversion, find the next type of error.

project wasn't finished.

human readable

linear

parse what you see. describe.

regex.

use parse statement to build more complex.

IComparisonWithAdvance base classes.

evolved.
