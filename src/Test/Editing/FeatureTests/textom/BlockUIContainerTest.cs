// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API of the BlockUIContainer class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 25 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/textom/BlockUIContainer.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Test.Uis.Data;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;    
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows.Markup;

    #endregion Namespaces.


    /// <summary>
    /// Test the APIs of BlockUIContainer.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("29"), TestBugs("601"), TestWorkItem("82"), TestLastUpdatedOn("July 13, 2006")]
    public class BlockUIContainerAPITest : CombinedTestCase
    {
        RichTextBox _richTextBox;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            MainWindow.Content = _richTextBox;
            QueueDelegate(DoTest);
        }

        void DoTest()
        {
            //Basic creation.
            BlockUIContainer container = new BlockUIContainer();
            Verifier.Verify(container.Child == null, "Failed - BlockUIContainer.Child should be null when it is created without parameter");
            
            //Create with null:
            //Regression_Bug601 - Editing: should the constructor BlockUIContainer(UIElement uiElement) throw an exception when the parameter is null?
            try
            {
                container = new BlockUIContainer(null);
                throw new Exception("did not get exception for null parameter!");
            }
            catch (Exception e)
            {
                Log("Get the expected messange:"+ e.Message);
            }
            Button button = new Button();
            button.Content = "button1";
            container.Child = button;

            Verifier.Verify(container.Child is Button, "Failed - BlockUIcontainer does not contain a Button!");

            //remove child:
            container.Child = null;
            Verifier.Verify(container.Child == null, "Failed - BlockUIContainer.Child should be null when null is added to the child");

            //Test Undo for specifying Child.
            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(container);
            button.Content="button for undo";
            container.Child = button;
            QueueDelegate(PerformUndo);
        }

        void PerformUndo()
        {
            BlockUIContainer bcontainer = _richTextBox.Document.Blocks.FirstBlock as BlockUIContainer; 
            Button b = bcontainer.Child as Button;
            Verifier.Verify(((string)b.Content) == "button for undo", "Failed: button content is does not match, Expected[button for undo], Actual[" + b.Content.ToString() + "]");
            _richTextBox.Undo();
            QueueDelegate(VerifyUndo);
        }
        void VerifyUndo()
        {
            BlockUIContainer container = _richTextBox.Document.Blocks.FirstBlock as BlockUIContainer;
            Verifier.Verify(container != null && container.Child==null, "Can't undo the action of assigning child to BlockUIContainer!");
            EndTest();
        }
    }
}
