using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrowserEmulator;

public class BrowserEmulatorParser : ParseHTML
{
    #region Constructors
    public BrowserEmulatorParser(string page)
    {
        Source = page;
    }
    #endregion

    #region Find Methods
    public AttributeList FindTag(string tagName, object attributesObj = null)
    {
        return FindTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public AttributeList FindTag(string tagName, IDictionary<string, string> attributesDict)
    {
        try
        {
            while (true)
            {
                var ch = Parse();
                if (ch == 0)
                {
                    var tag = GetTag();
                    if (tag.Matches(tagName, attributesDict)) return tag;
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
        }
    }
    public AttributeList FindTags(params string[] tagNames)
    {
        try
        {
            while (true)
            {
                var ch = this.Parse();
                if (ch == 0)
                {
                    var tag = GetTag();
                    if (tagNames.Any(tagName => tag.Name.ToLower() == (tagName).ToLower())) return tag;
                }
            }
        }
        catch (IndexOutOfRangeException)
        {
            var sb = new StringBuilder();
            var first = true;
            foreach (var tagName in tagNames)
            {
                if (first)
                {
                    first = false;
                    sb.AppendLine("<" + tagName + ">");
                }
                else
                {
                    sb.AppendLine(" or <" + tagName + ">");
                }
            }
            throw new EOFException("Unexpected EOF where " + sb + " is expected");
        }
    }
    #endregion

    #region Process Methods
    public AttributeList ProcessTag(string tagName = null, object attributesObj = null)
    {
        return ProcessTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public AttributeList ProcessTag(string tagName, IDictionary<string, string> attributesDict) 
    {
        try 
        {
            while (true) 
            {
                var ch = Parse();
                if (ch == 0) 
                {
                    var tag = GetTag();
                    tag.EnsureMatches(tagName, attributesDict);
                    return tag;
                }
            }
        }
        catch (IndexOutOfRangeException) 
        {
            throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
        }
    }
    public AttributeList FindClosingTag(string closingTagName)
    {
        var openingTagName = closingTagName.Replace("/", "");
        try
        {
            while (true)
            {
                var nextTag = PeekNextTag();
                if (nextTag.Name == openingTagName)
                {
                    ProcessOpeningAndClosingTag(openingTagName);
                } 
                else if (nextTag.Name == closingTagName)
                {
                    break;
                }
                else
                {
                    ProcessTag();
                }
            }
            return ProcessTag(closingTagName);
        }
        catch (IndexOutOfRangeException)
        {
            throw new EOFException("Unexpected EOF where <" + openingTagName + "> is expected");
        }
    }
    public void ProcessOptionalTag(string tagName, object attributesObj = null)
    {
        ProcessOptionalTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public void ProcessOptionalTag(string tagName, IDictionary<string, string> attributesDict)
    {
        var nextTag = PeekNextTag();
        if (nextTag.Matches(tagName, attributesDict)) ProcessTag(tagName, attributesDict);
    }
    public void ProcessOpeningAndClosingTag(string tagName, object attributesObj = null)
    {
        ProcessOpeningAndClosingTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public void ProcessOpeningAndClosingTag(string tagName, IDictionary<string, string> attributesDict)
    {
        try
        {
            ProcessTag(tagName, attributesDict);
            FindClosingTag("/" + tagName);
        }
        catch (IndexOutOfRangeException)
        {
            throw new EOFException("Unexpected EOF where <" + tagName + "> is expected");
        }
    }
    public void ProcessOptionalOpeningAndClosingTag(string tagName, object attributesObj = null)
    {
        ProcessOptionalOpeningAndClosingTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public void ProcessOptionalOpeningAndClosingTag(string tagName, IDictionary<string, string> attributesDict)
    {
        var nextTag = PeekNextTag();
        if (nextTag.Matches(tagName, attributesDict)) ProcessOpeningAndClosingTag(tagName, attributesDict);
    }
    public void ProcessTextToTag(string expectedText, string tagName)
    {
        var text = GetTextToTag(tagName);
        if (text != expectedText) throw new BrowserEmulatorException("'" + text + "' while expecting '" + expectedText + "' before <" + tagName + ">");
    }
    #endregion

    #region Get Line Number Methods
    public int GetCurrentLineNumber()
    {
        return GetCurrentLineNumberForParserPoint(m_idx);
    }
    public int GetCurrentLineNumberForParserPoint(int parserPoint)
    {
        var line = 1;
        for (var i = 0; i < parserPoint && i < Source.Length - 1; i++)
        {
            if (Source[i] == '\n') line++;
        }

        return line;
    }
    #endregion

    #region Peek and Rewind/FF methods and Text methods
    public AttributeList PeekNextTag() 
    {
        var tmpIdx = m_idx;
        try 
        {
            while (true) 
            {
                var ch = this.Parse();
                if (ch == 0) return GetTag();
            }
        }
        catch (IndexOutOfRangeException) 
        {
            throw new EOFException("Unexpected EOF where an HTML tag is expected");
        }
        finally 
        {
            m_idx = tmpIdx;
        }
    }
    public int FindText(string text, bool ignoreCase = false)
    {
        return ignoreCase ? m_source.ToLower().IndexOf(text.ToLower(), m_idx, StringComparison.Ordinal) : m_source.IndexOf(text, m_idx, StringComparison.Ordinal);
    }
    public void FastForwardToText(string text)
    {
        var idx = FindText(text);
        if (idx == -1) throw new EOFException("Unexpected EOF while fast forwarding to '" + text + "' text is expected");
        SetParserPoint(idx);
    }
    public bool TextExistsAhead(string text)
    {
        var idx = m_source.IndexOf(text, m_idx, StringComparison.Ordinal);
        return idx != -1;
    }
    public int GetParserPoint()
    {
        return m_idx;
    }
    public void SetParserPoint(int parserPoint)
    {
        if (parserPoint < 0) throw new BrowserEmulatorException("Parser point must be greater than 0, while it is " + parserPoint);
        m_idx = parserPoint;
    }
    public string GetTextToNextTag(out AttributeList tag) 
    {
        StringBuilder text = new StringBuilder();
        try 
        {
            while (true) 
            {
                var ch = Parse();
                if (ch == 0) 
                {
                    tag = GetTag();
                    break;
                } 
                else 
                {
                    text.Append(ch);
                }
            }
        }
        catch (IndexOutOfRangeException) 
        {
            throw new EOFException("Unexpected EOF where an HTML tag is expected");
        }
        var result = text.ToString().Trim();
        if (result.Contains(">")) throw new BrowserEmulatorException($"Invalid HTML contract near '{result}'");
        return result;
    }
    public string GetTextAndTagsToTag(string tagName, object attributesObj = null)
    {
        return GetTextAndTagsToTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public string GetTextAndTagsToTag(string tagName, IDictionary<string, string> attributesDict) 
    {
        var initPosition = GetParserPoint();
        FindTag(tagName, attributesDict);
        var tagPosition = GetParserPoint();
        var textAndTags = m_source.Substring(initPosition, tagPosition - initPosition);
        var lowTextAndTags = textAndTags.ToLower();
        var closingTagIdx = lowTextAndTags.IndexOf("<" + tagName, StringComparison.Ordinal);
        var textAndTagsMinusEndTag = textAndTags.Substring(0, closingTagIdx);
        return textAndTagsMinusEndTag;
    }
    public string GetTextToTag(string tagName, object attributesObj = null)
    {
        return GetTextToTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public string GetTextToTag(string tagName, IDictionary<string, string> attributesDict) 
    {
        var text = new StringBuilder();
        try 
        {
            while (true) 
            {
                var ch = Parse();
                if (ch == 0) {
                    var tag = GetTag();
                    if (tag.Matches(tagName, attributesDict)) break;
                    throw new BrowserEmulatorException("<" + tag.Name + "> where <" + tagName + "> is expected");
                } 
                text.Append(ch);
            }
        }
        catch (IndexOutOfRangeException) 
        {
            throw new EOFException("Unexpected EOF where " + tagName + " is expected");
        }
        return text.ToString().Trim();
    }
    public string GetTextInsideTheTag(string tagName, object attributesObj = null)
    {
        return GetTextInsideTheTag(tagName, AttributesHelper.AnonymousObjectToHtmlAttributes(attributesObj));
    }
    public string GetTextInsideTheTag(string tagName, IDictionary<string, string> attributesDict)
    {
        ProcessTag(tagName, attributesDict);
        return GetTextToTag("/" + tagName);
    }
    #endregion
}