using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Supermodel.Presentation.Mvc.Bootstrap4.Models;

public static partial class Bs4
{ 
    public readonly struct Message
    {
        #region Constructors
        public Message(string messageText, bool isSingleRead)
        {
            MessageText = messageText;
            IsSingleRead = isSingleRead;
        }
        #endregion

        #region Properties
        public string MessageText { get; }
        public bool IsSingleRead { get; }
        #endregion

        #region Static Members
        public static string RegisterSingleReadMessageAndGetUrl(string messageText)
        { 
            return RegisterMessageAndGetUrl(messageText, true);
        }
        public static string RegisterMultiReadMessageAndGetUrl(string messageText)
        { 
            return RegisterMessageAndGetUrl(messageText, false);
        }

        public static Guid RegisterSingleReadMessage(string messageText)
        {
            return RegisterMessage(messageText, true);
        }
        public static Guid RegisterMultiReadMessage(string messageText)
        {
            return RegisterMessage(messageText, false);
        }

        public static void UnRegisterMessage(Guid guid)
        {
            if (!Messages.ContainsKey(guid)) throw new Exception("Guid does not exist in Messages dictionary");
            Messages.Remove(guid, out _);
        }

        public static string ReadMessageText(string textGuid)
        {
            return ReadMessageText(Guid.Parse(textGuid));
        }
        public static string ReadMessageText(Guid guid)
        {
            if (!Messages.ContainsKey(guid)) throw new Exception("Guid does not exist in Messages dictionary");
            var message = Messages[guid];
            if (message.IsSingleRead) UnRegisterMessage(guid);
            return message.MessageText;
        }
            
        private static string RegisterMessageAndGetUrl(string messageText, bool isSingleRead)
        {
            return $"/static_web_files/Message.html?msgGuid={RegisterMessage(messageText, isSingleRead)}";
        }
        private static Guid RegisterMessage(string messageText, bool isSingleRead)
        {
            var guid = Guid.NewGuid();
            var message = new Message(messageText, isSingleRead);
            Messages[guid] = message;
            return guid;
        }
            
        private static ConcurrentDictionary<Guid, Message> Messages { get; }  = new();
        #endregion
    }
}