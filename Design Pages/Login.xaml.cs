using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JuicyReddit
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Login
    {
        private string url = "http://www.reddit.com/api/login";
        WebClient wc;

        public Login()
        {
            this.InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            wc = new WebClient();

            string userName = UserNameTextBox.Text.ToString();
            string password = PasswordTextbox.Password.ToString();

            User.RootObject userResponse = await wc.Login(url,userName,password);

            //if (String.IsNullOrEmpty(userResponse.data.name))
            //{
            //    MessageDialog md = new MessageDialog("Wrong username or password, please try again");
            //}
            //else
                this.Frame.Navigate(typeof(MainPage));

        }
    }
}
