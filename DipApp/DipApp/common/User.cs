namespace DipApp.common;

public readonly struct User(Guid id, string name, string emailAddress)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
    public string EmailAddress { get; } = emailAddress;
}