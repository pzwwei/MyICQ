using System;
using System.Windows;
using System.ServiceModel;
using MyLibrary;

namespace Client
{
    public partial class MainWindow : Window
    {
        internal IServer proxy;
        internal string username;
        public class UsernamePasswordMismatchException : Exception { }
        public class ConnectingException : Exception { }

        public MainWindow()
        {
            InitializeComponent();
        }

        private bool Connect()
        {
            try
            {
                var address = new EndpointAddress("net.tcp://" + textBoxIP.Text + ":8000/Server");
                var binding = new NetTcpBinding();
                binding.Security.Mode = SecurityMode.None;
                proxy = ChannelFactory<IServer>.CreateChannel(binding, address);

                return true;
            }

            catch (Exception) 
            {                
                MessageBox.Show("Unable to connect with the server", "Error!");
                return false;
            }
        }

        private void buttonSignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Connect() == false)
                {
                    throw new ConnectingException();
                }

                if (proxy.SignIn(textBoxUsername.Text, textBoxPassword.Text) == false)
                {
                    throw new UsernamePasswordMismatchException();
                }

                username = textBoxUsername.Text;

                var windowChat = new WindowChat();
                windowChat.Owner = this;
                windowChat.Show();

                this.Hide();
            }

            catch (ConnectingException)
            {
            }

            catch (UsernamePasswordMismatchException)
            {
                MessageBox.Show("Username and password mismatch or the contact doesn't exist or you have already entered", "Error!");
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Connect() == false)
                {
                    throw new ConnectingException();
                }

                var windowCreateAccount = new WindowCreateAccount();
                windowCreateAccount.Owner = this;
                windowCreateAccount.Show();
            }

            catch (ConnectingException)
            {
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
