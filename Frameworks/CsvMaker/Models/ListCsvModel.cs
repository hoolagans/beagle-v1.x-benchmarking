using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using CsvMaker.CsvString;
using CsvMaker.Extensions;
using CsvMaker.Interfaces;

namespace CsvMaker.Models;

public class ListCsvModel : List<string>, ICsvMakerCustom, ICsvReaderCustom
{
    #region ICsvMakerCustom implementation
    public virtual StringBuilder ToCsvRowCustom(StringBuilder? sb = null)
    {
        sb ??= new StringBuilder();

        var firstColumn = true;
        foreach (var columnStr in this)
        {
            if (firstColumn) firstColumn = false;
            else sb.Append(",");

            if (columnStr == null) throw new NoNullAllowedException("ListCsvModel.ToCsvRowCustom(): columnStr == null");
            sb.Append(columnStr.PrepareCvsColumn());
        }
        return sb;
    }
    public virtual StringBuilder ToCsvHeaderCustom(StringBuilder? sb = null)
    {
        throw new InvalidOperationException("ListCsvModel does not support ValidateCsvHeaderCustom");
    }
    #endregion

    #region ICsvReaderCustom implementation
    public virtual T ValidateCsvHeaderCustom<T>(CsvStringReader sr)
    {
        throw new InvalidOperationException("ListCsvModel does not support ValidateCsvHeaderCustom");
    }
    public virtual T ReadCsvRowCustom<T>(CsvStringReader sr)
    {
        while (true)
        {
            string csvColumnStr;
            try
            {
                csvColumnStr = sr.ReadNextColumn();
            }
            catch(EOLException)
            {
                return (T)(object)this;
            }
            catch (EOFException)
            {
                return (T)(object)this;
            }
            Add(string.IsNullOrWhiteSpace(csvColumnStr) ? "" : csvColumnStr);
        }
    }
    #endregion
}