namespace Supermodel.Presentation.WebMonk.Models.Mvc;

public interface IMvcModelForEntity : IViewModelForEntity, IMvcModel
{
    string Label { get; }
    bool IsDisabled { get; }
}