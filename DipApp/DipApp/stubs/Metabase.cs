using DipApp.common;

namespace DipApp.stubs;

public class Metabase
{
    public async Task<IEnumerable<User>> Users()
    {
        return await Task.Run(() => new List<User>());
    }

    public async Task<IEnumerable<Post>> Posts(Guid userId)
    {
        return await Task.Run(() => new List<Post>());
    }
}