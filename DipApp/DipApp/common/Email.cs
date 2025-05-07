namespace DipApp.common;

public readonly struct Email(string emailAddress, string msg)
{
    public string EmailAddress { get; } = emailAddress;
    public string Msg { get; } = msg;
}