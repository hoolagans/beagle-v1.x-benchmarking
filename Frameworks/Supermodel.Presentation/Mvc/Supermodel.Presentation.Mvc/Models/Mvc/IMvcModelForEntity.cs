namespace Supermodel.Presentation.Mvc.Models.Mvc;

public interface IMvcModelForEntity : IMvcModel, IViewModelForEntity
{
    string Label { get; }
    bool IsDisabled { get; }
}