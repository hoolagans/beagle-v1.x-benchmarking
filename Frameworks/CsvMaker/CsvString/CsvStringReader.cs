using System;
using System.Text;

namespace CsvMaker.CsvString;

public class CsvStringReader
{
    #region Constructors
    public CsvStringReader(string csvFile)
    {
        _currentIndex = 0;
        _csvFile = csvFile;
    }
    #endregion

    #region Methods
    public void SkipRow()
    {
        while(!IsNextCharEOL()) ReadNextColumn();
        ReadEOLorEOF();
    }
    public bool IsEOF()
    {
        return _currentIndex > _csvFile.Length - 1;
    }
    public string ReadNextColumn()
    {
        if (_lastColumnEndedWithEOLorEOF && IsNextCharEOL())
        {
            ReadEOLorEOF();
            throw new EOLException();
        }

        ReadWhitespace();
        if (PeekNextChar() == '"' && !AreNextTwoCharactersDoubleQuotes()) return ReadQuotedColumn();

        var sb = new StringBuilder();
        while (true)
        {
            if (IsNextCharEOF())
            {
                _lastColumnEndedWithEOLorEOF = true;
                return sb.ToString().TrimEnd();
            }
            if (IsNextCharEOL())
            {
                _lastColumnEndedWithEOLorEOF = true;
                return sb.ToString().TrimEnd();
            }
            if (AreNextTwoCharactersDoubleQuotes())
            {
                sb.Append('"');
                MoveOneChar();
                MoveOneChar();
            }

            var nextChar = ReadNextChar();
            if (nextChar == '"') throw new FormatException("Double quote in the middle of a column");
            else if (nextChar == ',') return sb.ToString().TrimEnd();
            else sb.Append(nextChar);
        }
    }
    public string ReadQuotedColumn()
    {
        if (ReadNextChar() != '"') throw new FormatException("ReadQuotedColumn for a column that does not start from a quote");
        var sb = new StringBuilder();
        while (true)
        {
            if (AreNextTwoCharactersDoubleQuotes())
            {
                sb.Append('"');
                MoveOneChar();
                MoveOneChar();
            }
            var nextChar = ReadNextChar();

            if (nextChar == '"')
            {
                ReadWhitespace();
                if (IsNextCharEOL() || IsEOF()) return sb.ToString();
                if (ReadNextChar() != ',') throw new FormatException("ReadQuotedColumn: non-whitespace characters after closing '\"' and before ','");
                return sb.ToString();
            }
            else
            {
                sb.Append(nextChar);
            }
        }
    }
    public void ReadEOLorEOF()
    {
        _lastColumnEndedWithEOLorEOF = false;

        var newlineFound = false;
        while (true)
        {
            if (IsEOF()) return;
                
            if (IsNextCharEOL())
            {
                if (!newlineFound)
                {
                    newlineFound = true;
                    MoveOneChar();
                }
                else
                {
                    return;
                }
            }
            else
            {
                var nextChar = PeekNextChar();
                if (char.IsWhiteSpace(nextChar))
                {
                    MoveOneChar();
                }
                else
                {
                    if (newlineFound) return;
                    else throw new FormatException("EOL is expected");
                }
            }
        }
    }
    public string ReadWhitespace()
    {
        var sb = new StringBuilder();
        while (true)
        {
            if (IsNextCharEOL() || IsEOF()) return sb.ToString();
            var nextChar = PeekNextChar();
            if (char.IsWhiteSpace(nextChar))
            {
                sb.Append(nextChar);
                MoveOneChar();
            }
            else
            {
                return sb.ToString();
            }
        }
    }
    public char ReadNextChar()
    {
        var nextChar = PeekNextChar();
        MoveOneChar();
        return nextChar;
    }
    public bool IsNextCharEOF()
    {
        return _currentIndex > _csvFile.Length - 1;
    }
    public bool IsNextCharEOL()
    {
        var nextChar = PeekNextChar();
        {
            if (nextChar == '\n')
            {
                try { if (PeekCharacterAfterNext() == '\r') MoveOneChar(); }
                catch (EOFException) { }
                return true;
            }
            if (nextChar == '\r')
            {
                try { if (PeekCharacterAfterNext() == '\n') MoveOneChar(); }
                catch (EOFException) { }
                return true;
            }
            return false;
        }
    }
    public char PeekCharacterAfterNext()
    {
        if (_currentIndex + 1 > _csvFile.Length - 1) throw new EOFException();
        return _csvFile[_currentIndex + 1];
    }
    public char PeekNextChar()
    {
        if (_currentIndex > _csvFile.Length - 1) throw new EOFException();
        return _csvFile[_currentIndex];
    }
    public void MoveOneChar()
    {
        _currentIndex++;
    }
    public bool AreNextTwoCharactersDoubleQuotes()
    {
        if (_currentIndex > _csvFile.Length) throw new EOFException();
        if (_currentIndex + 1 > _csvFile.Length) return false;
        return (_csvFile[_currentIndex] == '"' && _csvFile[_currentIndex + 1] == '"');
    }
    #endregion

    #region Privates
    protected readonly string _csvFile;
    protected int _currentIndex;
    protected bool _lastColumnEndedWithEOLorEOF;
    #endregion
}