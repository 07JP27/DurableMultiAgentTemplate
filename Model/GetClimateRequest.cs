using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class GetClimateRequest
{
    [Description("�ꏊ�̖��O�B��: �{�X�g��, �����A�t�����X")]
    public required string Location { get; set; } = string.Empty;
}
