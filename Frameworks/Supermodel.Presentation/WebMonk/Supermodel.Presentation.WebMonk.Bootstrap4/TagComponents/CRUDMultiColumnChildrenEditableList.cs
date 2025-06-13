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
    public class CRUDMultiColumnChildrenEditableList : CRUDMultiColumnEditableListBase
    {
        #region Constructors
        public CRUDMultiColumnChildrenEditableList(IEnumerable<IChildMvcModelForEntity> items, Type dataContextType, Type childControllerType, long parentId, string pageTitle, bool skipAddNew = false, bool skipDelete = false) :
            base(items, dataContextType, childControllerType, new Txt(pageTitle), parentId, skipAddNew, skipDelete)
        { }
            
        public CRUDMultiColumnChildrenEditableList(IEnumerable<IChildMvcModelForEntity> items, Type dataContextType, Type childControllerType, long parentId, IGenerateHtml? pageTitle = null, bool skipAddNew = false, bool skipDelete = false) :
            base(items, dataContextType, childControllerType, pageTitle, parentId, skipAddNew, skipDelete)
        { }       
        #endregion
    }
}