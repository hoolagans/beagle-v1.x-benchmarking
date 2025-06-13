using System;
using System.Collections.Generic;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.DataContext;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.Exceptions;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Views;

public abstract class EnhancedCRUDMvcView<TMvcModel, TSearchMvcModel, TDataContext> : EnhancedCRUDMvcView<TMvcModel, TMvcModel, TSearchMvcModel, TDataContext>
    where TMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new();

public abstract class EnhancedCRUDMvcView<TDetailMvcModel, TListMvcModel, TSearchMvcModel, TDataContext> : CRUDMvcView<TDetailMvcModel, TListMvcModel, TDataContext>, IEnhancedCRUDMvcView<TDetailMvcModel, TListMvcModel, TSearchMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TSearchMvcModel : class, IMvcModel, new()
    where TDataContext : class, IDataContext, new()
{
    #region View Methods
    public virtual IGenerateHtml RenderSearch(TSearchMvcModel model)
    {
        if (!IncludeSearchViewMethod) throw new Exception404PageNotFound();
            
        var result = new HtmlStack();

        result.Append(RenderFilter(model));

        return ApplyToDefaultLayout(result);
    }
    public virtual IGenerateHtml RenderList(ListWithCriteria<TListMvcModel, TSearchMvcModel> models, int totalCount)
    {
        var result = new HtmlStack();

        if (ShowFilterOnList) result.Append(RenderFilter(models.Criteria));

        if (ListPageTitle != null) result.Append(new H2 { new Txt(ListPageTitle) });
            
        if (PaginationMode == PaginationMode.Top || PaginationMode == PaginationMode.TopAndBottom) result.Append(new Bs4.Pagination(totalCount));
        switch (ListMode)
        {
            case ListMode.NoList:
            {
                throw new InvalidOperationException("List is not valid for NoList ListModel");
            }
            case ListMode.Simple:
            {
                result.Append(new Bs4.CRUDList(models, (IGenerateHtml?)null, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
                break;
            }
            case ListMode.MultiColumn:
            {
                result.Append(new Bs4.CRUDMultiColumnList(models, (IGenerateHtml?)null, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
                break;
            }
            case ListMode.MultiColumnNoActions:
            {
                result.Append(new Bs4.CRUDMultiColumnListNoActions(models));
                break;
            }
            case ListMode.EditableMultiColumn:
            {
                if (ReadOnlyView) throw new InvalidOperationException("ReadOnlyView is not compatible with EditableMultiColumn ListModel");
                result.Append(new Bs4.CRUDMultiColumnEditableList(models, typeof(TDataContext), (IGenerateHtml?)null, ListSkipAddNew, ListSkipDelete));
                break;
            }
            default:
            {
                throw new SupermodelException($"Unknown ListMode: {ListMode}");
            }
        }
        if (PaginationMode == PaginationMode.Bottom || PaginationMode == PaginationMode.TopAndBottom) result.Append(new Bs4.Pagination(totalCount));

        if (ShowTotalRecords) result.Append(new P { new Txt($"Total Records: {totalCount}")});

        return ApplyToDefaultLayout(result);
    }

    protected virtual IGenerateHtml RenderFilter(TSearchMvcModel model)
    {
        var result = new HtmlStack();

        if (FilterTitle != null) result.Append(new Bs4.CRUDSearchForm(model, FilterTitle, resetButton: ResetButtonOnFilter));
        else result.Append(new Bs4.CRUDSearchForm(model, resetButton: ResetButtonOnFilter));

        return result;
    }
    #endregion

    #region Disabled View Methods
    public override IGenerateHtml RenderList(List<TListMvcModel> models) { throw new InvalidOperationException("Must call this overload: RenderList(ListWithCriteria<TListMvcModel, TSearchMvcModel> models, int totalCount)"); }
    #endregion

    #region Overrides
    protected virtual PaginationMode PaginationMode { get; } = PaginationMode.TopAndBottom;
    protected virtual bool ShowFilterOnList { get; } = typeof(TSearchMvcModel) != typeof(Bs4.DummySearchMvcModel);
    protected virtual bool IncludeSearchViewMethod { get; } = false;
    protected virtual bool ResetButtonOnFilter { get; } = false;
    protected virtual string? FilterTitle { get; } = null;
    protected virtual bool ShowTotalRecords { get; } = true;
    #endregion
}