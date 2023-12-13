// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit and functional testing for the TextContainer class.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading; using System.Windows.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.
    
    /// <summary>
    /// Verifies that the bug doesnt repro
    /// 1. Regression_Bug203
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("384"), TestBugs("203")]
    public class BugReproRegression_Bug203 : CustomTestCase
    {
        DockPanel _testPanel;
        Button _testButton;
        ScrollViewer _testSV;
        FlowDocumentScrollViewer _testTextPanel;
        UIElementWrapper _testWrapper;
        int _count = 0;

        #region TestStrings
        const string testText = @"Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Fusce mollis tortor eget mauris. Mauris eu quam. Nunc convallis vestibulum diam. In venenatis lorem non magna. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin sapien sapien, egestas vitae, adipiscing vel, condimentum id, erat. Aenean mauris mi, malesuada sed, euismod eget, consectetuer non, justo. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. Praesent vel enim. Vestibulum lobortis consequat tortor. Cras at erat ac nulla faucibus lobortis. Ut dapibus risus sed nibh. Integer ullamcorper interdum dui. Praesent sit amet mi. Donec vel pede.

Sed nisl mauris, porta at, malesuada sed, dignissim sit amet, massa. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Aliquam enim. Maecenas euismod orci non lorem. Duis et odio. Sed rutrum. Integer et metus eu velit rhoncus accumsan. Morbi ultricies. Suspendisse eu quam ac wisi aliquet bibendum. Fusce id felis. Aliquam interdum velit ac arcu. Vestibulum vehicula nulla eu metus convallis malesuada. Duis nibh. Ut ut mauris eget dolor luctus fermentum. Suspendisse id quam vitae quam vestibulum congue. Phasellus mi. Pellentesque venenatis dolor sed elit. Praesent erat.

Quisque quam dui, suscipit vitae, convallis ac, gravida a, nisl. Donec placerat lorem eget risus. Sed elementum ipsum at wisi. Duis dapibus, neque eu eleifend lobortis, mauris quam pharetra urna, sed elementum nibh libero interdum nulla. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Duis hendrerit, arcu nec pharetra faucibus, pede velit vestibulum ante, in blandit nulla lorem a diam. Integer gravida turpis non mi. Morbi tempus, pede ac rutrum varius, felis dolor sagittis est, volutpat porta tellus velit vel mi. Sed pulvinar, purus a malesuada rutrum, velit neque congue turpis, in rhoncus ante elit eu dui. Sed blandit enim sit amet quam. Fusce at magna vel nulla hendrerit aliquet. Etiam dui nunc, vestibulum vel, pellentesque at, placerat quis, wisi. Cras at augue malesuada ligula lobortis pellentesque. Nunc volutpat sagittis lectus. Aliquam dictum auctor tellus. Suspendisse placerat pede a odio. Curabitur elementum orci vitae eros. Suspendisse condimentum dictum est.

Sed diam. Nulla facilisi. Aenean placerat. Curabitur ac ipsum ut dolor bibendum vulputate. Mauris sed nunc. Morbi porttitor nunc nec nulla. Vivamus velit magna, tincidunt in, commodo non, porta vitae, dolor. Suspendisse malesuada. Ut convallis magna ac justo. Vestibulum pellentesque. Duis tincidunt, lorem non aliquam cursus, nisl lectus congue velit, vitae dapibus ante nisl id urna. Pellentesque sodales, pede molestie volutpat cursus, dui ante commodo nibh, eget ullamcorper massa mi in nibh. Suspendisse pellentesque consequat libero. Morbi libero. Sed convallis tellus et diam. Vestibulum aliquet metus vitae velit. Sed lacinia magna non sapien gravida hendrerit. Donec vestibulum fermentum orci. Mauris ultricies.

Nunc iaculis. Proin tincidunt, libero vel volutpat vestibulum, ante diam sollicitudin pede, at luctus ipsum diam nec arcu. Nunc ipsum tellus, gravida id, elementum ac, consequat id, orci. Donec turpis. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec et libero quis ligula vulputate accumsan. Praesent velit orci, lobortis nec, bibendum sed, commodo vel, enim. Vestibulum mauris ipsum, fermentum et, ultricies in, euismod quis, arcu. Nunc molestie purus eu tortor. Aliquam orci libero, viverra id, faucibus id, vestibulum eu, nibh. Vivamus pharetra semper neque. Donec urna orci, fringilla eget, pharetra eu, luctus eu, diam. Cras quis eros. Sed pede tellus, tempus vel, convallis sed, condimentum sed, felis. Sed pellentesque mauris.

Curabitur sit amet lorem sed nibh pellentesque cursus. Nullam nec dui. Donec tempor, velit et iaculis cursus, mi lectus luctus arcu, vel posuere massa sem vitae velit. Sed nonummy erat porta ante. Etiam neque massa, facilisis vel, lobortis eu, egestas non, nisl. Integer in elit eu eros suscipit porta. Mauris sagittis malesuada nisl. Sed ut orci quis neque nonummy cursus. Sed sodales, velit non lobortis varius, eros dolor fermentum mi, a suscipit felis mauris at eros. Vivamus convallis tempor felis. Etiam lorem ligula, varius sed, faucibus vel, malesuada eget, eros. Nulla porttitor pellentesque elit. Ut blandit pulvinar turpis. Phasellus mattis sem et tellus.

Mauris at leo. Nullam facilisis, velit ac posuere dapibus, nibh lectus nonummy tortor, et accumsan lorem dui tristique erat. Ut semper mattis diam. Proin justo nunc, facilisis vel, fringilla a, porta non, odio. Nam blandit augue eu ante. Phasellus lorem. Ut a neque. Proin ac dui nec purus hendrerit suscipit. Nulla quam. Pellentesque laoreet, dolor eget sagittis nonummy, tortor nibh volutpat metus, sed lacinia wisi sapien vitae sapien. Aliquam ut wisi nec turpis auctor dapibus. Aliquam aliquam pretium mi. Donec blandit, erat in consectetuer aliquet, erat metus dictum est, sed volutpat odio mauris ac eros. Vivamus tempus auctor nibh. Aenean auctor ipsum eget est. Pellentesque pharetra risus nec orci. Phasellus et wisi at neque pulvinar auctor. Vivamus ac arcu ac tortor mollis tincidunt.

Nulla facilisi. In hac habitasse platea dictumst. Maecenas nibh augue, convallis tincidunt, vestibulum ut, viverra vel, ligula. Vestibulum pharetra pede ac eros. Vivamus laoreet condimentum turpis. Phasellus semper gravida pede. Sed faucibus adipiscing est. Integer at sapien. Quisque vel diam non purus imperdiet malesuada. Vestibulum est nunc, vulputate ut, tristique id, suscipit pellentesque, diam. Donec dui pede, imperdiet nec, venenatis vel, porta in, lectus. Morbi dictum.

Fusce augue mauris, sollicitudin in, tempus et, bibendum ut, augue. In hac habitasse platea dictumst. Ut mollis purus eleifend elit. Curabitur ac enim. Praesent ornare. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Sed sodales velit non mauris. Maecenas consectetuer dui vitae felis. Quisque enim wisi, rutrum non, sodales nec, dapibus vitae, elit. Aenean tellus orci, faucibus nec, venenatis eget, eleifend non, dui. Phasellus nec libero. Morbi a nibh sit amet metus porta ultricies. Aliquam dapibus pede. Suspendisse faucibus rutrum ipsum. Ut sapien wisi, vehicula non, ultricies a, semper eu, libero. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Nullam vulputate lorem eget ligula. Suspendisse potenti. Vestibulum ut nisl. Nam nunc.

Quisque risus neque, pharetra id, malesuada nec, auctor quis, nulla. In lobortis. Nulla eget augue. Curabitur purus. Curabitur aliquam magna vel lacus rhoncus lobortis. Praesent vitae nibh ac arcu consequat luctus. Pellentesque pulvinar commodo libero. Cras molestie, diam vel condimentum lobortis, risus eros scelerisque orci, in adipiscing dolor massa non orci. Proin erat nisl, ultricies ut, lacinia eu, malesuada a, sapien. Fusce pretium ligula. Cras in libero. Donec sit amet erat ut risus lobortis pretium. Nam eget velit ac sapien lacinia suscipit. Sed pellentesque vehicula mauris. Fusce tincidunt erat id nisl. Sed quis mauris at ipsum consectetuer venenatis.

Donec varius facilisis massa. Curabitur tristique risus eget augue. Cras vitae lorem ac wisi semper molestie. Pellentesque ultrices felis in mi. Ut accumsan dictum ante. In neque. Curabitur ut dolor a wisi nonummy suscipit. Mauris risus turpis, porttitor vitae, euismod rutrum, auctor at, mauris. Mauris eros. Etiam in massa. Ut purus. Pellentesque in velit et quam vestibulum sollicitudin. Vestibulum tempus, felis non dictum luctus, metus est adipiscing elit, eu lobortis wisi ipsum vel metus. Nunc aliquam, lorem ac pellentesque consequat, turpis dolor posuere erat, ac hendrerit nulla leo sed turpis. In molestie aliquam orci. Donec sollicitudin. Cras faucibus.

Etiam gravida venenatis dui. Donec auctor urna sed justo. Aliquam venenatis sem commodo neque. Cras egestas ornare augue. Sed quis augue in massa imperdiet ultricies. Mauris adipiscing purus sed est. Mauris non purus. Nunc ullamcorper. Nullam neque magna, hendrerit ut, hendrerit in, condimentum ut, mauris. Mauris nec tellus. Vivamus vehicula commodo velit.

Maecenas a diam eu nulla feugiat laoreet. Praesent ultricies. Cras sem est, hendrerit non, vehicula vel, fermentum vel, nibh. Quisque a felis. In elit. Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Curabitur dictum wisi at velit. Nunc aliquet sollicitudin risus. In consectetuer ante vestibulum risus. Donec turpis mauris, mattis sed, cursus ut, posuere vitae, quam.

Quisque bibendum iaculis tortor. Phasellus wisi. Aenean pretium mollis ipsum. Fusce mauris. Phasellus ultrices, justo ut accumsan vestibulum, orci nisl iaculis tortor, id dignissim nibh tellus adipiscing enim. Aenean ac leo eget odio sagittis venenatis. Aliquam malesuada venenatis nibh. Mauris rhoncus tortor quis lacus. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. Integer eros nisl, dictum sit amet, aliquet sodales, consectetuer elementum, quam. Sed auctor tempor erat. Nulla in libero vel est gravida laoreet.

Nunc bibendum felis sed lacus. Nulla facilisi. Praesent ultrices rhoncus augue. In in turpis. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec vel sapien. Nunc diam diam, adipiscing sed, facilisis at, nonummy in, elit. Cras accumsan venenatis erat. Praesent tempor, neque a cursus vehicula, lectus purus malesuada purus, viverra ornare justo ante volutpat nibh. Nullam semper sem eu est.

Vivamus pellentesque urna in dui. Maecenas pharetra, nibh vitae laoreet blandit, massa dui scelerisque elit, vel tincidunt justo ligula vitae wisi. Sed sed enim quis mi sagittis tincidunt. Nullam ac justo. Curabitur quam lorem, elementum in, luctus vel, congue et, sem. Aenean consequat, metus sed porttitor pellentesque, velit arcu feugiat lacus, vel gravida mi purus sed purus. Nam tellus. Morbi nec est ac augue lobortis condimentum. Vivamus elementum tellus at nulla. Phasellus velit. Proin vitae nulla. Pellentesque eu odio in libero volutpat porta. Donec pulvinar, wisi non suscipit iaculis, sapien justo congue neque, sed euismod ante mauris id massa. Phasellus tempus ornare urna. Nunc dictum iaculis erat. Nullam in lacus. Mauris adipiscing dolor at eros. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. Pellentesque odio.

Phasellus quis tellus. Duis lorem diam, mollis a, imperdiet non, tristique vitae, enim. Nunc volutpat dignissim pede. Nullam mollis volutpat neque. Vestibulum ut arcu. Proin malesuada mattis magna. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos hymenaeos. Nullam condimentum consectetuer ipsum. Nulla diam libero, dictum id, tempus at, varius ac, magna. Donec pharetra fermentum sapien. Nam laoreet dolor non massa. Quisque semper, libero id vulputate elementum, erat magna blandit velit, eu viverra arcu justo sed magna.

Integer porta imperdiet risus. Donec faucibus elementum ligula. Maecenas nec justo a mi varius pretium. Donec nec metus. Sed vel elit et elit placerat hendrerit. Integer feugiat eros viverra lectus. Integer vel erat non leo pretium egestas. Nulla eros sapien, ultricies vitae, dapibus et, tristique at, mi. Etiam blandit, mi sed blandit tincidunt, quam risus semper quam, vitae dictum tortor tortor quis sem. Donec pretium nulla sed ante. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Curabitur non leo. Maecenas lacus. Nulla facilisi. Nullam eleifend. Integer lectus dui, accumsan vitae, pulvinar pharetra, rutrum quis, magna.

Nulla dapibus. Praesent ut ligula. Suspendisse sodales velit id augue. Suspendisse pretium iaculis enim. Mauris sodales cursus enim. Nulla lacinia euismod sapien. Duis nunc magna, tincidunt vel, auctor id, tempor nec, mi. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nam euismod volutpat orci. Aenean pellentesque vestibulum libero. Pellentesque purus eros, bibendum et, lobortis nec, mattis pulvinar, nulla.

Curabitur felis. Nulla tortor. Aliquam mollis tincidunt nibh. Duis tristique dui vel risus. Fusce elementum turpis vel turpis. Etiam viverra, diam a cursus adipiscing, justo urna lobortis sapien, sed accumsan diam augue in eros. Mauris ultrices mi tincidunt odio. Etiam in arcu a nisl blandit gravida. Duis tempus elit sit amet purus. Curabitur nibh.
";

        #endregion TestStrings

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _testPanel = new DockPanel();

            _testButton = new Button();
            _testButton.Width = 200;
            _testButton.SetValue(DockPanel.DockProperty, Dock.Top);
            _testButton.Content = "ClickME";
            _testButton.Click += new RoutedEventHandler(testButton_Click);
            _testSV = new ScrollViewer();

            _testPanel.Children.Add(_testButton);
            _testPanel.Children.Add(_testSV);

            _testTextPanel = new FlowDocumentScrollViewer();
            _testTextPanel.Document = new FlowDocument();
            _testWrapper = new UIElementWrapper(_testTextPanel);
            _testSV.Content = _testTextPanel;

            MainWindow.Content = _testPanel;
            MainWindow.Width = 500;
            MainWindow.Height = 500;
            QueueDelegate(DoInputAction);
        }

        private void testButton_Click(object sender, RoutedEventArgs args)
        {
            string str;
            str = testText;
            _testTextPanel.Text = str;
        }

        private void DoInputAction()
        {
            Log("Clicking the Button");
            MouseInput.MouseClick(_testButton);
            QueueDelegate(VerifyData);
        }

        private void VerifyData()
        {
            TextRange testTR;
            if (_count == 0)
            {
                QueueDelegate(DoInputAction);
                _count++;
            }
            else
            {
                testTR = _testWrapper.GetLineRange(7);
                Log("Actual contents of the 7th line is: [" + testTR.Text + "]");
                Log("Expected contents on the 7th line is: []");
                Verifier.Verify(testTR.Text == "", "Contents of line 7 match as expected", true);

                testTR = _testWrapper.GetLineRange(15);
                Log("Actual contents of the 15th line is: [" + testTR.Text + "]");
                Log("Expected contents on the 15th line is: []");
                Verifier.Verify(testTR.Text == "", "Contents of line 15 match as expected", true);

                testTR = _testWrapper.GetLineRange(20);
                Log("Actual contents of the 20th line is: [" + testTR.Text + "]");
                Log("Expected contents on the 20th line is nonempty");
                Verifier.Verify(testTR.Text != "", "Contents of line 20 match as expected", true);

                Logger.Current.ReportSuccess();
            }
        }
    }
}
