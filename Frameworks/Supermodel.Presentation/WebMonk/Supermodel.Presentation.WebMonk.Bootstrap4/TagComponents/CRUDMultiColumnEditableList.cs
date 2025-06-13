using System;
using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDMultiColumnEditableList : CRUDMultiColumnEditableListBase
    {
        #region Constructors
        public CRUDMultiColumnEditableList(IEnumerable<IMvcModelForEntity> items, Type dataContextType, string pageTitle, bool skipAddNew = false, bool skipDelete = false) :
            base(items, dataContextType, null, new Txt(pageTitle), null, skipAddNew, skipDelete)
        { }
            
        public CRUDMultiColumnEditableList(IEnumerable<IMvcModelForEntity> items, Type dataContextType, IGenerateHtml? pageTitle = null, bool skipAddNew = false, bool skipDelete = false) :
            base(items, dataContextType, null, pageTitle, null, skipAddNew, skipDelete)
        { }
        #endregion
    }
}