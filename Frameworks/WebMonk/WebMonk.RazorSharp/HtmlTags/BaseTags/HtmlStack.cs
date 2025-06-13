using System;
using System.Collections.Generic;
using System.Linq;
using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public class HtmlStack : IGenerateHtml
{
    #region Constructors
    public HtmlStack()
    {
        RootTags = new Tags();
        Stack.Push(RootTags);
    }
    #endregion

    #region Methods
    public Txt Append(string txt)
    {
        return Append(new Txt(txt));
    }
    public T Append<T>(T html) where T : IGenerateHtml
    {
        Stack.Peek().Add(html);
        return html;
    }
    public T AppendAndPush<T>(T html) where T : IGenerateAndContainHtml
    {
        Stack.Peek().Add(html);
        Stack.Push(html);
        return html;
    }

    public IGenerateAndContainHtml Pop()
    {
        if (Stack.Count <= 1) throw new InvalidOperationException("Stack is empty");
        return Stack.Pop();
    }
    public T Pop<T>() where T : IGenerateAndContainHtml
    {
        if (Stack.Count <= 1) throw new InvalidOperationException("Stack is empty");
        var pop = Stack.Pop();
        if (typeof(T) != pop.GetType()) throw new ArgumentException("Type T does not match the pop");
        return (T)pop;
    }

    public IGenerateAndContainHtml Peek()
    {
        if (Stack.Count <= 1) throw new InvalidOperationException("Stack is empty");
        return Stack.Peek();
    }
    public T Peek<T>() where T : IGenerateAndContainHtml
    {
        if (Stack.Count <= 1) throw new InvalidOperationException("Stack is empty");
        var peek = Stack.Peek();
        if (typeof(T) != peek.GetType()) throw new ArgumentException("Type T does not match the pop");
        return (T)peek;
    }

    public int Count => Stack.Count - 1;
    #endregion

    #region IGenerateHtml implementation
    public virtual StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null)
    {
        if (Count > 0) throw new IndexOutOfRangeException("Cannot convert HtmlStack to html when it has unPOPed elements (other than root)");
        return RootTags.ToHtml(sb);
    }

    public virtual IGenerateHtml FillBodySectionWith(IGenerateHtml replacement)
    {
        return RootTags.FillBodySectionWith(replacement);
    }
    public virtual IGenerateHtml FillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        return RootTags.FillSectionWith(sectionId, replacement);
    }
    public virtual bool TryFillSectionWith(string sectionId, IGenerateHtml replacement)
    {
        return RootTags.TryFillSectionWith(sectionId, replacement);
    }

    public virtual IGenerateHtml DisableAllControls()
    {
        RootTags.DisableAllControls();
        return this;
    }
    public virtual IGenerateHtml DisableAllControlsIf(bool condition)
    {
        RootTags.DisableAllControlsIf(condition);
        return this;
    }

    public virtual IEnumerable<Tag> GetTagsInOrder()
    {
        return RootTags.GetTagsInOrder();
    }
    public virtual List<Tag> NormalizeAndFlatten()
    {
        return RootTags.NormalizeAndFlatten();
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
        return RootTags.RemoveWhere(predicate);
    }
    public virtual int RemoveFirstWhere(Func<Tag, bool> predicate)
    {
        return RootTags.RemoveFirstWhere(predicate);
    }

    public virtual int InsertBeforeWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return RootTags.InsertBeforeWhere(predicate, tags);
    }
    public virtual int InsertAfterWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return RootTags.InsertAfterWhere(predicate, tags);
    }

    public virtual int InsertBeforeFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return RootTags.InsertBeforeFirstWhere(predicate, tags);
    }
    public virtual int InsertAfterFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags)
    {
        return RootTags.InsertAfterFirstWhere(predicate, tags);
    }
    #endregion

    #region Properties
    protected Stack<IGenerateAndContainHtml> Stack { get; set; } = new();
    public Tags RootTags { get; }
    #endregion
}