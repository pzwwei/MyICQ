using System;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;
using MyLibrary;
using System.ServiceModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace Client
{
    public partial class WindowChat : Window
    {
        DispatcherTimer chatMessagesTimer;
        DispatcherTimer privateMessagesTimer;
        DispatcherTimer onlineMembersTimer;
        MainWindow mainWindow;
        internal Dictionary<string, WindowDialog> currentConversations; 

        public WindowChat()
        {
            InitializeComponent();
        }

     
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)this.Owner;
            this.Title += "you are " + mainWindow.username;
            currentConversations = new Dictionary<string, WindowDialog>();

            var chatMessages = mainWindow.proxy.ReceiveFromChat(new ChatMessage(null, null));
            foreach (var message in chatMessages)
            {
                listBoxChat.Items.Add(message.From + ": " + message.Text);
            }


            var onlineMembers = mainWindow.proxy.GetOnlineMembers();
            foreach (var member in onlineMembers)                  
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = member;
                listBoxItem.MouseDoubleClick += this.listBoxItem_MouseDoubleClick;  
                listBoxContactlist.Items.Add(listBoxItem);
            }


            ControlTimers();
        }

        // extract a method
        private void ControlTimers()
        {
            chatMessagesTimer = new DispatcherTimer();
            chatMessagesTimer.Tick += new EventHandler(chatMessagesTimer_Tick);
            chatMessagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            chatMessagesTimer.Start();

            privateMessagesTimer = new DispatcherTimer();
            privateMessagesTimer.Tick += new EventHandler(privateMessagesTimer_Tick);
            privateMessagesTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            privateMessagesTimer.Start();

            onlineMembersTimer = new DispatcherTimer();
            onlineMembersTimer.Tick += new EventHandler(onlineMembersTimer_Tick);
            onlineMembersTimer.Interval = new TimeSpan(0, 0, 0, 0, 300);
            onlineMembersTimer.Start();
        }

        private void listBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var newConversation = ((ListBoxItem)sender).Content.ToString(); 
            if (newConversation == mainWindow.username) 
            {
                MessageBox.Show("You can't talk to yourself", "Error!");
                return;
            }

            var windowDialog = new WindowDialog();
            this.currentConversations.Add(newConversation, windowDialog); 

            windowDialog.Owner = this;
            windowDialog.Show();
        }

        private void buttonSend_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.proxy.SendToChat(mainWindow.username, textBoxMessage.Text);
            textBoxMessage.Clear();
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonSend_Click(sender, e);
            }
        }

        private void chatMessagesTimer_Tick(object sender, EventArgs e)
        {
            ChatMessage lastMessage;
            if (listBoxChat.Items.Count == 0) lastMessage = new ChatMessage(null, null);  //if still empty chat
            else
            {
                var lastItem = listBoxChat.Items[listBoxChat.Items.Count - 1];
                string[] split = lastItem.ToString().Split(new string[] { ": " }, 2, StringSplitOptions.None);
                lastMessage = new ChatMessage(split[0], split[1]);
            }


            var chatMessages = mainWindow.proxy.ReceiveFromChat(lastMessage);
            foreach (var message in chatMessages)
            {
                listBoxChat.Items.Add(message.From + ": " + message.Text);
            }
        }


        private void privateMessagesTimer_Tick(object sender, EventArgs e)
        {
            var privateMessages = mainWindow.proxy.Receive(mainWindow.username);
            foreach (var message in privateMessages)
            {
                if (!this.currentConversations.ContainsKey(message.From)) 
                {                                                               
                    var newConversation = message.From;
                    var windowDialog = new WindowDialog();
                    this.currentConversations.Add(newConversation, windowDialog);

                    windowDialog.Owner = this;
                    windowDialog.Show();
                }

                this.currentConversations[message.From].listBoxConversation.Items.Add(message.From + ": " + message.Text);
            }
        }

        private void onlineMembersTimer_Tick(object sender, EventArgs e)
        {
            listBoxContactlist.Items.Clear();

            var onlineMembers = mainWindow.proxy.GetOnlineMembers();
            foreach (var member in onlineMembers)
            {
                var listBoxItem = new ListBoxItem();
                listBoxItem.Content = member;
                listBoxItem.MouseDoubleClick += this.listBoxItem_MouseDoubleClick;
                listBoxContactlist.Items.Add(listBoxItem);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {    
            mainWindow.proxy.SignOut(mainWindow.username);

            foreach (var window in this.OwnedWindows) 
            {
                ((Window)window).Close();
            }
            mainWindow.Show();      
        }
    }
}
