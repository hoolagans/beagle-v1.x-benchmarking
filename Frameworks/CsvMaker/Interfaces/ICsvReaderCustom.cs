using CsvMaker.CsvString;

namespace CsvMaker.Interfaces;

public interface ICsvReaderCustom
{
    T ValidateCsvHeaderCustom<T>(CsvStringReader sr);
    T ReadCsvRowCustom<T>(CsvStringReader sr);
}