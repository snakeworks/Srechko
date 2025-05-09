using System.Collections;

public static class ListHelpers
{
    public static bool InRange(this ICollection collection, int index) 
    {
        return collection.Count > 0 && index < collection.Count && index >= 0; 
    }
}
