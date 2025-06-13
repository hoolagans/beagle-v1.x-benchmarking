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
    public class CRUDMultiColumnChildrenListNoActions : CRUDMultiColumnListNoActionBase
    {
        #region Constructors
        public CRUDMultiColumnChildrenListNoActions(IEnumerable<IChildMvcModelForEntity> items, Type detailControllerType, string pageTitle) :
            base(items, new Txt(pageTitle), detailControllerType)
        { }
            
        protected CRUDMultiColumnChildrenListNoActions(IEnumerable<IChildMvcModelForEntity> items, Type detailControllerType, IGenerateHtml? pageTitle = null) :
            base(items, pageTitle, detailControllerType)
        { }
        #endregion
    }
}