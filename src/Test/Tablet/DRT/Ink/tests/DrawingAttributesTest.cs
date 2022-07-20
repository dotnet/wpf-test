// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;

namespace DRT
{
    /// <summary>
    /// Summary description for DrawingAttributesUndoTest.
    /// </summary>
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class DrawingAttributesTest : DrtInkTestcase
    {

        public override void Run ()
        {

            TestDefaults();

            TestExtendedPropertyBackingStore();

            TestEvents();

            TestEquality();

            TestRegressions();

            Success = true;
        }

        private void TestRegressions()
        {
            DrawingAttributes da = new DrawingAttributes();
            bool exceptionThrown = false;
            try
            {
                da.Height = 0f / 0f;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            if (!exceptionThrown)
            {
                throw new InvalidOperationException("DA.Height will accept 0/0!");
            }

            exceptionThrown = false;
            try
            {
                da.Height = Double.NaN;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            if (!exceptionThrown)
            {
                throw new InvalidOperationException("DA.Height will accept Double.NaN!");
            }

            exceptionThrown = false;
            try
            {
                da.Width = Double.NaN;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            if (!exceptionThrown)
            {
                throw new InvalidOperationException("DA.Width will accept Double.NaN!");
            }

            exceptionThrown = false;
            try
            {
                da.Width = 0f / 0f;
            }
            catch (ArgumentOutOfRangeException)
            {
                exceptionThrown = true;
            }
            if (!exceptionThrown)
            {
                throw new InvalidOperationException("DA.Width will accept 0f / 0f");
            }

            exceptionThrown = false;
            try
            {
                Matrix matrix = new Matrix(1, 1, 1, 1, 0, 0);
                System.Diagnostics.Debug.Assert(!matrix.HasInverse);
                da.StylusTipTransform = matrix;
            }
            catch (ArgumentException)
            {
                exceptionThrown = true;
            }
            if (!exceptionThrown)
            {
                throw new InvalidOperationException("DA.StylusTipTransform will accept a non-invertable Matrix");
            }


        }
        private void TestEquality()
        {
            DrawingAttributes da = new DrawingAttributes();
            DrawingAttributes da2 = new DrawingAttributes();

            if (!da.Equals(da2) || !da.Equals(da) || !da2.Equals(da2) || !(da == da2))
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed");
            }

            da.Color = Colors.Blue;
            da.FitToCurve = false;
            da.Height = 3f;
            da.Width = 4f;
            da.IgnorePressure = true;
            da.StylusTip = StylusTip.Rectangle;
            Matrix matrix = new Matrix();
            matrix.Rotate(45f);
            da.StylusTipTransform = matrix;
            da.IsHighlighter = true;

            da2.Color = Colors.Blue;
            da2.FitToCurve = false;
            da2.Height = 3f;
            da2.Width = 4f;
            da2.IgnorePressure = true;
            da2.StylusTip = StylusTip.Rectangle;
            Matrix matrix2 = new Matrix();
            matrix2.Rotate(45f);
            da2.StylusTipTransform = matrix2;
            da2.IsHighlighter = true;

            if (!da.Equals(da2) || !da.Equals(da) || !da2.Equals(da2) || !(da == da2))
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed");
            }

            //
            // test with extended properties
            //
            Guid guid1 = Guid.NewGuid();
            da.AddPropertyData(guid1, 1);
            da2.AddPropertyData(guid1, 1);
            if (!da.Equals(da2) || !da.Equals(da) || !da2.Equals(da2) || !(da == da2))
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed after setting an extended property");
            }

            int[] ep1 = new int[] { 1, 2, 3, 4 };
            int[] ep2 = new int[] { 1, 2, 3, 4 };
            Guid guid2 = Guid.NewGuid();

            da.AddPropertyData(guid2, ep1);
            da2.AddPropertyData(guid2, ep2);
            if (!da.Equals(da2) || !da.Equals(da) || !da2.Equals(da2) || !(da == da2))
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed after setting an array extended property");
            }

            //now make then not equal
            int[] ep3 = new int[] { 1, 2, 3, 5 };
            da2.AddPropertyData(guid2, ep3);
            if (da.Equals(da2) || da == da2)
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed after setting an array extended property");
            }

