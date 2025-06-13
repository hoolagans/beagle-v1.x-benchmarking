using System;

namespace Supermodel.Presentation.Mvc;

[Flags]
public enum HttpMethod
{
    Get = 1,
    Post = 2,
    Put = 4,
    Delete = 8,
    Head = 16, // 0x00000010
    Patch = 32, // 0x00000020
    Options = 64, // 0x00000040
}