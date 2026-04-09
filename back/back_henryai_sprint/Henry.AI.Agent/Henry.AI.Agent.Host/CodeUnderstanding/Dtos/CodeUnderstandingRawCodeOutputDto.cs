namespace Henry.AI.Agent.Host.CodeUnderstanding.Dtos;

public class CodeUnderstandingRawCodeOutputDto
{
    public string Namespace { get; set; } = string.Empty;
    public List<TClass> Classes { get; set; } = new();
    public List<TError> Errors { get; set; } = new();
    public bool OK { get; set; }
}