using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace MyLibrary
{
    [DataContract]
    public class PrivateMessage    
    {
        [DataMember] 
        public string From { get; set; }
        [DataMember]
        public string To { get; set; }
        [DataMember]
        public string Text { get; set; }

        public PrivateMessage(string from, string to, string text)
        {
            this.From = from;
            this.To = to;
            this.Text = text;
        }
    }

    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public string From { get; set; }
        [DataMember]
        public string Text { get; set; }

        public ChatMessage(string from, string text)
        {
            this.From = from;
            this.Text = text;
        }
    }


    [ServiceContract]
    public interface IServer
    {
        [OperationContract]
        bool SignUp(string username, string password);
        [OperationContract]
        bool SignIn(string username, string password);
        [OperationContract]
        void SignOut(string username);

        [OperationContract]
        List<string> GetOnlineMembers();

        [OperationContract]
        void Send(string from, string to, string message);
        [OperationContract]
        List<PrivateMessage> Receive(string to);
        [OperationContract]
        void SendToChat(string from, string message);
        [OperationContract]
        List<ChatMessage> ReceiveFromChat(ChatMessage lastChatMessage);
    }
}
