using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public static class Utils {

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source)
    {
        if (source == null)
        {
            throw new Exception();
        }

        IList<TSource> list = source as IList<TSource>;
        if (list != null)
        {
            if (list.Count > 0)
            {
                return list[0];
            }
        }
        else
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return enumerator.Current;
                }
            }
        }
        return default(TSource);
    }
}
