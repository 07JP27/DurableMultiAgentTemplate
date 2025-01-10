using System.ComponentModel;

namespace DurableMultiAgentTemplate.Model;

public class SubmitReservationRequest
{
    [Description("�s����̃z�e���̖��O�B")]
    public required string Destination { get; set; } = string.Empty;
    [Description("�`�F�b�N�C�����BYYYY/MM/DD�`���B")]
    public required string CheckIn { get; set; } = string.Empty;
    [Description("�`�F�b�N�A�E�g���BYYYY/MM/DD�`���B")]
    public required string CheckOut { get; set; } = string.Empty;
    [Description("�h���l���B")]
    public required int GuestsCount { get; set; }
}
