using System;
using System.Collections.Generic;
using Supermodel.DataAnnotations;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

public interface IGenerateHtml
{
    StringBuilderWithIndents ToHtml(StringBuilderWithIndents? sb = null);
        
    IGenerateHtml FillBodySectionWith(IGenerateHtml replacement);
    IGenerateHtml FillSectionWith(string sectionId, IGenerateHtml replacement);
    bool TryFillSectionWith(string sectionId, IGenerateHtml replacement);

    IGenerateHtml DisableAllControls();
    IGenerateHtml DisableAllControlsIf(bool condition);

    List<Tag> NormalizeAndFlatten(); 
    IEnumerable<Tag> GetTagsInOrder();

    //Find Tag(s) methods
    int CountWhere(Func<Tag, bool> predicate);
    IEnumerable<Tag> Where(Func<Tag, bool> predicate);
    
    IEnumerable<Tag?> ParentsOfTagsWhere(Func<Tag, bool> predicate);

    Tag SingleWhere(Func<Tag, bool> predicate);
    Tag? SingleOrDefaultWhere(Func<Tag, bool> predicate);
    Tag FirstWhere(Func<Tag, bool> predicate);
    Tag? FirstOrDefaultWhere(Func<Tag, bool> predicate);
    bool AnyWhere(Func<Tag, bool> predicate);
    bool AllWhere(Func<Tag, bool> predicate);

    //Remove Tag(s) methods
    int RemoveWhere(Func<Tag, bool> predicate);
    int RemoveFirstWhere(Func<Tag, bool> predicate);

    //Insert Tag(s) methods
    int InsertBeforeWhere(Func<Tag, bool> predicate, IGenerateHtml tags);
    int InsertAfterWhere(Func<Tag, bool> predicate, IGenerateHtml tags);

    int InsertBeforeFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags);
    int InsertAfterFirstWhere(Func<Tag, bool> predicate, IGenerateHtml tags);
}