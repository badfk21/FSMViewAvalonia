using System.Collections.Concurrent;

namespace FSMViewAvalonia2.Context;
public abstract class GameIsolate<T>
{
    private readonly ConcurrentDictionary<GameId, T> games = [];

    public bool Exists(GameId id) => games.ContainsKey(id);
    public void Remove(GameId id) => games.Remove(id, out _);

    protected abstract T Create(GameId id);
    public T Get(GameId id)
    {
        if (!games.TryGetValue(id, out T result))
        {
            result = games.AddOrUpdate(id, Create, (_, orig) => orig);
        }

        return result;
    }
}
public class DefaultGameIsolate<T>(Func<GameId, T> factory) : GameIsolate<T>
{
    protected override T Create(GameId id)
    {
        return factory(id);
    }
}
