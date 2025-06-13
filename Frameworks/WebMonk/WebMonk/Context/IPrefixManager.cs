using System;

namespace WebMonk.Context;

public interface IPrefixManager
{
    IDisposable NewPrefix(string prefix, object? parent, string? controllerName = null);
    string CurrentPrefix { get; }
    object? CurrentParent { get; }
    string CurrentContextControllerName { get; }
    object? RootParent { get; }
}