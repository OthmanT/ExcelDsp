namespace ExcelDsp.Painter.Utility;

internal record struct Range(int Min, int Max)
{
    public static Range Create(int a, int b)
    {
        if(a < b)
            return new Range(a, b);
        else
            return new Range(b, a);
    }
}