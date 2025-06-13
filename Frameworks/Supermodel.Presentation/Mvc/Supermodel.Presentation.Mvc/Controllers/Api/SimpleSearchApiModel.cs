using Supermodel.Presentation.Mvc.Models.Api;

namespace Supermodel.Presentation.Mvc.Controllers.Api;

public class SimpleSearchApiModel : SearchApiModel
{
    public string SearchTerm { get; set; } = "";
}