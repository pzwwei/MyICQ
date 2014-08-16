using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace Client
{
    public partial class WindowDialog : Window
    {
        MainWindow mainWindow;
        WindowChat windowChat;
        string currentConversation; 

        public WindowDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            windowChat = (WindowChat)this.Owner;
            mainWindow = (MainWindow)this.Owner.Owner;
            currentConversation = windowChat.currentConversations.Last().Key;  
            this.Title += currentConversation;  
        }

        private void ButtonSend_Click(object sender, RoutedEventArgs e)
        {
            listBoxConversation.Items.Add(mainWindow.username + ": " + textBoxMessage.Text);
            mainWindow.proxy.Send(mainWindow.username, currentConversation, textBoxMessage.Text);            
            textBoxMessage.Clear();       
        }

        private void textBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ButtonSend_Click(sender, e);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            windowChat.currentConversations.Remove(currentConversation);
        }
    }
}
