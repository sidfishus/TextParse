
namespace Sid.Parse.TextPatternParser
{
    public interface IAssertion {

        string Name {
            get;
            set;
        }

        // A return value of 'null' here means the assertion is not relevant for the parameters.
        // For example you may only want to run an assertion when the beginPos == n.
        bool? Assert(
            string input,
            int beginPos,
            int? afterPos,
            RunState runState,
            bool? matched,
            out string errorMsg
        );
    }
}