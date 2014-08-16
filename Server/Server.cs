using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Collections.Generic;
using MyLibrary;

namespace Server
{
    class Server: IServer
    {
        static internal Database database;

        public bool SignUp(string username, string password)
        {
            return database.SignUp(username, password);
        }

        public bool SignIn(string username, string password)
        {
            return database.SignIn(username, password);
        }

        public void SignOut(string username)
        {
            database.SignOut(username);
        }

        public List<string> GetOnlineMembers()
        {
            return database.GetOnlineMembers();
        }

        public void Send(string from, string to, string message)
        {     
            database.privateMessages.Add(new PrivateMessage(from, to, message));
        }

        public List<PrivateMessage> Receive(string to)
        {
            var privateMessages = new List<PrivateMessage>();
            //foreach (var message in database.privateMessages)
            //{
            //    if (message.To == to)
            //    {
            //        privateMessages.Add(new PrivateMessage(message.From, message.To, message.Text));
            //        database.privateMessages.Remove(message);
            //    }
            //}

            for (int i = 0; i < database.privateMessages.Count; i++)  
            {
                if (database.privateMessages[i].To == to)
                {
                    var message = new PrivateMessage(database.privateMessages[i].From, database.privateMessages[i].To, database.privateMessages[i].Text);
                    privateMessages.Add(message);
                    database.privateMessages.RemoveAt(i);
                }
            }
            return privateMessages;
        }

        public void SendToChat(string from, string message)
        {
            database.chatMessages.Add(new ChatMessage(from, message));
        }

        public List<ChatMessage> ReceiveFromChat(ChatMessage lastChatMessage)
        {
            var chatMessages = new List<ChatMessage>();

            var lastChatMessageIndex = -1; 
            foreach (var message in database.chatMessages)
            {
                if (message.From == lastChatMessage.From && message.Text == lastChatMessage.Text)
                {
                    lastChatMessageIndex = database.chatMessages.LastIndexOf(message);
                    break;
                }
            }  

            foreach (var message in database.chatMessages)
            {
                if (database.chatMessages.IndexOf(message) > lastChatMessageIndex)
                {
                    chatMessages.Add(message);
                }
            }

            return chatMessages;
        }
    }
}
