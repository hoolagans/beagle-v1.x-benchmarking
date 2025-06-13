using System;
using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class CodeBlock : IGenerateHtml
{
    #region Constructors
    public CodeBlock(Func<IGenerateHtml> code)
    {
        Html = code();
    }
    #endregion

    #region IGenerateHtml implementation
    public StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        return Html.ToHtml(sb);
    }
    public IGenerateHtml FillBodySectionWith(IGenerateHtml replacement)
    {
        return Html.FillBodySectionWith(replacement);
    }
    public IGenerateHtml FillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        return Html.FillSectionWith(sectionId, replacement);
    }
    public bool TryFillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        return Html.TryFillSectionWith(sectionId, replacement);
    }

    public IGenerateHtml DisableAllControls()
    {
        Html.DisableAllControls();
        return this;
    }
    public IGenerateHtml DisableAllControlsIf(bool condition)
    {
        Html.DisableAllControlsIf(condition);
        return this;
    }

    public IEnumerable<Tag> GetTagsInOrder()
    {
        return Html.GetTagsInOrder();
    }
    public virtual List<Tag> NormalizeAndFlatten()
    {
        return Html.NormalizeAndFlatten();
    }
    #endregion

    #region Linq-like methods
    public virtual int CountWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().Count(predicate);
    }
    public virtual IEnumerable<Tag> Where(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().Where(predicate);
    }
    public virtual IEnumerable<Tag?> ParentsOfTagsWhere(Func<Tag, bool> predicate)
    {
        NormalizeAndFlatten();
        return GetTagsInOrder().Where(predicate).Select(x => x._parent);
    }

    public virtual Tag SingleWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().Single(predicate);
    }
    public virtual Tag? SingleOrDefaultWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().SingleOrDefault(predicate);
    }

    public virtual Tag FirstWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().First(predicate);
    }
    public virtual Tag? FirstOrDefaultWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().FirstOrDefault(predicate);
    }

    public virtual bool AnyWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().Any(predicate);
    }
    public virtual bool AllWhere(Func<Tag, bool> predicate)
    {
        return GetTagsInOrder().All(predicate);
    }

    public virtual int RemoveWhere(Func<Tag, bool> predicate)
    {
        return Html.RemoveWhere(predicate);
    }
    public virtual int RemoveFirstWhere(Func<Tag, bool> predicate)
    {
        return Html.RemoveFirstWhere(predicate);
    }

    public virtual int InsertBeforeWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return Html.InsertBeforeWhere(predicate, tags);
    }
    public virtual int InsertAfterWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return Html.InsertAfterWhere(predicate, tags);
    }

    public virtual int InsertBeforeFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return Html.InsertBeforeFirstWhere(predicate, tags);
    }
    public virtual int InsertAfterFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return Html.InsertAfterFirstWhere(predicate, tags);
    }
    #endregion

    #region Properties
    public IGenerateHtml Html { get; set; }
    #endregion
}