using System;
using System.Xml;
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.ComponentModel;

using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.Actions;

using System.Windows.Media;

namespace Avalon.Test.ComponentModel.UnitTests
{
     /// <summary>
    /// test GroupStyle property for StatusBar
    /// </summary>
    class StatusBarGroupStyleTest : IUnitTest
    {
        private int[] counts = new int[3]{2, 1, 1};

        private const double c_Tolerance = 3; 

        public TestResult Perform(object testObject, XmlElement variation)
        {
            StackPanel sp = (StackPanel)testObject;
            StatusBar stbar = (StatusBar)sp.Children[0];

            if (stbar.IsGrouping == false)
            {
                GlobalLog.LogEvidence("stbar has wrong IsGrouping state");
                return TestResult.Fail;
            }

            System.Drawing.Rectangle[] rects = new System.Drawing.Rectangle[3];

            GroupItem[] items = (GroupItem[])(VisualTreeUtils.FindPartByType(stbar, typeof(GroupItem)).ToArray(typeof(GroupItem)));
            if (items == null || items.Length != 3)
            {
                GlobalLog.LogEvidence("Fail: GroupStyle can not work correctly for StatusBar");
                return TestResult.Fail;
            }
            for (int i = 0; i < items.Length; i++)
            {
                StatusBarItem[] sbi = (StatusBarItem[])(VisualTreeUtils.FindPartByType(items[i], typeof(StatusBarItem)).ToArray(typeof(StatusBarItem)));
                if (sbi == null || sbi.Length != counts[i])
                {
                    GlobalLog.LogEvidence("Fail: GroupItem do not have expected StatusBarItem");
                    return TestResult.Fail;
                }

                TextBlock tb = (TextBlock)(VisualTreeUtils.FindPartByType(items[i], typeof(TextBlock), 0));
                if (tb == null || tb.Text != i+1 + "")
                {
                    GlobalLog.LogEvidence("Fail: GroupStyle header can not work correctly");
                    return TestResult.Fail;
                }
                rects[i] = ImageUtility.GetScreenBoundingRectangle(tb);
            }

            if (rects[0].Left < rects[1].Left && rects[1].Left < rects[2].Left
                && Math.Abs(rects[0].Top - rects[1].Top) < c_Tolerance && Math.Abs(rects[2].Top - rects[1].Top) < c_Tolerance)
            {
                return TestResult.Pass;
            }

            GlobalLog.LogEvidence("Fail: GroupStyle can not work for StatusBar");
            return TestResult.Fail;
        }

    }

    /// <summary>
    /// test DisplayMemberPath property for StatusBar
    /// </summary>
    class StatusBarDisplayMemberPathTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar st = (StatusBar)testObject;

            TextBlock[] items = (TextBlock[])(VisualTreeUtils.FindPartByType(st, typeof(TextBlock)).ToArray(typeof(TextBlock)));

            if (items == null || items.Length != st.Items.Count)
            {
                GlobalLog.LogEvidence("Fail : can not find corrent textblock");
                return TestResult.Fail;
            }

