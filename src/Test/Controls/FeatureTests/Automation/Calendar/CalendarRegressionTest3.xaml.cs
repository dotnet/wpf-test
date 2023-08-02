using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Forms.Integration;

namespace Microsoft.Test.Controls
{
    public partial class CalendarRegressionTest3 : Page
    {
        public CalendarRegressionTest3()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (Type type in controlTypes)
            {
                if (typeof(UIElement).IsAssignableFrom(type) &&
                    !type.ContainsGenericParameters)
                {
                    var cons = type.GetConstructor(new Type[] { });
                    if (cons != null)
                    {
                        var element = cons.Invoke(null) as UIElement;
                        element.Visibility = Visibility.Collapsed;
                        if (!(element is Window) &&
                            !(element is ContextMenu) &&
                            !(element is AdornedElementPlaceholder) &&
                            (type != typeof(Page)) &&
                            (type != typeof(ToolTip)))
                        {
                            panel.Children.Add(element);
                        }
                    }
                }
            }
        }

        Type[] controlTypes = new Type[] {  typeof(Button),
                                            typeof(Calendar),
                                            typeof(CheckBox),
                                            typeof(ComboBox),
                                            typeof(DataGrid),
                                            typeof(DatePicker),
                                            typeof(DocumentViewer),
                                            typeof(Expander),
                                            typeof(Frame),
                                            typeof(GridSplitter),
                                            typeof(GroupBox),
                                            typeof(Image),
                                            typeof(Label),
                                            typeof(ListBox),
                                            typeof(ListView),
                                            typeof(MediaElement),
                                            typeof(Menu),
                                            typeof(PasswordBox),
                                            typeof(ProgressBar),
                                            typeof(RadioButton),
                                            typeof(RichTextBox),
                                            typeof(ScrollBar),
                                            typeof(ScrollViewer),
                                            typeof(Separator),
                                            typeof(Slider),
                                            typeof(StatusBar),
                                            typeof(TabControl),
                                            typeof(TextBlock),
                                            typeof(TextBox),
                                            typeof(ToolBar),
                                            typeof(TreeView),
                                            typeof(WindowsFormsHost),
                                            typeof(ComboBoxItem),
                                            typeof(ContextMenu),
                                            typeof(FlowDocument),
                                            typeof(FlowDocumentPageViewer),
                                            typeof(FlowDocumentScrollViewer),
                                            typeof(FlowDocumentReader),
                                            typeof(GridView),
                                            typeof(GridViewColumn),
                                            typeof(GridViewColumnHeader),
                                            typeof(GridViewHeaderRowPresenter),
                                            typeof(GridViewRowPresenter),
                                            typeof(GroupItem),
                                            typeof(InkCanvas),
                                            typeof(InkPresenter),
                                            typeof(ListBoxItem),
                                            typeof(ListViewItem),
                                            typeof(MenuItem),
                                            typeof(TabItem),
                                            typeof(ToolTip),
                                            typeof(TreeViewItem)
        };
    }  
}
