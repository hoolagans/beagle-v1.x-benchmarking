using System.Net;
using Newtonsoft.Json;

namespace WebMonk.Results;

public class JsonApiResult : TextResult
{
    #region Constructors
    public JsonApiResult(object apiModel, HttpStatusCode statusCode = HttpStatusCode.OK) : base(statusCode, JsonConvert.SerializeObject(apiModel), "application/json") { }
    #endregion
}