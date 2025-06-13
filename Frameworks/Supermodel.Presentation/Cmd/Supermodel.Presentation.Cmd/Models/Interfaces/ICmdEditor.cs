namespace Supermodel.Presentation.Cmd.Models.Interfaces;

public interface ICmdEditor
{
    object? Edit(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue); 
}