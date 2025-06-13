using System.Collections.Generic;
using System.Linq;
using Supermodel.Presentation.WebMonk.Bootstrap4.Extensions;
using Supermodel.Presentation.WebMonk.Models.Mvc;
using Supermodel.ReflectionMapper;
using WebMonk.RazorSharp.HtmlTags;
using WebMonk.RazorSharp.HtmlTags.BaseTags;

// ReSharper disable once CheckNamespace
namespace Supermodel.Presentation.WebMonk.Bootstrap4.Models;

public static partial class Bs4
{ 
    public class CRUDMultiColumnListNoActions : HtmlSnippet
    {
        #region Constructors
        public CRUDMultiColumnListNoActions(IEnumerable<IMvcModel> items, string pageTitle) :
            this(items, new Txt(pageTitle))
        { }
            
        public CRUDMultiColumnListNoActions(IEnumerable<IMvcModel> items, IGenerateHtml? pageTitle = null)
        {
            if (pageTitle != null) Append(new H2(new { @class=ScaffoldingSettings.ListTitleCssClass}) { pageTitle } );   
            
            AppendAndPush(new Div(new { id=ScaffoldingSettings.CRUDListTopDivId, @class=ScaffoldingSettings.CRUDListTopDivCssClass }));
            AppendAndPush(new Table(new { id=ScaffoldingSettings.CRUDListTableId, @class=ScaffoldingSettings.CRUDListTableCssClass }));
            
            AppendAndPush(new Thead());
            AppendAndPush(new Tr());
            //Create header using reflection
            var mvcModelType = items.GetType().GetInterfaces().Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)).Select(t => t.GetGenericArguments()[0]).First();
            var mvcModelForHeader = ReflectionHelper.CreateType(mvcModelType);                
            Append(mvcModelForHeader.ToReadOnlyHtmlTableHeader());
            Pop<Tr>();
            Pop<Thead>();

            AppendAndPush(new Tbody());
            foreach (var item in items)
            {
                AppendAndPush(new Tr());
                //Render list columns using reflection
                Append(item.ToReadOnlyHtmlTableRow());
                Pop<Tr>();
            }
            Pop<Tbody>();

            Pop<Table>();
            Pop<Div>();
        }
        #endregion
    }
}