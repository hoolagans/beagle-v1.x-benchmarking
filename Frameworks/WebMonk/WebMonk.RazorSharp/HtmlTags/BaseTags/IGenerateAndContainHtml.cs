using System.Collections.Generic;

namespace WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IGenerateAndContainHtml : IGenerateHtml, IList<IGenerateHtml>
{
    void Add(string txt);
}