using System.IO;
using Newtonsoft.Json;
using Supermodel.ReflectionMapper;

namespace Supermodel.Presentation.Mvc.Models.Api;

public class BinaryFileApiModel : BinaryFileModelBase
{
    #region Overrides
    [JsonIgnore, NotRMapped] public override string Extension => Path.GetExtension(FileName);
    [JsonIgnore, NotRMapped] public override string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FileName);
    [JsonIgnore, NotRMapped] public override bool IsEmpty => string.IsNullOrEmpty(FileName) && BinaryContent.Length == 0;
    #endregion
}