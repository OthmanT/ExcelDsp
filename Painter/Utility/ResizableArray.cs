using System;

namespace ExcelDsp.Painter.Utility;

/// <summary>Helper for dynamically sized arrays</summary>
internal static class ResizableArray
{
    /// <summary>Add an array element, resizing the array if necessary</summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="array">Target array; receives new array if resized</param>
    /// <param name="index">Target index; incremented afterwards</param>
    /// <param name="item">Element to add</param>
    public static void AddItem<T>(ref T[] array, ref int index, T item)
    {
        int count = index + 1;
        if(array.Length < count)
        {
            int nextSize = array.Length * 2;
            nextSize = Math.Max(nextSize, count);
            Array.Resize(ref array, nextSize);
        }

        array[index] = item;
        index = count;
    }
}
