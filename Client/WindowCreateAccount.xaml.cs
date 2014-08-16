using System;
using System.Windows;

namespace Client
{
    public partial class WindowCreateAccount : Window
    {
        MainWindow mainWindow;
        public class MissingDataException : Exception { }

        public WindowCreateAccount()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (MainWindow)this.Owner;
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (textBoxUsername.Text == "" || textBoxPassword.Text == "")
                    throw new MissingDataException();
                

                // todo: improve algorithm
                if (mainWindow.proxy.SignUp(textBoxUsername.Text, textBoxPassword.Text) == false)
                {
                    var username = textBoxUsername.Text;
                    var counter = 1;

                    username += counter.ToString();
                    for (counter = 0; counter < 65536; counter++)
                    {
                        username = username.Replace(counter.ToString(), (counter + 1).ToString());
                        if (mainWindow.proxy.SignUp(username, textBoxPassword.Text) == false) continue;
                        else break;
                    }

                    MessageBox.Show(textBoxUsername.Text + " is already used. You are registered as " + username, "Info");
                }


                this.Close();
            }

            catch (MissingDataException)
            {
                MessageBox.Show("You should fill in all the fields", "Error!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
        }
    }
}
