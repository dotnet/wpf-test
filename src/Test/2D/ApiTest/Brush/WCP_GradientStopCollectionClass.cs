// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test all public API's in the GradientStopCollection class
//
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Graphics
{
    internal class WCP_GradientStopCollectionClass : ApiTest
    {
        public WCP_GradientStopCollectionClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            _class_testresult = true;
            _objectType = typeof(GradientStopCollection);
            _helper = new HelperClass();
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus(_objectType.ToString());

            // Animation to be used for testing Properties
            ColorAnimation AnimColor = new ColorAnimation(Colors.Fuchsia, Colors.Yellow, new System.Windows.Duration(TimeSpan.FromMilliseconds(1000)));

            // Expected exception messages
            String InvalidOperationException = "Specified argument was out of the range of valid values." + (char)13 + (char)10 + "Parameter name: index";

            #region SECTION I - CONSTRUCTORS
            CommonLib.LogStatus("***** SECTION I - CONSTRUCTORS *****");

            #region Test #1 - Default Constructor
            // Usage: GradientStopCollection()
            // Notes: Default Constructor creates an empty GradientStopCollection.
            CommonLib.LogStatus("Test #1 - Default Constructor");

            // Create a GradientStopCollection 
            GradientStopCollection GSCprop = new GradientStopCollection();
            GSCprop.Add(new GradientStop(Colors.Red, 0.0));
            GSCprop.Add(new GradientStop(Colors.Blue, 1.0));
            // Confirm that a GradientStopCollection was created successfully.
            _class_testresult &= _helper.CheckType(GSCprop, _objectType);
            #endregion

            #endregion End Of SECTION I

            #region SECTION II - PROPERTIES
            CommonLib.LogStatus("***** SECTION II - PROPERTIES *****");

            #region Test #2 - The Count Property
            // Usage: int32 = GradientStopCollection.Count (Read Only)
            // Notes: Returns the number of GradientStops in the Collection.
            CommonLib.LogStatus("Test #2 - The Count Property");

            // Get the Count property and check the Count value to assure that it is
            // the expected value.
            _class_testresult &= _helper.CompareProp("Count", GSCprop.Count, 2);
            #endregion

            #region Test #4 - The CanFreeze Property
            // Usage: bool = GradientStopCollection.CanFreeze (Read Only)
            // Notes: Returns a bool that indicates whether the GradientStopCollection is animatable.
            CommonLib.LogStatus("Test #4 - The CanFreeze Property");

            // Create an array of GradientStops
            GradientStop[] gsArray24 = new GradientStop[]
            { 
                new GradientStop (Colors.Red, 0.0), new GradientStop (Colors.Blue, 1.0) 
            };

            // Create a GradientStopCollection
            GradientStopCollection GSC24 = new GradientStopCollection();
            foreach (GradientStop g in gsArray24)
            {
                GSC24.Add(g);
            }

            // Check the CanFreeze value of a non-animated GradientStopCollection.
            _class_testresult &= _helper.CompareProp("CanFreeze", GSC24.CanFreeze, true);

            // Add an Animation to a GradientStop in the Collection
            GSC24[0].BeginAnimation(GradientStop.ColorProperty, AnimColor);

            // Check the CanFreeze value of a GradientStopCollection with an animated GradientStop.
            _class_testresult &= _helper.CompareProp("CanFreeze (Animated)", GSC24.CanFreeze, false);
            #endregion

            #region Test #7 - The Item [] Property
            // Usage: GradientStop = GradientStopCollection[int32] (R/W)
            // Notes: Gets or Sets the GradientStop at the specified index in the Collection.
            // Overrides the subscript operator [].
            CommonLib.LogStatus("Test #7 - The Item [] Property");

            // Use the Item property to Set a GradientStop.
            GradientStop NewStop = new GradientStop(Colors.Yellow, 0.1);
            GSCprop[0] = NewStop;

            // Get the Item property and check the Item value to assure that it was the
            // one that was set.
            _class_testresult &= _helper.CompareProp("Item", GSCprop[0], NewStop);

            // Trying to Get an Item that has an index < 0 or > Count should generate
            // the expected exception.
            try
            {
                GradientStop BadStop = GSCprop[-1];
            }
            catch (System.Exception e)
            {
                if (e.Message == InvalidOperationException)
                {
                    CommonLib.LogStatus("Pass: Item < 0 returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Item < 0 returns " + e.Message + " should be " + InvalidOperationException);
                    _class_testresult &= false;
                }
            }

            // Trying to Get an Item that has an index < 0 or > Count should generate
            // the expected exception.
            try
            {
                GradientStop BadStop2 = GSCprop[GSCprop.Count + 1];
            }
            catch (System.Exception e)
            {
                if (e.Message == InvalidOperationException)
                {
                    CommonLib.LogStatus("Pass: Item > Count returns the expected exception");
                }
                else
                {
                    CommonLib.LogFail("Fail: Item > Count returns " + e.Message + " should be " + InvalidOperationException);
                    _class_testresult &= false;
                }
            }
            #endregion

            #region Test #8 - Basic rendering with a GradientStopCollection
            // Just fill the surface with a RadialGradient that uses a GradientStopCollection
            // to see if we crash or not.
            CommonLib.LogStatus("Test #8 - Basic rendering with a GradientStopCollection");

            // Create a RadialGradientBrush and set the GradientStopCollection
            RadialGradientBrush brush = new RadialGradientBrush();
            brush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            brush.Center = new Point(0.5, 0.5);
            brush.GradientOrigin = new Point(0.5, 0.5);
            brush.RadiusX = 0.7;
            brush.RadiusY = 0.4;
            brush.GradientStops = GSCprop;

            DC.DrawRectangle(brush, null, new System.Windows.Rect(m_top, m_left, m_width, m_height));
            #endregion
            #endregion End Of SECTION II

            #region SECTION III - METHODS
            CommonLib.LogStatus("***** SECTION III - METHODS *****");

            #region Test #1 - The Add Method
            // Usage: int32 = GradientStopCollection.Add(GradientStop)
            // Notes: Adds a GradientStop to the Collection and returns the index of the new Stop.
            CommonLib.LogStatus("Test #1 - The Add Method");

            // Create a new GradientStopCollection
            GradientStopCollection GSC31 = new GradientStopCollection();

            // Use the Add Method
            GradientStop GS31 = new GradientStop(Colors.Green, 0.0);
            GSC31.Add(GS31);
            int AddIndex = GSC31.IndexOf(GS31);

            // Check that the GradientStop was added correctly
            _class_testresult &= _helper.CompareProp("Add", GSC31[AddIndex], GS31);

            // Add the same Automated GradientStop twice as a regression test for Regression_Bug56
            GradientStopCollection GSC31Anim = new GradientStopCollection();

            GradientStop GS31Anim = new GradientStop(Colors.Green, 0.0);
            GS31Anim.BeginAnimation(GradientStop.ColorProperty, AnimColor);

            GSC31Anim.Add(GS31Anim);
            GSC31Anim.Add(GS31Anim);

            // Check that the GradientStop was added correctly
            _class_testresult &= _helper.CompareProp("Add Animated Stop Twice", GSC31Anim[1], GS31Anim);
            #endregion

            #region Test #3 - The Clear Method
            // Usage: GradientStopCollection.Clear()
            // Notes: Clears the contents of the GradientStopCollection
            CommonLib.LogStatus("Test #3 - The Clear Method");

            // Use the Clear Method
            GSC31.Clear();

            // Check that the GradientStopCollection was cleared
            _class_testresult &= _helper.CompareProp("Clear", GSC31.Count, 0);
            #endregion

            #region Test #4 - The Contains Method
            // Usage: bool = GradientStopCollection.Contains(GradientStop)
            // Notes: Returns a bool that indicates whether the specified
            // GradientStop is in the Collection. Note: this uses reference
            // instead of value to search.
            CommonLib.LogStatus("Test #4 - The Contains Method");

            // Create a GradientStop to search for
            GradientStop GS34 = new GradientStop(Colors.Red, 0.0);

            // Create a new GradientStopCollection
            GradientStopCollection GSC34 = new GradientStopCollection();
            GSC34.Add(GS34);
            GSC34.Add(new GradientStop(Colors.Blue, 1.0));

            // Use the Contains Method and check that the Contains method returns the expected value.
            _class_testresult &= _helper.CompareProp("Contains", GSC34.Contains(GS34), true);
            #endregion

            #region Test #5 - The Copy Method
            // Usage: GradientStopCollection = GradientStopCollection.Clone()
            // Notes: Returns a copy of this GradientStopCollection.
            CommonLib.LogStatus("Test #5 - The Copy Method");

            // Create a GradientStopCollection.
            GradientStopCollection GSC35a =
            new GradientStopCollection();
            GSC35a.Add(new GradientStop(Colors.Red, 0.1));
            GSC35a.Add(new GradientStop(Colors.Blue, 0.8));

            // Use the Copy method to create a new GradientStopCollection that has the same value.
            GradientStopCollection GSC35b = GSC35a.Clone();

            // Check that the two GradientStopCollections are the same
            _class_testresult &= _helper.CompareProp("Copy", GSC35a, GSC35b);
            #endregion

            #region Test #6 - The CopyTo Method
            // Usage: void = GradientStopCollection.CopyTo(System.Array, int32)
            // Notes: Copies the entire GradientStopCollection to an Array of
            // GradientStops beginning at the specified index of the destination array.
            CommonLib.LogStatus("Test #6 - The CopyTo Method");

            // Create a GradientStopCollection.
            GradientStopCollection GSC36 = new GradientStopCollection();
            GSC36.Add(new GradientStop(Colors.Blue, 0.0));
            GSC36.Add(new GradientStop(Colors.Yellow, 1.0));

            // Use the CopyTo method to create an array that has just
            // two of the stops in the original Collection.
            GradientStop[] stop_array = new GradientStop[2];
            GSC36.CopyTo(stop_array, 0);

            // Check the size and contents of the Array
            if (stop_array.Length == 2)
            {
                if ((((GradientStop)stop_array.GetValue(0)).Color == Colors.Blue && ((GradientStop)stop_array.GetValue(0)).Offset == 0.0) &&
                (((GradientStop)stop_array.GetValue(1)).Color == Colors.Yellow && ((GradientStop)stop_array.GetValue(1)).Offset == 1.0))
                {
                    CommonLib.LogStatus("Pass: CopyTo - GradientStop properties have the same value");
                }
                else
                {
                    CommonLib.LogFail("Fail: CopyTo - GradientStop properties do not have the same value");
                    _class_testresult &= false;
                }
            }
            else
            {
                CommonLib.LogFail("Fail: CopyTo - GradientStopCollection has " + stop_array.Length.ToString() + " values when it should have 2");
                _class_testresult &= false;
            }
            #endregion

            #region Test #7 - The CloneCurrentValue Method
            // Usage: GradientStopCollection = GradientStopCollection.CloneCurrentValue()
            // Notes: Returns a new GradientStopCollection that has the same CurrentValue
            // as this GradientStopCollection.
            CommonLib.LogStatus("Test #7 - The CloneCurrentValue Method");

            // Create a GradientStopCollection.
            GradientStopCollection GSC37a = new GradientStopCollection();
            GSC37a.Add(new GradientStop(Colors.Red, 0.2));
            GSC37a.Add(new GradientStop(Colors.Green, 0.7));

            // Use the CloneCurrentValue method to create a new GradientStopCollection
            // that has the same value.
            GradientStopCollection GSC37b = GSC37a.CloneCurrentValue();

            // Check that the two GradientStopCollections are the same
            _class_testresult &= _helper.CompareProp("CloneCurrentValue", GSC37a, GSC37b);
            #endregion

            #region Test #12 - The IndexOf Method - with one GradientStop
            // Usage: int32 = GradientStopCollection.IndexOf(GradientStop)
            // Notes: Returns the index of the first matching GradientStop from the Collection.
            // (Calls IndexOf() on the internal Array).
            // Note: this uses reference instead of value to search.
            CommonLib.LogStatus("Test #12 - The IndexOf Method - with one GradientStop");

            // Create a GradientStopCollection for use in the IndexOf tests.
            GradientStopCollection GSC310 = new GradientStopCollection();
            GSC310.Add(new GradientStop(Colors.Red, 0.0));
            GSC310.Add(new GradientStop(Colors.Green, 0.5));
            GSC310.Add(new GradientStop(Colors.Yellow, 1.0));

            // Use the IndexOf Method
            int IndexOf312 = GSC310.IndexOf(new GradientStop(Colors.Yellow, 1.0));

            // Check that IndexOf returns the expected value
            _class_testresult &= _helper.CompareProp("IndexOf", IndexOf312, -1);
            #endregion

            #region Test #13 - The Insert Method
            // Usage: GradientStopCollection.Insert(int32, GradientStop)
            // Notes: Inserts a GradientStop into the collection at the specified Index.
            CommonLib.LogStatus("Test #13 - The Insert Method");

            // Use the Insert Method
            GradientStop GS313 = new GradientStop(Colors.Orange, 0.3);
            GSC310.Insert(1, GS313);

            // Check that the GradientStop was inserted in the right position and returns the expected value
            _class_testresult &= _helper.CompareProp("Insert", GSC310[1], GS313);
            #endregion

            #region Test #17 - The Remove Method
            // Usage: GradientStopCollection.Remove(GradientStop)
            // Removes the specified GradientStop from the Collection.
            // Note: this uses reference instead of value to search.
            CommonLib.LogStatus("Test #17 - The Remove Method");

            // Create a GradientStopCollection
            GradientStopCollection GSC317 = new GradientStopCollection();
            GSC317.Add(new GradientStop(Colors.Red, 0.0));

            // Use the Remove Method
            GSC317.Remove(new GradientStop(Colors.Red, 0.0));

            // There should now be 0 stops in the Collection
            _class_testresult &= _helper.CompareProp("Remove", GSC317.Count, 1);
            #endregion

            #region Test #18 - The RemoveAt Method
            // Usage: GradientStopCollection.RemoveAt(int32)
            // Notes: Removes the GradientStop from the Collection that is at the
            // specified Index.
            CommonLib.LogStatus("Test #18 - The RemoveAt Method");

            // Create a GradientStopCollection 
            GradientStopCollection GSC318 = new GradientStopCollection();
            GSC318.Add(new GradientStop(Colors.Red, 0.0));
            GSC318.Add(new GradientStop(Colors.Blue, 1.0));

            // Use the RemoveAt Method
            GSC318.RemoveAt(1);

            // There should now be 1 stop in the Collection
            _class_testresult &= _helper.CompareProp("RemoveAt", GSC318.Count, 1);
            #endregion

            #endregion End Of SECTION III

            CommonLib.LogTest("Result for: " + _objectType);
        }

        private bool _class_testresult;
        private Type _objectType;
        private HelperClass _helper;
    }
}