            int[] ep4 = new int[] { 1 };
            da2.AddPropertyData(guid2, ep4);
            if (da.Equals(da2) || da == da2)
            {
                throw new InvalidOperationException("DrawingAttributes.Equals / == test failed after setting an array extended property");
            }

        }

        /// <summary>
        /// Validates defaults
        /// </summary>
        private void TestDefaults()
        {
            DrawingAttributes drawingAttributes = new DrawingAttributes();

            //validate V2 defaults
            if (drawingAttributes.Color != Colors.Black ||
                drawingAttributes.FitToCurve != false ||
                drawingAttributes.Height != 2.0031496062992127 ||
                drawingAttributes.Width != 2.0031496062992127 ||
                drawingAttributes.IgnorePressure != false ||
                drawingAttributes.StylusTip != StylusTip.Ellipse ||
                drawingAttributes.StylusTipTransform != Matrix.Identity ||
                drawingAttributes.IsHighlighter != false
                )
            {
                throw new InvalidOperationException("Invalid assumptions about V2 default da properties");
            }
        }

        private int GetEPCItemCount(DrawingAttributes da)
        {
            try
            {
                return da.GetPropertyDataIds().Length;
            }
            catch (ArgumentException)
            {
                return 0;
            }
        }


        /// <summary>
        /// The backing store for drawing attributes is it's extendedpropertycollection
        /// test
        /// </summary>
        private void TestExtendedPropertyBackingStore()
        {
            DrawingAttributes da = new DrawingAttributes();

            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Default DA EPC count is not zero!");
            }

            ///// VALIDATE COLOR ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.Color = Colors.Black;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default Color caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.Color = Colors.Blue;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default Color failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.Color = Colors.Black;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a Color back to the default value failed to remove the color EP from DA.EPC!");
            }


            ///// VALIDATE HEIGHT ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.Height = 2.0031496062992127;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default Height caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.Height = 23.5;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default Height failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.Height = 2.0031496062992127;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting Height back to the default value failed to remove the height EP from DA.EPC!");
            }


            ///// VALIDATE WIDTH ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.Width = 2.0031496062992127;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default Width caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.Width = 23.5;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default Width failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.Width = 2.0031496062992127;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting Width back to the default value failed to remove the height EP from DA.EPC!");
            }



            ///// VALIDATE StylusTip ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.StylusTip = StylusTip.Ellipse;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default StylusTip caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.StylusTip = StylusTip.Rectangle;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default StylusTip failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.StylusTip = StylusTip.Ellipse;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting StylusTip back to the default value failed to remove the height EP from DA.EPC!");
            }


            ///// VALIDATE StylusTipTransform ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.StylusTipTransform = Matrix.Identity;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default StylusTipTransform caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            Matrix xf = Matrix.Identity;
            xf.Rotate(45f);
            da.StylusTipTransform = xf;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default StylusTipTransform failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.StylusTipTransform = Matrix.Identity;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting StylusTipTransform back to the default value failed to remove the height EP from DA.EPC!");
            }

            ///// VALIDATE IsHighlighter ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.IsHighlighter = false;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default IsHighlighter caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.IsHighlighter = true;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default IsHighlighter failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.IsHighlighter = false;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting IsHighlighter back to the default value failed to remove the height EP from DA.EPC!");
            }


            ///// VALIDATE DrawingFlags (FitToCurve, IgnorePressure) ///////

            //set the default, make sure it doesn't add to the backing EPC
            da.FitToCurve = false;
            da.IgnorePressure = false;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting a default DrawingFlags caused an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.FitToCurve = true;
            da.IgnorePressure = false;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default DrawingFlags failed to cause an EP to be added to DA's EPC!");
            }

            //set a non-default, make sure it is added to EPC
            da.FitToCurve = false;
            da.IgnorePressure = true;
            if (GetEPCItemCount(da) != 1)
            {
                throw new InvalidOperationException("Setting a non-default DrawingFlags failed to cause an EP to be added to DA's EPC!");
            }

            //go back to the default, make sure the EPC is removed
            da.FitToCurve = false;
            da.IgnorePressure = false;
            if (GetEPCItemCount(da) != 0)
            {
                throw new InvalidOperationException("Setting DrawingFlags back to the default value failed to remove the height EP from DA.EPC!");
            }
        }

        /// <summary>
        /// Validate events on DA
        /// </summary>
        private void TestEvents()
        {

            DrawingAttributes da = new DrawingAttributes();
            int currentDaEventCount = 0;
            int expectedDaEventCount = 0;

            Guid guid;
            object newValue = null;
            object previousValue = null;

            //sync the da changed event with an anonymous delegate

            PropertyDataChangedEventHandler daChangedAnonymousDelegate =
                delegate(object sender, PropertyDataChangedEventArgs args)
            {
                currentDaEventCount++;
                guid = args.PropertyGuid;
                newValue = args.NewValue;
                previousValue = args.PreviousValue;
            };


            try
            {
                da.AttributeChanged += daChangedAnonymousDelegate;

                //
                // Validate events for Color
                //
                expectedDaEventCount++;
                da.Color = Colors.Blue;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Color failed to raise a DA Changed event");
                }

                //make sure the EP was added to the EPC
                try
                {
                    Object o = da.GetPropertyData(DrawingAttributeIds.Color);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default Color failed to add DrawingAttributeIds.Color to the EPC!");
                }

                //make sure the color is the same
                Color color = (Color)da.GetPropertyData(DrawingAttributeIds.Color);
                if (color != Colors.Blue)
                {
                    throw new InvalidOperationException("Unexpected Color in the EPC");
                }

                //try setting the color via the ep
                expectedDaEventCount++;
                da.AddPropertyData(DrawingAttributeIds.Color, Colors.Red);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Color via the EPC failed to raise a DA Changed event");
                }

                //set again, to verify we don't raise another change notification
                da.AddPropertyData(DrawingAttributeIds.Color, Colors.Red);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a duplicate Color via the EPC raised a DA Changed event");
                }


                //
                // Validate events for Height
                //

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                da.Height = 23.5;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Height failed to raise a DA Changed event");
                }

                //set again, to verify we don't raise another change notification
                da.Height = 23.5;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a duplicate Height raised a DA Changed event");
                }

                //make sure the EP was added to the EPC
                try
                {
                    Object o = da.GetPropertyData(DrawingAttributeIds.StylusHeight);
                }
                catch (ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default Height failed to add DrawingAttributeIds.StylusHeight to the EPC!");
                }

                //make sure the height is the same
                double height = (double)da.GetPropertyData(DrawingAttributeIds.StylusHeight);
                if (height != 23.5)
                {
                    throw new InvalidOperationException("Unexpected Height in the EPC");
                }

                //try setting the height via the ep
                expectedDaEventCount++;
                da.AddPropertyData(DrawingAttributeIds.StylusHeight, 56.5);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Height via the EPC failed to raise a DA Changed event");
                }


                //
                // Validate events for Width
                //

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                da.Width = 23.5;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Width failed to raise a DA Changed event");
                }

                //make sure the EP was added to the EPC
                try
                {
                    Object o = da.GetPropertyData(DrawingAttributeIds.StylusWidth);
                }
                catch(ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default Width failed to add DrawingAttributeIds.StylusWidth to the EPC!");
                }

                //make sure the width is the same
                double width = (double)da.GetPropertyData(DrawingAttributeIds.StylusWidth);
                if (width != 23.5)
                {
                    throw new InvalidOperationException("Unexpected Width in the EPC");
                }

                //try setting the height via the ep
                expectedDaEventCount++;
                da.AddPropertyData(DrawingAttributeIds.StylusWidth, 56.5);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default Width via the EPC failed to raise a DA Changed event");
                }

                //set again, to verify we don't raise another change notification
                da.AddPropertyData(DrawingAttributeIds.StylusWidth, 56.5);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a duplicate Width via the EPC raised a DA Changed event");
                }


                //
                // Validate events for StylusTip
                //

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                da.StylusTip = StylusTip.Rectangle;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTip failed to raise a DA Changed event");
                }

                //try setting a dupe and make sure we don't get a bogus event
                da.StylusTip = StylusTip.Rectangle;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a duplicate StylusTip raised a DA Changed event");
                }

                //make sure the EP was added to the EPC
                try
                {
                    da.GetPropertyData(DrawingAttributeIds.StylusTip);
                }
                catch(ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTip failed to add DrawingAttributeIds.StylusTip to the EPC!");
                }

                //make sure the StylusTip is the same
                StylusTip tip = (StylusTip)da.GetPropertyData(DrawingAttributeIds.StylusTip);
                if (tip != StylusTip.Rectangle)
                {
                    throw new InvalidOperationException("Unexpected StylusTip in the EPC");
                }

                //try setting the StylusTip via the ep
                expectedDaEventCount++;
                da.AddPropertyData(DrawingAttributeIds.StylusTip, StylusTip.Ellipse);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTip via the EPC failed to raise a DA Changed event");
                }


                //
                // Validate events for StylusTipTransform
                //

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                Matrix xf = Matrix.Identity;
                xf.Rotate(45f);
                da.StylusTipTransform = xf;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTipTransform failed to raise a DA Changed event");
                }

                //make sure the EP was added to the EPC
                try
                {
                    Object o = da.GetPropertyData(DrawingAttributeIds.StylusTipTransform);
                }
                catch(ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTipTransform failed to add DrawingAttributeIds.StylusTipTransform to the EPC!");
                }

                //make sure the StylusTip is the same
                Matrix matrix = (Matrix)da.GetPropertyData(DrawingAttributeIds.StylusTipTransform);
                if (xf != matrix)
                {
                    throw new InvalidOperationException("Unexpected StylusTipTransform in the EPC");
                }

                //try setting the StylusTip via the ep
                expectedDaEventCount++;
                xf.Rotate(90f);
                da.AddPropertyData(DrawingAttributeIds.StylusTipTransform, xf);
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default StylusTipTransform via the EPC failed to raise a DA Changed event");
                }

                //
                // Validate events for DrawingFlags
                //

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                da.IgnorePressure = true;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default DrawingFlags failed to raise a DA Changed event");
                }

                //set a non-default, make sure it is added to EPC
                expectedDaEventCount++;
                da.FitToCurve = true;
                if (currentDaEventCount != expectedDaEventCount)
                {
                    throw new InvalidOperationException("Setting a non-default DrawingFlags failed to raise a DA Changed event");
                }

                try
                {
                    Object o = da.GetPropertyData(DrawingAttributeIds.DrawingFlags);
                }
                catch(ArgumentException)
                {
                    throw new InvalidOperationException("Setting a non-default DrawingFlags failed to add DrawingAttributeIds.StylusTipTransform to the EPC!");
                }
            }
            finally
            {
                da.AttributeChanged -= daChangedAnonymousDelegate;
            }
        }
    }
}
