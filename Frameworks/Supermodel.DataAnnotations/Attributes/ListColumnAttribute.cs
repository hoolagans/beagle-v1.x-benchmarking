using System;
using System.Linq;
using System.Text;

namespace Supermodel.DataAnnotations.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class ListColumnAttribute : Attribute
{
    #region Constructors
    public ListColumnAttribute()
    {
        Header = null;
        HeaderOrder = 100;
        OrderBy = null;
        OrderByDesc = null;
    }
    #endregion

    #region Methods
    public static string? InverseOrder(string? order)
    {
        if (string.IsNullOrEmpty(order)) return order;
            
        var invertedOrderSb = new StringBuilder();

        var columnNamesToSortBy = order.Split(',');
        var first = true;
        foreach(var column in columnNamesToSortBy.Select(x => x.Trim()))
        {
            string invertedColumn;
            if (column.StartsWith("-")) invertedColumn = column.Substring(1);
            else invertedColumn = "-" + column;

            if (first) first = false;
            else invertedOrderSb.Append(", ");

            invertedOrderSb.Append(invertedColumn);
        }

        return invertedOrderSb.ToString();
    }
    #endregion
        
    #region Properties
    public string? Header { get; set; }
    public int HeaderOrder { get; set; }
    public string? OrderBy { get; set; }
        
    public string? OrderByDesc
    {
        get => !string.IsNullOrEmpty(_orderByDesc) ? _orderByDesc : InverseOrder(OrderBy);
        set => _orderByDesc = value;
    }
    private string? _orderByDesc;
    #endregion
}