using DipApp.common;

namespace DipApp.stubs;

public class RabbitMq
{
    public Task Enqueue(Email email)
    {
        return Task.CompletedTask;
    }
}