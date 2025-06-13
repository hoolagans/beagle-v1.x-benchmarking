using Supermodel.Presentation.WebMonk.Models.Api;

namespace Supermodel.Presentation.WebMonk.Controllers.Api;

public class SimpleSearchApiModel : SearchApiModel
{
    public string SearchTerm { get; set; } = "";
}