using Supermodel.Presentation.WebMonk.Bootstrap4.TagComponents.Base;
using System;
using System.Collections.Generic;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDChildrenList : CRUDListBase
    {
        #region Constructors
        public CRUDChildrenList(IEnumerable<IMvcModelForEntity> items, Type childControllerType, long parentId, string pageTitle, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, parentId, new Txt(pageTitle), skipAddNew, skipDelete, viewOnly, childControllerType)
        { }

        public CRUDChildrenList(IEnumerable<IMvcModelForEntity> items, Type childControllerType, long parentId, IGenerateHtml? pageTitle = null, bool skipAddNew = false, bool skipDelete = false, bool viewOnly = false) :
            base(items, parentId, pageTitle, skipAddNew, skipDelete, viewOnly, childControllerType)
        { }
        #endregion
    }
}