// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.Logging;
using System.Windows.Media;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Misc
{
    /// <summary/>
    public static class TextRecord_Verify
    {
        /// <summary>
        /// Verify the text in the button.
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            bool result = true;

            ContentControl[] elements = new ContentControl[21];
            for (int i = 0; i < 21; i++)
            {
                elements[i] = (ContentControl) LogicalTreeHelper.FindLogicalNode(rootElement, "Element" + i.ToString());
            }

            ComboBox elem8ComboBox  = elements[8].Content as ComboBox;
            ComboBox elem9ComboBox  = elements[9].Content as ComboBox;
            ComboBox elem10ComboBox = elements[10].Content as ComboBox;

            ItemCollection elem8Children  = elem8ComboBox.Items;
            ItemCollection elem9Children  = elem9ComboBox.Items;
            ItemCollection elem10Children = elem10ComboBox.Items;

            /*
            



*/

            if (!(((String) elements[0].Content).Equals("उ नीहरू किन नेपाली मात्र बोल्न सक्दैनन् ?")))
            {
                GlobalLog.LogStatus("elements[0] did not match");
                result = false;
            }
            if (!(((String) elements[1].Content).Equals("な ぜ、みんな日本語を話してくれないのか？")))
            {
                GlobalLog.LogStatus("elements[1] did not match");
                result = false;
            }
            if (!(((String) elements[2].Content).Equals("ಅ ವರು ಕನ್ನಡ ಮಾತನಾಡಬಹುದಲ್ಲಾ?")))
            {
                GlobalLog.LogStatus("elements[2] did not match");
                result = false;
            }
            if (!(((String) elements[3].Content).Equals("ทำ ไมเขาถึงไม่พูดภาษาไทย")))
            {
                GlobalLog.LogStatus("elements[3] did not match");
                result = false;
            }
            if (!(((String) elements[4].Content).Equals("他 们为什么不说中文（中国）？")))
            {
                GlobalLog.LogStatus("elements[4] did not match");
                result = false;
            }
            if (!(((String) elements[5].Content).Equals("Hey \\n\\x Dude")))
            {
                GlobalLog.LogStatus("elements[5] did not match");
                result = false;
            }
            if (!(((String) elements[6].Content).Equals("Hey dear Dude")))
            {
                GlobalLog.LogStatus("elements[6] did not match");
                result = false;
            }
            if (!(((String) elements[7].Content).Equals("Hey Dude")))
            {
                GlobalLog.LogStatus("elements[7] did not match");
                result = false;
            }
            if (!(((String) elem8Children[0]).Equals("Hey Dude ")))
            {
                GlobalLog.LogStatus("elem8Children[0]elem8Children[0] did not match");
                result = false;
            }
            if (!(((String) elem9Children[1]).Equals(" Hey Dude")))
            {
                GlobalLog.LogStatus("elem9Children[1] did not match");
                result = false;
            }
            if (!(((String) elem10Children[0]).Equals("Hey dear ")))
            {
                GlobalLog.LogStatus("elem10Children[0] did not match");
                result = false;
            }
            if (!(((String) elem10Children[2]).Equals(" Dude")))
            {
                GlobalLog.LogStatus("elem10Children[2] did not match");
                result = false;
            }
            if (!(Color.Equals((((Button) elements[11]).Background as SolidColorBrush).Color, Colors.Red)))
            {
                GlobalLog.LogStatus("elements[11] did not match");
                result = false;
            }
            if (!(((String) elements[12].Content).Equals("ทำไมเขาถึงไม่พูดภาษาไทย Hi 他们为什么不说中文（中国）？ Dude उनीहरू किन नेपाली मात्र बोल्न सक्दैनन् ?")))
            {
                GlobalLog.LogStatus("elements[12] did not match");
                result = false;
            }
            if (!(((String) elements[13].Content).Equals("Hey Dude")))
            {
                GlobalLog.LogStatus("elements[13] did not match");
                result = false;
            }
            if (!(((String) elements[15].Content).Equals("  Hey     Dude  "))) //No verification for 14;
            {
                GlobalLog.LogStatus("elements[15] did not match");
                result = false;
            }
            if (!(((String) elements[16].Content).Equals("Hey" + "\n\t    Dude" + "\n\t"))) // With XML, newline is always \n
            {
                GlobalLog.LogStatus("elements[16] did not match");
                result = false;
            }
            if (!(((String) elements[17].Content).Equals(" Hey Dude")))
            {
                GlobalLog.LogStatus("elements[17] did not match");
                result = false;
            }
            if (!(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[0] as String).Equals("Hello World ")))
            {
                GlobalLog.LogStatus("!(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[0] as String).Equals(Hello World ))");
                result = false;
            }
            if (!(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[2] as String).Equals(" Hi There")))
            {
                GlobalLog.LogStatus("!(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[2] as String).Equals( Hi There))");
                result = false;
            }

            if (!(Color.Equals((((Button) elements[19]).Background as SolidColorBrush).Color, Colors.Red)))
            {
                GlobalLog.LogStatus("!(Color.Equals((((Button)elements[19]).Background as SolidColorBrush).Color, Colors.Red))");
                result = false;
            }
            if (!(Color.Equals((((Button) elements[20]).Background as SolidColorBrush).Color, Colors.Green)))
            {
                GlobalLog.LogStatus("!(Color.Equals((((Button)elements[20]).Background as SolidColorBrush).Color, Colors.Green))");
                result = false;
            }

            // 


            return result;
        }
    }
}
