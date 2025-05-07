#nullable enable

namespace DipApp.common;

public readonly struct Post(Guid id, DateTime dateTime, string msg)
{
    public Guid Id { get; } = id;
    public DateTime DateTime { get; } = dateTime;
    public string Msg { get; } = msg;
}
