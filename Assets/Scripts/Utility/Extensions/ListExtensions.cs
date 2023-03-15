using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class ListExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOf<T>(this IList<T> list, T item) {
        return list.IndexOf(item);
    }

    // O(1)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool RemoveBySwap<T>(this IList<T> list, int index) {
        list[index] = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SafeCount<T>(this IList<T> list) {
        return list != null ? list.Count : 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PopLast<T>(this IList<T> list) {
        int count = list.SafeCount();

        if (count == 0)
            return default(T);

        var element = list[count - 1];
        list.RemoveAt(count - 1);
        return element;
    }
}
