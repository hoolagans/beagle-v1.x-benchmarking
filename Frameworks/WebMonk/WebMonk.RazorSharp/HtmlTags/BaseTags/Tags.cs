using System;
using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations;
using WebMonk.RazorSharp.Exceptions;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class Tags : List<IGenerateHtml>, IGenerateAndContainHtml
{
    #region Overrides
    public virtual StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        sb ??= new StringBuilderWithIndents();
        foreach (var tag in this) sb = tag.ToHtml(sb);
        return sb;
    }
        
    public IGenerateHtml FillBodySectionWith(IGenerateHtml replacement)
    {
        FillSectionWith(BodySectionPlaceholder.BodySection, replacement);
        return this;
    }
    public IGenerateHtml FillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        if (!TryFillSectionWith(sectionId, replacement)) throw new RazorSharpException($"Unable to find placeholder with uniqueIdentifier = '{sectionId}'");
        return this;
    }
    public bool TryFillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        for(var i = 0; i < Count; i++)
        {
            if (this[i] is SectionPlaceholder placeholder)
            {
                if (placeholder.UniqueIdentifier == sectionId)
                {
                    this[i] = replacement;
                    return true;
                }
            }
            if (this[i].TryFillSectionWith(sectionId, replacement)) return true;
        }
        return false;        
    }

    public IGenerateHtml DisableAllControls()
    {
        foreach (var html in this) html.DisableAllControls();
        return this;
    }
    public IGenerateHtml DisableAllControlsIf(bool condition)
    {
        if (condition) DisableAllControls();
        return this;
    }

    public IEnumerable<Tag> GetTagsInOrder()
    {
        return this.SelectMany(x => x.GetTagsInOrder());
    }
    public virtual List<Tag> NormalizeAndFlatten()
    {
        var childrenList = new List<Tag>();
        foreach (var iGenerateHtml in this)
        {
            if (iGenerateHtml is Tag tag)
            {
                tag.NormalizeAndFlatten();
                childrenList.Add(tag);
            }
            else 
            {
                childrenList.AddRange(iGenerateHtml.NormalizeAndFlatten());
            }
        }

        Clear();
        AddRange(childrenList);

        return childrenList;
    }
    #endregion

    #region Methods
    public void Add(string txt)
    {
        Add(new Txt(txt));
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
        var removalCount = 0;
        var remainingChildren = new List<IGenerateHtml>();

        foreach (var iGenerateHtml in this)
        {
            if (iGenerateHtml is Tag tag)
            {
                if (predicate(tag)) removalCount++;
                else remainingChildren.Add(iGenerateHtml);
            }
            else
            {
                remainingChildren.Add(iGenerateHtml);
            }
        }

        foreach (var remainingChild in remainingChildren)
        {
            removalCount += remainingChild.RemoveWhere(predicate);
        }

        Clear();
        AddRange(remainingChildren);

        return removalCount;
    }
    public virtual int RemoveFirstWhere(Func<Tag, bool> predicate)
    {
        var removalCount = 0;
        var remainingChildren = new List<IGenerateHtml>();

        foreach (var iGenerateHtml in this)
        {
            if (iGenerateHtml is Tag tag)
            {
                if (predicate(tag) && removalCount == 0) removalCount++;
                else remainingChildren.Add(iGenerateHtml);
            }
            else
            {
                remainingChildren.Add(iGenerateHtml);
            }
        }

        if (removalCount == 0)
        {
            foreach (var remainingChild in remainingChildren)
            {
                removalCount += remainingChild.RemoveFirstWhere(predicate);
                if (removalCount > 0) break;
            }
        }

        Clear();
        AddRange(remainingChildren);

        return removalCount;
    }

    public virtual int InsertBeforeWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        var insertCount = 0;

        for (var i = 0; i < Count; i++)
        {
            var iGenerateHtml = this[i];
            if (iGenerateHtml is Tag tag && predicate(tag))
            {
                Insert(i, tags);
                i++;
                insertCount++;
            }
        }

        foreach (var child in this)
        {
            insertCount += child.InsertBeforeWhere(predicate, tags);
        }

        return insertCount;
    }
    public virtual int InsertAfterWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        var insertCount = 0;

        for (var i = 0; i < Count; i++)
        {
            var iGenerateHtml = this[i];
            if (iGenerateHtml is Tag tag && predicate(tag))
            {
                Insert(i + 1, tags);
                i++;
                insertCount++;
            }
        }

        foreach (var child in this)
        {
            insertCount += child.InsertAfterWhere(predicate, tags);
        }

        return insertCount;
    }

    public virtual int InsertBeforeFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        var insertCount = 0;

        for (var i = 0; i < Count; i++)
        {
            var iGenerateHtml = this[i];
            if (iGenerateHtml is Tag tag && predicate(tag))
            {
                Insert(i, tags);
                insertCount++;
                return insertCount; //1
            }
        }

        foreach (var child in this)
        {
            insertCount += child.InsertBeforeFirstWhere(predicate, tags);
            if (insertCount > 0) return insertCount; //1
        }

        return insertCount; //0
    }
    public virtual int InsertAfterFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        var insertCount = 0;

        for (var i = 0; i < Count; i++)
        {
            var iGenerateHtml = this[i];
            if (iGenerateHtml is Tag tag && predicate(tag))
            {
                Insert(i + 1, tags);
                insertCount++;
                return insertCount; //1
            }
        }

        foreach (var child in this)
        {
            insertCount += child.InsertAfterFirstWhere(predicate, tags);
            if (insertCount > 0) return insertCount; //1
        }

        return insertCount; //0
    }
    #endregion
}