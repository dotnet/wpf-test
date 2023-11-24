using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyUserControl
{
    public class UserControl1 : UserControl
    {
        public System.Windows.Controls.Button bt1 = new Button();
        public System.Windows.Controls.Button bt2 = new Button();
        public System.Windows.Controls.Button bt3 = new Button();
        System.Windows.Controls.StackPanel sp = new StackPanel();

        public UserControl1()
        {
            sp.Background = Brushes.Orange;
            this.Loaded += new RoutedEventHandler(UserControlLoaded);
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            bt1.Content = "Button 1";
            bt1.Name = "AVButton1";
            bt2.Content = "Button 2";
            bt2.Name = "AVButton2";
            bt3.Content = "Button 3";
            bt3.Name = "AVButton3";
            sp.Children.Add(bt1);
            sp.Children.Add(bt2);
            sp.Children.Add(bt3);
            this.AddChild(sp);
            bt1.TabIndex = 0;
            bt2.TabIndex = 1;
            bt3.TabIndex = 2;
        }
    }
}