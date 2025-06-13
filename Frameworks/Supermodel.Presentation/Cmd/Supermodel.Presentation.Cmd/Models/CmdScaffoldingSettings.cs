using System;
using Supermodel.Presentation.Cmd.ConsoleOutput;

namespace Supermodel.Presentation.Cmd.Models;

public static class CmdScaffoldingSettings
{
    public static ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        
    public static FBColors? Prompt { get; set; } = new FBColors(ConsoleColor.Cyan, BackgroundColor);
    public static FBColors? CommandValue { get; set; } = new FBColors(ConsoleColor.Magenta, BackgroundColor);
    public static FBColors? InvalidCommandMessage { get; set; } = new FBColors(ConsoleColor.Magenta, BackgroundColor);
    public static FBColors? Title { get; set; } = new FBColors(ConsoleColor.Cyan, BackgroundColor);

    public static FBColors? ListEntityId { get; set; } = new FBColors(ConsoleColor.Cyan, BackgroundColor);
    public static FBColors? DefaultListLabel { get; set; } = new FBColors(ConsoleColor.Yellow, BackgroundColor);
        
    public static FBColors? Label { get; set; } = new FBColors(ConsoleColor.White, BackgroundColor);
    public static FBColors? Value { get; set; } = new FBColors(ConsoleColor.Yellow, BackgroundColor);
    public static FBColors? Placeholder { get; set; } = new FBColors(ConsoleColor.DarkYellow, BackgroundColor);
    public static FBColors? DropdownArrow { get; set; } = new FBColors(ConsoleColor.Cyan, BackgroundColor);

    public static FBColors? InvalidValueMessage { get; set; } = new FBColors(ConsoleColor.Magenta, BackgroundColor);
    public static FBColors? ValidationErrorMessage { get; set; } = new FBColors(ConsoleColor.Magenta, BackgroundColor);

    public static StringWithColor RequiredMarker { get; set; } = new("*", ConsoleColor.Magenta, BackgroundColor);
}