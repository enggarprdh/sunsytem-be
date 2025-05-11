using System;

namespace _5unSystem.Model.Shared;

public class Result<T>
{
    public string? Message { get; set; }
    public T? Data { get; set; }
}
