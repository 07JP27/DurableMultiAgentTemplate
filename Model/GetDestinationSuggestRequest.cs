using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class GetDestinationSuggestRequest
{
    [Description("�s����ɋ��߂��]�̏���")]
    public required string SearchTerm { get; set; } = string.Empty;
}