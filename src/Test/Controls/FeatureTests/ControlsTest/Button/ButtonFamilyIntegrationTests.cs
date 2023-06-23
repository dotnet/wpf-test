using System;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.IntegrationTests
{
    public static class ButtonFamilyTestHelper
    {
        public static void AddButtonFamilyTestContent(Panel rootPanel, XmlElement variation, ref int numberOfControls)
        {
            if (variation.Attributes["NumberOfControls"] != null)
                numberOfControls = int.Parse(variation.GetAttribute("NumberOfControls"), CultureInfo.InvariantCulture.NumberFormat);

            for (int i = 1; i <= numberOfControls; i++)
            {
                FrameworkElement control = (FrameworkElement)ObjectFactory.CreateObjectFromXaml(((XmlElement)variation["CONTROLS"].FirstChild).OuterXml);
                if (!(control is Menu) && !(control is Canvas))
                {
                    ((ContentControl)control).Content = Microsoft.Test.Globalization.Extract.GetTestString(0, 15);
                }
                rootPanel.Children.Add(control);
            }
        }
        public static void AddButtonFamilyTestControl(Panel rootPanel, XmlElement variation, ref int numberOfControls)
        {
            if (variation.Attributes["NumberOfControls"] != null)
                numberOfControls = int.Parse(variation.GetAttribute("NumberOfControls"), CultureInfo.InvariantCulture.NumberFormat);

            for (int i = 1; i <= numberOfControls; i++)
            {
                FrameworkElement control = (FrameworkElement)ObjectFactory.CreateObjectFromXaml(((XmlElement)variation["CONTROLS"].FirstChild).OuterXml);
                rootPanel.Children.Add(control);
            }
        }
    }

}



