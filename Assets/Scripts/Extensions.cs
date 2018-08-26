using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ListExtensions
{
    public static bool Exists<T>(this IList<T> list, Predicate<T> match, out T value)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (match.Invoke(list[i])) { value = list[i]; return true; }
        }
        value = default(T);
        return false;
    }
}
