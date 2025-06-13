using System;
using System.Collections.Concurrent;
using System.Text;

namespace WebMonk.Context;

public class PrefixManager : IPrefixManager
{
    #region EmbeddedTypes
    public class PrefixState
    {
        #region Constrollers
        public PrefixState(string prefix, object? parent, string? contextControllerName)
        {
            Prefix = prefix;
            Parent = parent;
            ContextControllerName = contextControllerName;
        }
        #endregion

        #region Properties
        public string Prefix { get; }
        public object? Parent { get; }
        public string? ContextControllerName { get; }
        #endregion
    }
        
    public class WebMonkPrefixEmptyCleaner : IDisposable
    {
        #region IDisposable implementation
        public void Dispose()
        {
            //do nothing
        }
        #endregion
    }
        
    public class WebMonkPrefixCleaner : IDisposable
    {
        #region Conastructors
        public WebMonkPrefixCleaner(ConcurrentStack<PrefixState> prefixesStack)
        {
            PrefixesStack = prefixesStack;
        }
        #endregion
            
        #region IDisposable implementation
        public void Dispose()
        {
            PrefixesStack.TryPop(out _);
        }
        #endregion

        #region Properties
        protected ConcurrentStack<PrefixState> PrefixesStack { get; }
        #endregion
    }
    #endregion
        
    #region Methods
    public IDisposable NewPrefix(string prefix, object? parent, string? controllerName = null)
    {
        if (string.IsNullOrEmpty(prefix)) return new WebMonkPrefixEmptyCleaner();

        PrefixesStack.Push(new PrefixState(prefix, parent, controllerName));
        return new WebMonkPrefixCleaner(PrefixesStack); 
    }
    #endregion

    #region Properties
    public string CurrentPrefix
    {
        get
        {
            var sb = new StringBuilder();
            var first = true;
            var prefixesArray = PrefixesStack.ToArray();
            for(var i = prefixesArray.Length - 1; i >= 0; i--)
            {
                var prefix = prefixesArray[i].Prefix;
                if (first) 
                {
                    sb.Append(prefix);
                    first = false;
                }
                else
                {
                    if (prefix.StartsWith("[") && prefix.EndsWith("]")) sb.Append(prefix);
                    else sb.Append($".{prefix}");
                }

            }
            return sb.ToString();
        }
    }
    public object? CurrentParent
    {
        get
        {
            var prefixesArray = PrefixesStack.ToArray();
            for(var i = 0; i < prefixesArray.Length; i++)
            {
                if (prefixesArray[i].Parent != null) return prefixesArray[i].Parent;
            }
            return null;
        }
    }

    public string CurrentContextControllerName
    {
        get
        {
            var prefixesArray = PrefixesStack.ToArray();
            for (var i = 0; i < prefixesArray.Length; i++)
            {
                if (prefixesArray[i].ContextControllerName != null) return prefixesArray[i].ContextControllerName!;
            }
            return HttpContext.Current.RouteManager.GetControllerFromRoute();
        }
    }

    public object? RootParent
    {
        get
        {
            var prefixesArray = PrefixesStack.ToArray();
            for(var i = prefixesArray.Length - 1; i >= 0; i--)
            {
                if (prefixesArray[i].Parent != null) return prefixesArray[i].Parent;
            }
            return null;
        }
    }
    protected ConcurrentStack<PrefixState> PrefixesStack { get; } = new();
    #endregion
}