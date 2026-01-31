using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool IsValid(this IList list, int index) => index >= 0 && list.Count > index;

    public static bool TryGetEntry<T>(this IList<T> list, int index, out T output)
    {
        if (index >= 0 && list.Count > index)
        {
            output = list[index];
            return true;
        }

        output = default;
        return false;
    }
}
