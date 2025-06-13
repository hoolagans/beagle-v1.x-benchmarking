using System;
using System.Collections.Generic;
using Supermodel.DataAnnotations.Exceptions;
using Supermodel.Persistence.DataContext;
using Supermodel.Presentation.WebMonk.Bootstrap4.Models;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.Presentation.WebMonk.Views;
using Supermodel.Presentation.WebMonk.Views.Interfaces;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

namespace Supermodel.Presentation.WebMonk.Bootstrap4.Views;

public abstract class CRUDMvcView<TMvcModel, TDataContext> : CRUDMvcView<TMvcModel, TMvcModel, TDataContext> 
    where TMvcModel : class, IMvcModelForEntity, new() 
    where TDataContext : class, IDataContext, new();

    
public abstract class CRUDMvcView<TDetailMvcModel, TListMvcModel, TDataContext> : CRUDMvcViewBase<TDetailMvcModel, TListMvcModel>
    where TDetailMvcModel : class, IMvcModelForEntity, new()
    where TListMvcModel : class, IMvcModelForEntity, new()
    where TDataContext : class, IDataContext, new()
{
    #region Views Methods
    public override IGenerateHtml RenderList(List<TListMvcModel> models)
    {
        switch (ListMode)
        {
            case ListMode.NoList:
            {
                throw new InvalidOperationException("List is not valid for NoList ListModel");
            }
            case ListMode.Simple:
            {
                if (ListPageTitle != null) return ApplyToDefaultLayout(new Bs4.CRUDList(models, ListPageTitle, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
                else return ApplyToDefaultLayout(new Bs4.CRUDList(models, (IGenerateHtml?)null, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
            }
            case ListMode.MultiColumn:
            {
                if (ListPageTitle != null) return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnList(models, ListPageTitle, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
                else return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnList(models, (IGenerateHtml?)null, ListSkipAddNew || ReadOnlyView, ListSkipDelete || ReadOnlyView, ReadOnlyView));
            }
            case ListMode.MultiColumnNoActions:
            {
                if (ListPageTitle != null) return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnListNoActions(models, ListPageTitle));
                else return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnListNoActions(models));
            }
            case ListMode.EditableMultiColumn:
            {
                if (ReadOnlyView) throw new InvalidOperationException("ReadOnlyView is not compatible with EditableMultiColumn ListModel");
                    
                if (ListPageTitle != null) return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnEditableList(models, typeof(TDataContext), ListPageTitle, ListSkipAddNew, ListSkipDelete));
                else return ApplyToDefaultLayout(new Bs4.CRUDMultiColumnEditableList(models, typeof(TDataContext), (IGenerateHtml?)null, ListSkipAddNew, ListSkipDelete));
            }
            default:
            {
                throw new SupermodelException($"Unknown ListMode: {ListMode}");
            }
        }
    }
    public override IGenerateHtml RenderDetail(TDetailMvcModel model)
    {
        if (ListMode == ListMode.EditableMultiColumn || ListMode == ListMode.MultiColumnNoActions) 
        {
            throw new InvalidOperationException("Detail is not valid for EditableMultiColumn or MultiColumnNoActions ListModels");
        }

        string? detailPageTitle;
        if (model.IsNewModel())
        {
            if (ShowDefaultCreatePageTitle) detailPageTitle = "Create New";
            else detailPageTitle = CreatePageTitle;
        }
        else
        {
            if (ShowDefaultEditPageTitle) detailPageTitle = model.Label;
            else detailPageTitle = EditPageTitle;
        }
            
        var accordionPanels = GetAccordionPanels(model);
            
        IGenerateHtml editTags;
        if (detailPageTitle != null)
        {
            if (accordionPanels == null) editTags = new Bs4.CRUDEdit(model, detailPageTitle, ReadOnlyView, ListMode == ListMode.NoList);
            else editTags = new Bs4.CRUDEditInAccordion(model, typeof(TDetailMvcModel).Name, accordionPanels, detailPageTitle, ReadOnlyView, ListMode == ListMode.NoList);
        }
        else
        {
            if (accordionPanels == null) editTags = new Bs4.CRUDEdit(model, (IGenerateHtml?)null, ReadOnlyView, ListMode == ListMode.NoList);
            else editTags = new Bs4.CRUDEditInAccordion(model, typeof(TDetailMvcModel).Name, accordionPanels, (IGenerateHtml?)null, ReadOnlyView, ListMode == ListMode.NoList);
        }
            
        var childrenTags = model.IsNewModel()? null : RenderChildren(model);

        if (childrenTags == null) return ApplyToDefaultLayout(editTags);
        else return ApplyToDefaultLayout(new Tags{ editTags, childrenTags });
    }
    #endregion

    #region Overrides
    protected virtual string? ListPageTitle { get; } = null;

    protected virtual bool ShowDefaultEditPageTitle { get; } = true;
    protected virtual string? EditPageTitle { get; } = null;

    protected virtual bool ShowDefaultCreatePageTitle { get; } = true;
    protected virtual string? CreatePageTitle { get; } = null;
        
    protected virtual bool ListSkipDelete { get; } = false;
    protected virtual bool ListSkipAddNew { get; } = false;

    protected virtual bool ReadOnlyView { get; } = false;

    //override this to get accordion
    protected virtual IEnumerable<Bs4.AccordionPanel>? GetAccordionPanels(TDetailMvcModel model)
    {
        return null; 
    }

    protected virtual IGenerateHtml? RenderChildren(TDetailMvcModel model) => null;
    #endregion
}