            GlobalLog.LogStatus("DisplayMemberPath : " + st.DisplayMemberPath);
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Text != ((DateTime)st.Items[i]).Month.ToString())
                {
                    GlobalLog.LogStatus(items[i].Text);
                    GlobalLog.LogEvidence("Fail: DisplayMemberPath can not work correctly for Month");
                    return TestResult.Fail;
                }
            }

            st.DisplayMemberPath = "Day";
            QueueHelper.WaitTillQueueItemsProcessed();
            GlobalLog.LogStatus("DisplayMemberPath : " + st.DisplayMemberPath);
            items = (TextBlock[])(VisualTreeUtils.FindPartByType(st, typeof(TextBlock)).ToArray(typeof(TextBlock)));

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Text != ((DateTime)st.Items[i]).Day.ToString())
                {
                    GlobalLog.LogStatus(items[i].Text);
                    GlobalLog.LogEvidence("Fail: DisplayMemberPath can not work correctly for Day");
                    return TestResult.Fail;
                }
            }

            st.DisplayMemberPath = null;
            QueueHelper.WaitTillQueueItemsProcessed();
            GlobalLog.LogStatus("DisplayMemberPath : " + st.DisplayMemberPath);
            items = (TextBlock[])(VisualTreeUtils.FindPartByType(st, typeof(TextBlock)).ToArray(typeof(TextBlock)));
            
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Text != ((DateTime)st.Items[i]).ToShortDateString())
                {
                    GlobalLog.LogStatus("" + items[i].Text);
                    GlobalLog.LogStatus(((DateTime)st.Items[i]).ToShortDateString());
                    GlobalLog.LogEvidence("Fail: DisplayMemberPath can not work correctly (DisplayMemberPath is null)");
                    return TestResult.Fail;
                }
            }
            return TestResult.Pass;
        }
    }

    /// <summary>
    /// test HasItem property for StatusBar
    /// </summary>
    class StatusBarHasItemsTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar st = (StatusBar)testObject;
            st.Items.Add(new StatusBarItem());
            QueueHelper.WaitTillQueueItemsProcessed();

            if (st.HasItems == false)
            {
                GlobalLog.LogEvidence("st.HasItems should be true");
                return TestResult.Fail;
            }

            st.Items.Clear();
            QueueHelper.WaitTillQueueItemsProcessed();
            if (st.HasItems == true)
            {
                GlobalLog.LogEvidence("st.HasItems should be false");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }
    }

    /// <summary>
    /// test ItemTemplate property for StatusBar
    /// </summary>
    class StatusBarItemTemplateTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar stbar = (StatusBar)testObject;

            ArrayList arr = new ArrayList();
            arr.Add(new DateTime(2005, 5, 1));
            arr.Add(new DateTime(2005, 6, 2));
            arr.Add(new DateTime(2005, 6, 8));
            stbar.ItemsSource = arr;

            //create new ItemTemplate for ListView
            stbar.ItemTemplate = CreateCustomItemTemplate();
            QueueHelper.WaitTillQueueItemsProcessed();

            //validation
            ContentPresenter[] cps = (ContentPresenter[])(VisualTreeUtils.FindPartByType(stbar, typeof(ContentPresenter)).ToArray(typeof(ContentPresenter)));

            for (int i = 0; i < arr.Count; i++)
            {
                //find TextBlock to verify if the content equal the expected one 
                TextBlock tb = (TextBlock)(VisualTreeUtils.FindPartByType(cps[i], typeof(TextBlock), 0));
                if (tb == null)
                {
                    GlobalLog.LogEvidence("Fail:ItemTemplate can not work (TextBlock is null)!");
                    return TestResult.Fail;
                }

                GlobalLog.LogStatus("Displayed Text:" + tb.Text);
                GlobalLog.LogStatus("Expected  Text:" + ((DateTime)arr[i]).Day);

                //compare displayed text and expected text
                if (tb.Text != ((DateTime)arr[i]).Day.ToString())
                {
                    GlobalLog.LogEvidence("Current Value : " + tb.Text + " " + " Expected Value : " + ((DateTime)arr[i]).Day.ToString());
                    GlobalLog.LogEvidence("Fail:ItemTemplate can not work!");
                    return TestResult.Fail;
                }
            }

            GlobalLog.LogEvidence("Test pass: ItemTemplate work correctly on StatusBar");
            return TestResult.Pass;
        }

        private DataTemplate CreateCustomItemTemplate()
        {
            DataTemplate dt = new DataTemplate();

            //create border
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border), "Border");
            border.SetValue(Border.BorderBrushProperty, Brushes.LightGray);
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1));

            FrameworkElementFactory stackpanel = new FrameworkElementFactory(typeof(StackPanel), "StackPanel");

            //textblock for display content
            FrameworkElementFactory textblock = new FrameworkElementFactory(typeof(TextBlock), "TextBlock1");

            Binding bind = new Binding("Day");
            bind.Mode = BindingMode.OneWay;
            textblock.SetBinding(TextBlock.TextProperty, bind);

            stackpanel.AppendChild(textblock);

            border.AppendChild(stackpanel);

            dt.VisualTree = border;
            return dt;
        }
    }

    /// <summary>
    /// test ItemContainerStyle property for StatusBar
    /// </summary>
    class StatusBarItemContainerStyleTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar stbar = (StatusBar)testObject;

            StatusBarItem item = new StatusBarItem();
            item.Content = "StatusBarItem";
            stbar.Items.Add(item);

            GlobalLog.LogStatus("Change ItemContainerStyle to customized style");
            stbar.ItemContainerStyle = CreateItemContainerStyle();
            QueueHelper.WaitTillQueueItemsProcessed();

            if (item.BorderBrush != Brushes.Blue)
            {

            	GlobalLog.LogEvidence("BorderBrush " + item.BorderBrush + " should be Blue");
              return TestResult.Fail;
            }
            if (item.BorderThickness != new Thickness(2))
            {

            	GlobalLog.LogEvidence("BorderThickness " + item.BorderThickness + " should be (2,2,2,2)");
              return TestResult.Fail;
            }
            if (item.Background != Brushes.Red)
            {
              GlobalLog.LogEvidence("Background " + item.Background + " should be Red");
              return TestResult.Fail;
             }
            if (item.Foreground != Brushes.Green)
            {
            	GlobalLog.LogEvidence("Foreground " + stbar.Foreground + " should be Green");
            	return TestResult.Fail;
            }
            if (Math.Abs(item.Width - 200) > 2)
            {
            	GlobalLog.LogEvidence("Width " + item.Width + " should be 200");
            	return TestResult.Fail;
            }
            if (Math.Abs(item.Height - 45) > 2)
            { 
            	GlobalLog.LogEvidence("Height " + item.Height + " should be 45");
            	return TestResult.Fail;
            }
            if (Math.Abs(item.FontSize - 25) > 1)
            {
            	GlobalLog.LogEvidence("FontSize " + item.FontSize + " should be 25");
            	return TestResult.Fail;
            }
            if (item.FontWeight != FontWeights.Bold)
            {
            	GlobalLog.LogEvidence("FontWeight " + item.FontWeight + " should be Bold");
            	return TestResult.Fail;
            }

            if (item.Padding != new Thickness(6))
            {
                GlobalLog.LogEvidence("Padding " + item.Padding + " should be (6,6,6,6)");
                return TestResult.Fail;
            }

            GlobalLog.LogStatus("Change ItemContainerStyle to null");
            stbar.ItemContainerStyle = null;
            QueueHelper.WaitTillQueueItemsProcessed();

            if (item.BorderBrush == Brushes.Blue)
            {

            	GlobalLog.LogEvidence("BorderBrush " + item.BorderBrush + " should not be Blue");
              return TestResult.Fail;
            }
            if (item.BorderThickness == new Thickness(2))
            {

            	GlobalLog.LogEvidence("BorderThickness " + item.BorderThickness + " should not be (2,2,2,2)");
              return TestResult.Fail;
            }
            if (item.Background == Brushes.Red)
            {
              GlobalLog.LogEvidence("Background " + item.Background + " should not be Red");
              return TestResult.Fail;
             }
            if (item.Foreground == Brushes.Green)
            {
            	GlobalLog.LogEvidence("Foreground " + stbar.Foreground + " should not be Green");
            	return TestResult.Fail;
            }
            if (!Double.IsNaN(item.Width))
            {
            	GlobalLog.LogEvidence("Width " + item.Width + " should be NaN");
            	return TestResult.Fail;
            }
            if (!Double.IsNaN(item.Height))
            { 
            	GlobalLog.LogEvidence("Height " + item.Height + " should be NaN");
            	return TestResult.Fail;
            }
            if (Math.Abs(item.FontSize - 25) < 1)
            {
            	GlobalLog.LogEvidence("FontSize " + item.FontSize + " should not be 25");
            	return TestResult.Fail;
            }
            if (item.FontWeight == FontWeights.Bold)
            {
            	GlobalLog.LogEvidence("FontWeight " + item.FontWeight + " should not be Bold");
            	return TestResult.Fail;
            }

            if (item.Padding == new Thickness(6))
            {
                GlobalLog.LogEvidence("Padding " + item.Padding + " should not be (6,6,6,6)");
                return TestResult.Fail;
            }
            
            return TestResult.Pass;

        }

        private Style CreateItemContainerStyle()
        {
            Style style = new Style();
            style.TargetType = typeof(StatusBarItem);

            //set BorderBrush to Blue
            Setter borderBrushSetter = new Setter();
            borderBrushSetter.Property = StatusBarItem.BorderBrushProperty;
            borderBrushSetter.Value = Brushes.Blue;
            style.Setters.Add(borderBrushSetter);

            //set BorderThickness to (2,2,2,2)
            Setter borderThicknessSetter = new Setter();
            borderThicknessSetter.Property = StatusBarItem.BorderThicknessProperty;
            borderThicknessSetter.Value = new Thickness(2);
            style.Setters.Add(borderThicknessSetter);

            //set Background to Red
            Setter backgroundSetter = new Setter();
            backgroundSetter.Property = StatusBarItem.BackgroundProperty;
            backgroundSetter.Value = Brushes.Red;
            style.Setters.Add(backgroundSetter);

            //set Foreground to Green
            Setter foregroundSetter = new Setter();
            foregroundSetter.Property = StatusBarItem.ForegroundProperty;
            foregroundSetter.Value = Brushes.Green;
            style.Setters.Add(foregroundSetter);

            //set Width to 200
            Setter widthSetter = new Setter();
            widthSetter.Property = StatusBarItem.WidthProperty;
            widthSetter.Value = 200d;
            style.Setters.Add(widthSetter);

            //set Height to 45
            Setter heightSetter = new Setter();
            heightSetter.Property = StatusBarItem.HeightProperty;
            heightSetter.Value = 45d;
            style.Setters.Add(heightSetter);

            //set FontSize to 25
            Setter fontSizeSetter = new Setter();
            fontSizeSetter.Property = StatusBarItem.FontSizeProperty;
            fontSizeSetter.Value = 25d;
            style.Setters.Add(fontSizeSetter);

            //set FontWeight to Bold
            Setter fontWeightSetter = new Setter();
            fontWeightSetter.Property = StatusBarItem.FontWeightProperty;
            fontWeightSetter.Value = FontWeights.Bold;
            style.Setters.Add(fontWeightSetter);

            //set Padding to (6,6,6,6)
            Setter paddingSetter = new Setter();
            paddingSetter.Property = StatusBarItem.PaddingProperty;
            paddingSetter.Value = new Thickness(6);
            style.Setters.Add(paddingSetter);

            return style;
        }
    }

    /// <summary>
    /// test Item Scroll string scenario for StatusBar
    /// </summary>
    class StatusBarItemScrollStringTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar stbar = (StatusBar)testObject;

            ScrollString str = new ScrollString();
            str.OriginContent = "scrolling text";
            GlobalLog.LogStatus("OriginContent : " + str.OriginContent);


            StatusBarItem item = new StatusBarItem();
            Binding bind = new Binding("Content");
            bind.Source = str;
            bind.Mode = BindingMode.OneWay;
            item.SetBinding(StatusBarItem.ContentProperty, bind);

            GlobalLog.LogStatus("create binding and set it to StatusBarItem's Content");
            stbar.Items.Add(item);
            QueueHelper.WaitTillQueueItemsProcessed();

            GlobalLog.LogStatus("scroll text first");
            str.Scroll();
            QueueHelper.WaitTillQueueItemsProcessed();

            if (item.Content.ToString() != "crolling text   s")
            {
                GlobalLog.LogEvidence("Current Value : " + item.Content);
                GlobalLog.LogEvidence("Expected Value : crolling text   s");
                return TestResult.Fail;
            }

            GlobalLog.LogStatus("scroll text again");
            str.Scroll();
            QueueHelper.WaitTillQueueItemsProcessed();

            if (item.Content.ToString() != "rolling text   sc")
            {
                GlobalLog.LogEvidence("Content : " + item.Content);
                GlobalLog.LogEvidence("Expected Value : rolling text   sc");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }

    /// <summary>
    /// used to scroll string, user can use it to scroll text just like IExplorer
    /// </summary>
    class ScrollString : INotifyPropertyChanged
    {
        //current content after scrolling
        private string _strContent = null;

        //original string for scrolling
        private string _strOrigin = null;

        //if Content changed, if event will be fired
        //public event EventHandler _dataChanged;

        //new property for getting string for scrolling
        public string OriginContent
        {
            get
            {
                return _strOrigin;
            }
            set
            {
                _strOrigin = value;
                _strContent = _strOrigin + "   ";
            }
        }

        //new property, content for every scroll
        public string Content
        {
            get
            {
                return _strContent;
            }
        }

        public void Scroll()
        {
            if (_strContent == null)
            {
                return;
            }
            _strContent = _strContent.Substring(1) + _strContent[0];

            if (_propertyChanged != null)
            {
                PropertyChangedEventArgs arg = new PropertyChangedEventArgs("Content");
                _propertyChanged(this, arg);
            }
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                _propertyChanged += value;
            }
            remove
            {
                _propertyChanged -= value;
            }
        }

        private event PropertyChangedEventHandler _propertyChanged;
    }

    /// <summary>
    /// test ItemTemplateSelector property for StatusBar
    /// </summary>
    class StatusBarItemTemplateSelectorTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar stbar = (StatusBar)testObject;

            ArrayList arr = new ArrayList();
            arr.Add(new DateTime(2005, 4, 2));
            arr.Add(new DateTime(2005, 4, 3));
            arr.Add(new DateTime(2005, 4, 4));
            stbar.ItemsSource = arr;

            stbar.ItemTemplateSelector = new SbarItemSelector();
            QueueHelper.WaitTillQueueItemsProcessed();

            TextBlock[] tbs = (TextBlock[])(VisualTreeUtils.FindPartByType(stbar, typeof(TextBlock)).ToArray(typeof(TextBlock)));

            if (tbs == null || tbs.Length != arr.Count)
            {
                GlobalLog.LogEvidence("Fail : Can not find testblock in statusbar");
                return TestResult.Fail;
            }

            for (int i = 0; i < tbs.Length; i++)
            {
                if (tbs[i].Text != ((DateTime)arr[i]).Day.ToString())
                {
                    GlobalLog.LogEvidence("TextBlock has wrong text : " + tbs[i].Text);
                    GlobalLog.LogEvidence("Expected Value : " + ((DateTime)arr[i]).Day.ToString());
                    return TestResult.Fail;
                }
                DayOfWeek dw = ((DateTime)arr[i]).DayOfWeek;
                if (dw == DayOfWeek.Sunday)
                {
                    if (tbs[i].Foreground != Brushes.Red)
                    {
                        GlobalLog.LogEvidence("TextBlock has wrong Foreground color (should be red color)");
                        return TestResult.Fail;
                    }
                }
                else if (dw == DayOfWeek.Saturday)
                {
                    if (tbs[i].Foreground != Brushes.Green)
                    {
                        GlobalLog.LogEvidence("TextBlock has wrong Foreground color (should be green color)");
                        return TestResult.Fail;
                    }
                }
                else
                {
                    if (tbs[i].Foreground != Brushes.Black)
                    {
                        GlobalLog.LogEvidence("TextBlock has wrong Foreground color (should be black color)");
                        return TestResult.Fail;
                    }
                }
            }

            return TestResult.Pass;

        }
    }

    /// <summary>
    /// create customized ItemTemplateSelector for StatusBar
    /// </summary>
    class SbarItemSelector : DataTemplateSelector
    {
        /// <summary>
        /// create new DataTemplate
        /// </summary>
        /// <param name="item">DateTime</param>
        /// <param name="container">ContentPresenter</param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DateTime dt = (DateTime)item;

            DataTemplate dtemplate = new DataTemplate();

            FrameworkElementFactory tb1 = new FrameworkElementFactory(typeof(TextBlock), "tb1");
            Binding bind = new Binding("Day");
            tb1.SetBinding(TextBlock.TextProperty, bind);

            if (dt.DayOfWeek == DayOfWeek.Sunday)
            {//Sunday Style
                tb1.SetValue(TextBlock.ForegroundProperty, Brushes.Red);
            }
            else if (dt.DayOfWeek == DayOfWeek.Saturday)
            {//SaturDay Style
                tb1.SetValue(TextBlock.ForegroundProperty, Brushes.Green);
            }
            else
            {//Other day Style
                tb1.SetValue(TextBlock.ForegroundProperty, Brushes.Black);
            }

            dtemplate.VisualTree = tb1;

            return dtemplate;
        }
    }
    
     /// <summary>
    /// Test ItemContainerStyleSelector property 
    /// </summary>
    class StatusBarItemContainerStyleSelectorTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar stbar = (StatusBar)testObject;

            ArrayList arr = new ArrayList();
            arr.Add(new DateTime(2005, 4, 1));
            arr.Add(new DateTime(2005, 4, 2));
            arr.Add(new DateTime(2005, 4, 3));
            arr.Add(new DateTime(2005, 4, 4));
            stbar.ItemsSource = arr;

            GlobalLog.LogStatus("change ItemContainerStyleSelector to customized one");
            stbar.ItemContainerStyleSelector = new SBarItemStyleSelector();
            QueueHelper.WaitTillQueueItemsProcessed();

            StatusBarItem[] items = (StatusBarItem[])(VisualTreeUtils.FindPartByType(stbar, typeof(StatusBarItem)).ToArray(typeof(StatusBarItem)));
            if (items == null || items.Length != 4)
            {
                GlobalLog.LogEvidence("Fail : Can not find StatusBarItem");
                return TestResult.Fail;
            }

            for (int i = 0; i < items.Length; i++)
            {
                DayOfWeek dw = DateTime.Parse(items[i].Content.ToString()).DayOfWeek;
                GlobalLog.LogStatus("DayOfWeek : " + dw.ToString());
                if (dw == DayOfWeek.Sunday)
                {
                    if (items[i].Background != Brushes.Green)
                    {
                        GlobalLog.LogEvidence("Fail: Background should be Green, current is " + items[i].Background);
                        return TestResult.Fail;
                    }
                    if (items[i].Foreground != Brushes.Red)
                    {
                        GlobalLog.LogEvidence("Fail: Foreground should be Red, current is " + items[i].Foreground);
                        return TestResult.Fail;
                    }
                }
                else if (dw == DayOfWeek.Saturday)
                {
                    if (items[i].Background != Brushes.Green)
                    {
                        GlobalLog.LogEvidence("Fail: Background should be Green, current is " + items[i].Background);
                        return TestResult.Fail;
                    }
                    if (items[i].Foreground != Brushes.Yellow)
                    {
                        GlobalLog.LogEvidence("Fail: Foreground should be Yellow, current is " + items[i].Foreground);
                        return TestResult.Fail;
                    }
                }
                else
                {
                    if (items[i].Background != Brushes.White)
                    {
                        GlobalLog.LogEvidence("Fail: Background should be White, current is " + items[i].Background);
                        return TestResult.Fail;
                    }
                    if (items[i].Foreground != Brushes.Black)
                    {
                        GlobalLog.LogEvidence("Fail: Foreground should be Black, current is " + items[i].Foreground);
                        return TestResult.Fail;
                    }
                }
            }

            GlobalLog.LogStatus("change ItemContainerStyleSelector to null");
            stbar.ItemContainerStyleSelector = null;
            QueueHelper.WaitTillQueueItemsProcessed();

            //change ItemContainerStyleSelector will regenerate item
            items = (StatusBarItem[])(VisualTreeUtils.FindPartByType(stbar, typeof(StatusBarItem)).ToArray(typeof(StatusBarItem)));
            if (items == null || items.Length != 4)
            {
                GlobalLog.LogEvidence("Fail : Can not find StatusBarItem");
                return TestResult.Fail;
            }

            for (int i = 0; i < items.Length; i++)
            {
                if (((SolidColorBrush)items[i].Foreground).Color != SystemColors.ControlTextBrush.Color)
                {
                    GlobalLog.LogEvidence("current Foreground value : " + items[i].Foreground + " should be " + SystemColors.ControlTextBrush);
                    return TestResult.Fail;
                }
                if (((SolidColorBrush)items[i].Background).Color != Brushes.Transparent.Color)
                {
                    GlobalLog.LogEvidence("current Background value : " + items[i].Background + " should be " + Brushes.Transparent);
                    return TestResult.Fail;
                }
            }


            return TestResult.Pass;
        }
    }

    /// <summary>
    /// create customised StyleSelector
    /// </summary>
    class SBarItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            DateTime dt = (DateTime)item;

            Style st = new Style();
            st.TargetType = typeof(StatusBarItem);

            // change Foreground property
            Setter foreGroundSetter = new Setter();
            foreGroundSetter.Property = StatusBarItem.ForegroundProperty;

            //change Background property
            Setter backGroundSetter = new Setter();
            backGroundSetter.Property = StatusBarItem.BackgroundProperty;

            switch (dt.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    foreGroundSetter.Value = Brushes.Red;
                    backGroundSetter.Value = Brushes.Green;
                    break;

                case DayOfWeek.Saturday:
                    foreGroundSetter.Value = Brushes.Yellow;
                    backGroundSetter.Value = Brushes.Green;
                    break;

                case DayOfWeek.Monday:      //fall through
                case DayOfWeek.Tuesday:     //fall through
                case DayOfWeek.Wednesday:   //fall through
                case DayOfWeek.Thursday:    //fall through
                case DayOfWeek.Friday:      //fall through
                    foreGroundSetter.Value = Brushes.Black;
                    backGroundSetter.Value = Brushes.White;
                    break;

                default:
                    throw new NotSupportedException("dt.DayOfWeek");
            }

            st.Setters.Add(foreGroundSetter);
            st.Setters.Add(backGroundSetter);

            return st;
        }
    }

    /// <summary>
    /// test below scenario:
    /// when StatusBar has Separator in it, the ItemContainerStyleSelector should not affect Separator
    /// </summary>
    class StatusBarItemContainerStyleSelectorWithSeparatorTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar sbar = (StatusBar)testObject;

            try
            {
                GlobalLog.LogStatus("Try to change ItemContainerStyleSelector");
                sbar.ItemContainerStyleSelector = new SBarItemStyleSelector2();
                GlobalLog.LogStatus("Finished to change ItemContainerStyleSelector");
                return TestResult.Pass;
            }
            catch (SeparatorException)
            {
                GlobalLog.LogEvidence("Fail: Catch SeparatorException");
                return TestResult.Fail;
            }
        }
    }

    /// <summary>
    /// test below scenario:
    /// when StatusBar has Separator in it, the ItemTemplateSelector should not affect Separator and StatusBarItem
    /// </summary>
    class StatusBarItemTemplateSelectorWithSeparatorTest : IUnitTest
    {
        public TestResult Perform(object testObject, XmlElement variation)
        {
            StatusBar sbar = (StatusBar)testObject;

            try
            {
                GlobalLog.LogStatus("Try to change ItemTemplateSelector");
                sbar.ItemTemplateSelector = new SbarItemSelector2();
                GlobalLog.LogStatus("Finished to change ItemTemplateSelector");
                return TestResult.Pass;
            }
            catch (SeparatorException)
            {
                GlobalLog.LogEvidence("Fail: Catch SeparatorException");
                return TestResult.Fail;
            }            
        }
    }

    class SBarItemStyleSelector2 : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is Separator)
            {
                throw new SeparatorException("StyleSelector should not affect Separator");
            }
            return new Style();
        }
    }

    class SbarItemSelector2 : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Separator || item is StatusBarItem)
            {
                throw new SeparatorException("DataTemplateSelector should not affect Separator or StatusBarItem");
            }
            return new DataTemplate();
        }
    }

    class SeparatorException : Exception
    {
        public SeparatorException(string str)
            : base(str)
        {
        }
    }
}
