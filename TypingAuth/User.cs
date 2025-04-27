namespace TypingAuth;

public class User(string name, int errors, long avgWordTypingSpeed)
{
    public string Name { get; set; } = name;
    public int Errors { get; set; } = errors;
    public long AvgWordTypingSpeed  { get; set; } = avgWordTypingSpeed;
}