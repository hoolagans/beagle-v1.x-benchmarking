using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDMultiColumnList : CRUDMultiColumnListBase
    {
        #region Constructors
        public CRUDMultiColumnList(IEnumerable<IMvcModelForEntity> items, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, null, new Txt(pageTitle), null, skipAddNew, skipDelete, viewOnly)
        { }

        public CRUDMultiColumnList(IEnumerable<IMvcModelForEntity> items, IGenerateHtml? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, null, pageTitle, null, skipAddNew, skipDelete, viewOnly)
        { }
        #endregion
    }
}