using System;

namespace ExcelDsp.Painter.Utility;
internal static class ResizableArray
{
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
