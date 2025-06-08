using System;

namespace _5unSystem.Model.Shared;

public class Result<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
}

// Fixed the generic type declaration for ResultList
public class ResultList<T>
{
    public string? Message { get; set; }
    public List<T>? Data { get; set; }
    public int DataLength { get; set; }
}
