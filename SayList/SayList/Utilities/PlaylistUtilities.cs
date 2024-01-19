using SayList.Things;

namespace SayList.Utilities;

public static class PlaylistUtilities
{
    public static T Pop<T>(this List<T> list)
    {
        T popped = list[^1];
        list.RemoveAt(list.Count-1);
        return popped;
    }
}