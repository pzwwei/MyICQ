using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Linq;
using MyLibrary;

namespace Server
{
    [Serializable]
    class Database
    {
        class AccountField 
        {
            internal string password;
            internal bool isLoggedIn;

            public AccountField(string password, bool isLoggedIn)
            {
                this.password = password;
                this.isLoggedIn = isLoggedIn;
            }
        }

        private Dictionary<string, AccountField> database; 
        internal List<PrivateMessage> privateMessages = new List<PrivateMessage>();
        internal List<ChatMessage> chatMessages = new List<ChatMessage>();

        internal void Load()
        {
            var accounts = XDocument.Load("accounts.xml");
            database = new Dictionary<string, AccountField>();
            foreach (var account in accounts.Descendants("account"))
            {
                var key = account.Attribute("name").Value;
                var value = new AccountField(null, false);
                value.password = account.Attribute("password").Value;
                value.isLoggedIn = false;
                database.Add(key, value);
            }
        }

        internal void Save()
        {   
            var xaccounts = new XDocument();  //rewrite completely. todo: improve
            xaccounts.Add(new XElement("accounts")); 
            foreach (var account in database)
            {
                xaccounts.Element("accounts").Add(new XElement("account", new XAttribute("name", account.Key), new XAttribute("password", account.Value.password)));
            }
            xaccounts.Save("accounts.xml");
        } 

        internal bool SignUp(string username, string password)
        {
            if (database.ContainsKey(username))
            {
                return false;
            }

            var account =  new AccountField(password, false);
            database.Add(username, account);
            this.Save();
            return true;
        }

        internal bool SignIn(string username, string password)
        {
            if (database.ContainsKey(username))
            {
                if (database[username].isLoggedIn == true)
                {
                    return false;
                }

                else if (database[username].password == password)
                {
                    database[username].isLoggedIn = true;
                    Console.WriteLine(username + " has signed in");
                    return true;
                }

                else
                {
                    return false;
                }
            }

            else
            {
                return false;
            }
        }

        internal void SignOut(string username)
        {   
            database[username].isLoggedIn = false;
            Console.WriteLine(username + " has signed out");
        }

        internal List<string> GetOnlineMembers()
        {
            var onlineMembers = new List<string>();
            foreach (var username in database.Keys)
            {
                if (database[username].isLoggedIn == true)
                {
                    onlineMembers.Add(username);
                }
            }
            return onlineMembers;
        }
    }
}
