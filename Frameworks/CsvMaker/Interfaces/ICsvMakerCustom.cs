using System.Text;

namespace CsvMaker.Interfaces;

public interface ICsvMakerCustom
{
    StringBuilder ToCsvRowCustom(StringBuilder? sb = null);
    StringBuilder ToCsvHeaderCustom(StringBuilder? sb = null);
}