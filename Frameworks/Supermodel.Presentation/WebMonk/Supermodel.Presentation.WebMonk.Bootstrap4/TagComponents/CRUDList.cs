using Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;
using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDList : CRUDListBase
    {
        #region Constructors
        public CRUDList(IEnumerable<IMvcModelForEntity> items, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, null, new Txt(pageTitle), skipAddNew, skipDelete, viewOnly, null)
        {}
            
        public CRUDList(IEnumerable<IMvcModelForEntity> items, IGenerateHtml? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, null, pageTitle, skipAddNew, skipDelete, viewOnly, null)
        {}
        #endregion
    }
}