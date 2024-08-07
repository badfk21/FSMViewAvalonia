using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace FSMViewAvalonia2;
public struct GameId
{
    public string Id { get; private set; }
    public readonly bool IsNone => string.IsNullOrEmpty(Id);
    public readonly override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj is not GameId)
        {
            return false;
        }
        var gi = (GameId) obj;
        return gi.Id.Equals(Id);
    }
    public readonly override int GetHashCode() => Id.GetHashCode();
    public readonly override string ToString() => Id;


    private static string ProcessGameId(string id)
    {
        int i = 0;
        char[] bf = ArrayPool<char>.Shared.Rent(id.Length);
        for (int j = 0; j < id.Length; j++)
        {
            char c = id[j];
            if (c == '-' ||
               c == ' '
            )
            {
                continue;
            }
            if (c >= 'A' && c <= 'Z')
            {
                c = (char) (c - 'A' + 'a');
            }
            bf[i++] = c;
        }
        string result = new string(bf, 0, i);
        ArrayPool<char>.Shared.Return(bf);
        return result;
    }
    private static string GetGameIdFromPath(string gamePath)
    {
        string path = Path.GetDirectoryName(Path.GetFullPath(gamePath));
        if (string.IsNullOrEmpty(path) || !Directory.Exists(Path.Combine(path, "Managed"))) //Default as hk
        {
            return null;
        }

        string dataName = Path.GetFileName(path);
        if (!dataName.Contains("_Data", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return ProcessGameId(dataName);
    }
    public static GameId FromName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return default;
        }
        return new()
        {
            Id = ProcessGameId(name + "_data"),
        };
    }
    public static GameId FromPath(string path)
    {
        string id = GetGameIdFromPath(path);
        if (id == null)
        {
            string fp = Path.GetFullPath(path);
            foreach (Context.GameInfo v in GameFileHelper.allGameInfos)
            {
                string gp = GameFileHelper.FindGamePath(null, true, v).Result;
                if (string.IsNullOrEmpty(gp))
                {
                    continue;
                }
                if (fp.StartsWith(
                    Path.GetFullPath(gp), StringComparison.OrdinalIgnoreCase
                    ))
                {
                    return FromName(v.Name);
                }
            }
        }

        return new()
        {
            Id = id,
        };
    }
}
