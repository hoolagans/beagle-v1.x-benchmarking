using System;

namespace WebMonk.RazorSharp.Html2RazorSharp;

public class UserFacingMessageException : Exception
{
    internal UserFacingMessageException(string message) : base(message) { }
}