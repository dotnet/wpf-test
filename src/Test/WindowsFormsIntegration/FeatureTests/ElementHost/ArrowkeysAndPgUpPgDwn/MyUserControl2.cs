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
    public class UserControl2 : UserControl
    {
        public System.Windows.Controls.Button bt1 = new Button();
        public System.Windows.Controls.Button bt2 = new Button();
        public System.Windows.Controls.Button bt3 = new Button();
        System.Windows.Controls.Grid gr = new Grid();

        public UserControl2()
        {
            gr.Background = Brushes.Orange;
            this.Loaded += new RoutedEventHandler(UserControlLoaded);
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {
            gr.ColumnDefinitions.Add(new ColumnDefinition());
            gr.RowDefinitions.Add(new RowDefinition());
            gr.RowDefinitions.Add(new RowDefinition());
            gr.RowDefinitions.Add(new RowDefinition());
            Grid.SetRow(bt1, 0);
            Grid.SetRow(bt2, 1);
            Grid.SetRow(bt3, 2);
            bt1.Content = "Button 1";
            bt1.Name = "AVButton1";
            gr.Children.Add(bt1);
            bt2.Content = "Button 2";
            bt2.Name = "AVButton2";
            gr.Children.Add(bt2);
            bt3.Content = "Button 3";
            bt3.Name = "AVButton3";
            gr.Children.Add(bt3);
            this.AddChild(gr);
            bt1.TabIndex = 0;
            bt2.TabIndex = 1;
            bt3.TabIndex = 2;
        }
    }
}