using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supermodel.Mobile.Runtime.Common.Models;
using Supermodel.ReflectionMapper;

namespace Supermodel.Mobile.Runtime.Common.XForms.ViewModels;

public class ListViewModel<TViewModelForEntity> : ListViewModel<TViewModelForEntity, TViewModelForEntity> where TViewModelForEntity : class, IHaveIdentity, new(){}
    
public class ListViewModel<TViewModelForEntity, TModel> : List<TViewModelForEntity>, IRMapperCustom
    where TViewModelForEntity : class, IHaveIdentity, new()
    where TModel : class, IHaveIdentity, new()
{
    #region IRMapperCustom implemtation
    public async Task MapFromCustomAsync<T>(T other)
    {
        Clear();
        var modelList = (ICollection<TModel>)other;
        if (modelList == null) throw new ArgumentNullException(nameof(other));
        foreach (var model in modelList.ToList())
        {
            var viewModel = await new TViewModelForEntity().MapFromAsync(model);
            Add(viewModel);
        }
    }
    public async Task<T> MapToCustomAsync<T>(T other)
    {
        var modelList = (ICollection<TModel>)other;
        if (modelList == null) throw new ArgumentNullException(nameof(other));

        //Add or Update
        foreach (var viewModel in this)
        {
            var modelMatch = modelList.SingleOrDefault(x => x.Identity == viewModel.Identity);
            if (modelMatch != null)
            {
                await viewModel.MapToAsync(modelMatch);
            }
            else
            {
                var newModel = await viewModel.MapToAsync(new TModel());
                modelList.Add(newModel);
            }
        }

        //Delete
        foreach (var model in modelList.ToList())
        {
            if (this.All(x => x.Identity != model.Identity)) modelList.Remove(model);
        }

        return (T)modelList;        
    }
    #endregion
}