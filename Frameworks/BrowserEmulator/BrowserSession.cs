using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserEmulator;
//public class DefaultCertificatePolicy : ICertificatePolicy 
//{
//  public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem) 
//  {
//		//Return True to force any certificate to be accepted.
//		return true;
//	}
//}

public class BrowserSession 
{
    public BrowserSession()
    {
        ResponseAbsoluteUrl = "";
        ResponseAbsolutePath = "";
        AllowAutoRedirect = true;
        Retries = 1;
        SleepBetweenRetries = 0;
        Timeout = 20000;
        ProtocolVersion = HttpVersion.Version11;
        SessionCookies = new CookieContainer();
        UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/27.0.1453.116 Safari/537.36";
    }
        
    public void Reset() 
    {
        SessionCookies = new CookieContainer();
        _referrer = "";
    }

    public async Task<string> HTTPGetAsync(string url)
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "GET";

                pageHtml = await GetStringResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }
    public string HTTPGet(string url)
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "GET";

                pageHtml = GetStringResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }
		
    public async Task<byte[]> HTTPGetReceiveBinaryAsync(string url)
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "GET";

                responseFile = await GetBinaryResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }
    public byte[] HTTPGetReceiveBinary(string url) 
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "GET";

                responseFile = GetBinaryResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }

    public async Task<string> HTTPPostAsync(string url, FieldsCollection fields)
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var utf8Encoding = new UTF8Encoding();
                var requestBody = utf8Encoding.GetBytes(fields.GetHTTPKeysAndValuesString());
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                pageHtml = await GetStringResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }
    public string HTTPPost(string url, FieldsCollection fields) 
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var utf8Encoding = new UTF8Encoding();
                var requestBody = utf8Encoding.GetBytes(fields.GetHTTPKeysAndValuesString());
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                pageHtml = GetStringResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }
            
        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }

    public async Task<byte[]> HTTPPostReceiveBinaryAsync(string url, FieldsCollection fields)
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var utf8Encoding = new UTF8Encoding();
                var requestBody = utf8Encoding.GetBytes(fields.GetHTTPKeysAndValuesString());
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                responseFile = await GetBinaryResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }
    public byte[] HTTPPostReceiveBinary(string url, FieldsCollection fields) 
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var utf8Encoding = new UTF8Encoding();
                var requestBody = utf8Encoding.GetBytes(fields.GetHTTPKeysAndValuesString());
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                responseFile = GetBinaryResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }

    public async Task<string> HTTPPostFileAsync(string url, byte[] file)
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var requestBody = file;
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                pageHtml = await GetStringResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }
    public string HTTPPostFile(string url, byte[] file) 
    {
        HttpWebRequest request;
        string pageHtml;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var requestBody = file;
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                pageHtml = GetStringResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return pageHtml;
    }

    public async Task<byte[]> HTTPPostFileReceiveBinaryAsync(string url, byte[] file) 
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var requestBody = file;
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                responseFile = await GetBinaryResponseAsync(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }
    public byte[] HTTPPostFileReceiveBinary(string url, byte[] file)
    {
        HttpWebRequest request;
        byte[] responseFile;
        var attempt = 1;

        while (true)
        {
            try
            {
                request = SetUpHeader(url, Timeout);
                request.Method = "POST";

                var requestBody = file;
                request.ContentLength = requestBody.Length;

                request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
                request.GetRequestStream().Close();

                responseFile = GetBinaryResponse(request);
                break;
            }
            catch (WebException)
            {
                attempt++;
                if (attempt >= Retries) throw;
                Thread.Sleep(SleepBetweenRetries);
            }
        }

        SessionCookies = request.CookieContainer;
        _referrer = url;
        return responseFile;
    }

    public async Task<string> HTTPPostStringFileAsync(string url, string stringFile)
    {
        var utf8Encoding = new UTF8Encoding();
        return await HTTPPostFileAsync(url, utf8Encoding.GetBytes(stringFile));
    }
    public string HTTPPostStringFile(string url, string stringFile) 
    {
        var utf8Encoding = new UTF8Encoding();
        return HTTPPostFile(url, utf8Encoding.GetBytes(stringFile));
    }

    public static byte[] Stream2ByteArray(Stream stream) 
    {
        byte[] buffer = new byte[32768];
        using (MemoryStream ms = new MemoryStream()) 
        {
            while (true) 
            {
                var read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0) return ms.ToArray();
                ms.Write(buffer, 0, read);
            }
        }
    }

    #region ASP.NET Froms postback related fields
    public string HTTPDotNetPostback(string page, string formName, string formAction, string postbackField, string eventArg, string url, FieldsCollection variablePostbackFields, int timeout)
    {
        FieldsCollection input = GetDotNetPostbackInput(page, formName, formAction, postbackField, eventArg, url, variablePostbackFields, timeout, true);
        //return input.GetHTTPKeysAndValuesStringWithBR();
        if (input != null) return HTTPPost(url, input);
        else return page;
    }
    public FieldsCollection GetDotNetPostbackInput(string page, string formName, string formAction, string postbackField, string eventArg, string url, FieldsCollection variablePostbackFields, int timeout, bool fieldValidation)
    {
        FieldsCollection input = new FieldsCollection();
        input.AutogenerateDefaultFields(page, formName, formAction);

        if (fieldValidation)
        {
            //check to see that postback field exist
            try
            {
                input.GetKey(postbackField);
            }
            catch (FieldDoesNotExistException)
            {
                throw new BrowserEmulatorException("Postback field (" + postbackField + ") does not exist on the page");
            }
        }
        input["__EVENTTARGET"].HttpValue = postbackField;
        input["__EVENTARGUMENT"].HttpValue = eventArg;
        input["__VIEWSTATE"].HttpValue = FieldsCollection.GetVIEWSTATE(page, formName, formAction);

        // if value changed we submit
        bool submit = true;
        if (fieldValidation)
        {
            if (input[postbackField].HttpValue == variablePostbackFields[postbackField].HttpValue) submit = false;
        }

        input.MergeReplace(variablePostbackFields);
        input.DoNotSubmitSubmit();
        input.ValidateAgainstHTMLPage(page, formName, formAction);

        //if we don't need to submit we return null
        if (submit) return input;
        else return null;
    }
    public string HTTPDotNetPostbackNoFieldValidation(string page, string formName, string formAction, string postbackField, string eventArg, string url, FieldsCollection variablePostbackFields, int timeout)
    {
        FieldsCollection input = GetDotNetPostbackInput(page, formName, formAction, postbackField, eventArg, url, variablePostbackFields, timeout, false);
        //return input.GetHTTPKeysAndValuesStringWithBR();
        if (input != null) return HTTPPost(url, input);
        else return page;
    }
    #endregion

    private async Task<byte[]> GetBinaryResponseAsync(HttpWebRequest request)
    {
        var response = await request.GetResponseAsync();
        ResponseAbsoluteUrl = response.ResponseUri.AbsoluteUri;
        ResponseAbsolutePath = response.ResponseUri.AbsolutePath;

        var responseStream = response.GetResponseStream();
        return Stream2ByteArray(responseStream);
    }
    private byte[] GetBinaryResponse(HttpWebRequest request) 
    {
        var response = request.GetResponse();
        ResponseAbsoluteUrl = response.ResponseUri.AbsoluteUri;
        ResponseAbsolutePath = response.ResponseUri.AbsolutePath;

        var responseStream = response.GetResponseStream();
        return Stream2ByteArray(responseStream);
    }

    private async Task<string> GetStringResponseAsync(WebRequest request)
    {
        var response = await request.GetResponseAsync();

        ResponseAbsoluteUrl = response.ResponseUri.AbsoluteUri;
        ResponseAbsolutePath = response.ResponseUri.AbsolutePath;

        var responseStream = response.GetResponseStream();

        if (responseStream == null) throw new Exception("This should never happen");
        using(var stream = new StreamReader(responseStream, Encoding.ASCII))
        {
            var pageHtml = await stream.ReadToEndAsync();
            return pageHtml;
        }
    }
    private string GetStringResponse(WebRequest request) 
    {
        var response = request.GetResponse();
        ResponseAbsoluteUrl = response.ResponseUri.AbsoluteUri;
        ResponseAbsolutePath = response.ResponseUri.AbsolutePath;

        var responseStream = response.GetResponseStream();

        if (responseStream == null) throw new Exception("This should never happen");
        using(var stream = new StreamReader(responseStream, Encoding.ASCII))
        {
            var pageHtml = stream.ReadToEnd();
            return pageHtml;
        }
    }

    private HttpWebRequest SetUpHeader(string url, int timeout) 
    {
        var request = (HttpWebRequest)WebRequest.Create(url);

        request.Timeout = request.ReadWriteTimeout = timeout;

        request.ProtocolVersion = ProtocolVersion;

        request.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, application/msword, application/vnd.ms-excel, application/vnd.ms-powerpoint, application/x-shockwave-flash, */*";
        request.ContentType = "application/x-www-form-urlencoded";
        request.UserAgent = UserAgent;
        request.Headers.Add("Accept-Language: en-us");
        request.Headers.Add("Cache-Control: no-cache");
        request.AllowAutoRedirect = AllowAutoRedirect;
        request.CookieContainer = SessionCookies;

        if (_referrer != "") request.Referer = _referrer;
        return request;
    }

    public string ResponseAbsoluteUrl { get; set; }
    public string ResponseAbsolutePath { get; set; }
    public bool AllowAutoRedirect { get; set; }
        
    public int Timeout { get; set; }
    public int Retries { get; set; }
    public int SleepBetweenRetries { get; set; }

    public Version ProtocolVersion { get; set; }
    public CookieContainer SessionCookies { get; set; }
    public string UserAgent { get; set; }
		
    private string _referrer = "";
}