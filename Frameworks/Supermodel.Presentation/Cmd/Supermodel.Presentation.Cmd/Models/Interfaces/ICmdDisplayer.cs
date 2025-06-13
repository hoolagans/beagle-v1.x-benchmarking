namespace Supermodel.Presentation.Cmd.Models.Interfaces;

public interface ICmdDisplayer
{
    void Display(int screenOrderFrom = int.MinValue, int screenOrderTo = int.MaxValue);    
}