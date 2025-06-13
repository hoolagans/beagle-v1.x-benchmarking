using System.Text;
using System.Threading.Tasks;
using CsvMaker.CsvString;
using CsvMaker.Extensions;
using CsvMaker.Interfaces;
using Supermodel.ReflectionMapper;

namespace CsvMaker.Models;

public abstract class CsvModel : ICsvMakerCustom, ICsvReaderCustom, IRMapperCustom
{
    #region ICsvMakerCustom implementation
    public virtual StringBuilder ToCsvRowCustom(StringBuilder? sb = null)
    {
        return this.ToCsvRowBase(sb);
    }
    public virtual StringBuilder ToCsvHeaderCustom(StringBuilder? sb = null)
    {
        return this.ToCsvHeaderBase(sb);
    }
    #endregion

    #region ICsvReaderCustom implementation
    public virtual T ValidateCsvHeaderCustom<T>(CsvStringReader sr)
    {
        return (T)(object)this.ValidateCsvHeaderRowBase(sr);
    }

    public virtual T ReadCsvRowCustom<T>(CsvStringReader sr)
    {
        return (T)(object)this.ReadCsvRowBase(sr);
    }
    #endregion

    #region IRMapperCustom implementation
    public virtual Task MapFromCustomAsync<T>(T other)
    {
        return this.MapFromCustomBaseAsync(other);
    }
    public virtual Task<T> MapToCustomAsync<T>(T other)
    {
        return this.MapToCustomBaseAsync(other);
    }
    #endregion
}