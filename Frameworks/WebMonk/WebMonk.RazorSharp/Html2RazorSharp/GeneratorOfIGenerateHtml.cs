using System;
using System.Collections.Generic;
using Attribute = BrowserEmulator.Attribute;

namespace WebMonk.RazorSharp.Html2RazorSharp;

public abstract class GeneratorOfIGenerateHtml<TAttribute, TResult>
{
    #region Constructors
    protected GeneratorOfIGenerateHtml(bool sortAttributes = false, bool generateInvalidTags = false)
    {
        SortAttributes = sortAttributes;
        GenerateInvalidTags = generateInvalidTags;
    }
    #endregion

    #region Methods
    internal abstract void Initialize();

    internal abstract TAttribute GenerateAttributes(List<Attribute> attributes);

    internal abstract void AddRecognizedSelfClosingTag(Type tag, TAttribute attributes);

    internal abstract bool AddRecognizedNonSelfClosingTagAndPotentiallyPop(Type tag, bool attributeAreEmpty, bool closesSelf, bool emptyTag, TAttribute attributes);

    internal abstract bool AddInvalidTagAndPotentiallyPop(string tagName, bool attributesAreEmpty, bool closesSelf, bool emptyTag, TAttribute attributes);

    internal abstract void CloseTag();

    internal abstract void AddTxtTag(string text);

    internal abstract void Finish();

    internal abstract TResult GetResult();
    #endregion

    #region Properties
    internal bool SortAttributes { get; }
    internal bool GenerateInvalidTags { get; }
    #endregion
}