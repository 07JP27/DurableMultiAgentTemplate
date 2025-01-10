using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class GetDestinationSuggestRequest
{
    [Description("s‚«æ‚É‹‚ß‚éŠó–]‚ÌğŒ")]
    public required string SearchTerm { get; set; } = string.Empty;
}