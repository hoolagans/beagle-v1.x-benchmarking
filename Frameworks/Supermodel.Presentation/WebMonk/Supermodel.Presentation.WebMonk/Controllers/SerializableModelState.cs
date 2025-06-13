using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Supermodel.DataAnnotations.Validations;
using WebMonk.Context;
using WebMonk.ValueProviders;

namespace Supermodel.Presentation.WebMonk.Controllers;

public class SerializableModelState
{
    #region EmbeddedTypes
    protected class TmpSerializableModelState
    {
        #region Properties
        public List<TmpValidationResult> ValidationResultList { get; set; } = new();
        public Dictionary<string, object> MessageBodyValueProviderDict { get; set; } = new();
        #endregion
    }

    [method: JsonConstructor]
    protected class TmpValidationResult(string errorMessage, IEnumerable<string> memberNames) : ValidationResult(errorMessage, memberNames);
    #endregion
            
    #region Constructors
    protected SerializableModelState(){ }
    protected SerializableModelState(TmpSerializableModelState tmp)
    {
        MessageBodyValueProviderDict = tmp.MessageBodyValueProviderDict;
        foreach (var tmpValidationResult in tmp.ValidationResultList) ValidationResultList.Add(new ValidationResult(tmpValidationResult.ErrorMessage, tmpValidationResult.MemberNames));
    }
    public static SerializableModelState CreateFromJson(string json)
    {
        var tmp = JsonConvert.DeserializeObject<TmpSerializableModelState>(json);
        return new SerializableModelState(tmp!);
    }
    public static async Task<SerializableModelState> CreateFromContextAsync()
    {
        var validationProviders = await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);
        // ReSharper disable once IdentifierTypo
        var mbvp = validationProviders.GetFirstOrDefaultValueProviderOfType<MessageBodyValueProvider>();
                
        //var dict = mbvp == null ? new Dictionary<string, object>() : mbvp.Values.ToDictionary(x => x.Key, x => x.Value);
        var dict = new Dictionary<string, object>();
        if (mbvp != null)
        {
            foreach(var value in mbvp.Values)
            {
                if (value.Value is IList<string> valueList)
                {
                    var first = true;
                    var composedValue = "";
                    foreach (var subValue in valueList)
                    {
                        if (first)
                        {
                            first = false;
                            composedValue += $"{subValue}";
                        }
                        else
                        {
                            composedValue += $",{subValue}";
                        }
                    }
                    dict.Add(value.Key, composedValue);
                }
                else
                {
                    dict.Add(value.Key, value.Value);
                }
            }
        }
                
        var vrl = HttpContext.Current.ValidationResultList;
                
        return new SerializableModelState { MessageBodyValueProviderDict = dict, ValidationResultList = vrl };
    }
    #endregion
            
    #region Methods
    public string SerializeToJson()
    {
        return JsonConvert.SerializeObject(this);
    }
    public async Task ReplaceInContextAsync()
    {
        var validationProviders = await HttpContext.Current.ValueProviderManager.GetValueProvidersListAsync().ConfigureAwait(false);
        var newMbvp = await new MessageBodyValueProvider().InitAsync(MessageBodyValueProviderDict).ConfigureAwait(false);
        validationProviders.ReplaceOrAppendValueProvider(newMbvp);
                
        HttpContext.Current.ValidationResultList.Clear();
        HttpContext.Current.ValidationResultList.AddValidationResultList(ValidationResultList);
    }
    #endregion
            
    #region Properties
    public ValidationResultList ValidationResultList { get; set; } = new();
    public Dictionary<string, object> MessageBodyValueProviderDict { get; set; } = new();
    #endregion
}