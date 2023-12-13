// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  GMC API Tests - Testing GeometryCollection class
//  Author:   Microsoft
//
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.Globalization;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Summary description for GeometryCollection.
    /// </summary>
    internal class GeometryCollectionClass : ApiTest
    {
        public GeometryCollectionClass( double left, double top, double width, double height)
            : base(left, top, width, height)
        {
            Update();
        }

        protected override void OnRender(DrawingContext DC)
        {
            CommonLib.LogStatus("GeometryCollection Class");

            string objectType = "System.Windows.Media.GeometryCollection";

            //Getting the exception message directly from the ExceptionStringTable in PresentationCore.dll in order
            //to the trouble of prepare all of those localized exception strings running on different locales.
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("FxResources.PresentationCore.SR", typeof(PathFigure).Assembly);

            #region Section I: Initialization stage
            #region Test #1 - Default constructor
            CommonLib.LogStatus("Test #1: Default Constructor");
            GeometryCollection gc1 = new GeometryCollection();
            CommonLib.TypeVerifier(gc1, objectType);
            #endregion

            #region Test #2 - .ctor (IEnumerable<Geometry> collection)
            CommonLib.LogStatus("Test #2 - .ctor (IEnumerable<Geometry> collection)");
            GeometryCollection gc2 = new GeometryCollection(new Geometry[] { new PathGeometry(), new EllipseGeometry() });
            CommonLib.TypeVerifier(gc2, objectType);
            #endregion

            #region Test #3 - Copy constructor
            CommonLib.LogStatus("Test #3 - Copy constructor");
            GeometryCollection gc3 = gc1.Clone();
            CommonLib.TypeVerifier(gc3, objectType);
            #endregion
            #endregion

            #region Section II: GeometryCollection properties testing
            #region Test #4: ICollection.IsSynchronized property
            CommonLib.LogStatus("Test #4: ICollection.IsSynchronized property");
            GeometryCollection gc4 = new GeometryCollection();
            bool result4 = ((ICollection)gc4).IsSynchronized;
            CommonLib.GenericVerifier(result4 == true, "ICollection.IsSynchronized property");
            #endregion
            #region Test #5: CanFreeze property
            CommonLib.LogStatus("Test #5: CanFreeze property");
            CommonLib.GenericVerifier(gc1.CanFreeze, "CanFreeze property");
            #endregion

            #region Test #7: Indexer property - Getter
            CommonLib.LogStatus("Test #7: Indexer property");
            GeometryCollection gc7 = new GeometryCollection(new Geometry[] { new RectangleGeometry(), new LineGeometry(), new PathGeometry() });
            Geometry g7 = gc7[1];
            CommonLib.GenericVerifier(g7 != null && g7 is LineGeometry, "Indexer property - Getter");
            #endregion

            #region Test #8: Indexer property - Setter
            CommonLib.LogStatus("Test #8: Indexer property - Setter");
            GeometryCollection gc8 = new GeometryCollection(new Geometry[] { new RectangleGeometry(), new LineGeometry(), new PathGeometry() });
            gc8[0] = new PathGeometry();
            CommonLib.GenericVerifier(gc8[0] is PathGeometry, "Indexer property - Setter");
            #endregion

            #region Test #9: Count property
            CommonLib.LogStatus("Test #9: Count property");
            GeometryCollection gc9 = new GeometryCollection();
            GeometryCollection gc9_2 = new GeometryCollection(new Geometry[] { new LineGeometry() });
            CommonLib.GenericVerifier(gc9.Count == 0 && gc9_2.Count == 1, "Count property");
            #endregion
            #endregion

            #region Section III: GeometryCollection public method
            #region Test #10: Add method
            CommonLib.LogStatus("Test #10: Add property");
            GeometryCollection gc10 = new GeometryCollection();
            gc10.Add(new LineGeometry());
            gc10.Add(new RectangleGeometry());
            CommonLib.GenericVerifier(gc10.Count == 2 && gc10[1] is RectangleGeometry, "Add method");
            #endregion

            #region Test #11: Clear method
            CommonLib.LogStatus("Test #11: Clear method");
            GeometryCollection gc11 = new GeometryCollection(new Geometry[] { new LineGeometry(), new LineGeometry(), new LineGeometry(), new LineGeometry() });
            gc11.Clear();
            CommonLib.GenericVerifier(gc11.Count == 0, "Clear()");
            #endregion

            #region Test #12: Contains()
            CommonLib.LogStatus("Test #12: Contains()");
            GeometryCollection gc12 = new GeometryCollection(new Geometry[] { new LineGeometry() });
            bool ret12 = gc12.Contains(gc12[0]);
            CommonLib.GenericVerifier(ret12 == true && !gc12.Contains(new RectangleGeometry()), "Contains()");
            #endregion

            #region Test #13: Copy()
            CommonLib.LogStatus("Test #13: Copy()");
            GeometryCollection gc13 = new GeometryCollection(new Geometry[] { new LineGeometry(new Point(3, 2), new Point(102.32, 42.12)) });
            GeometryCollection gc13_1 = gc13.Clone();
            CommonLib.GenericVerifier(gc13_1.Count == 1 && gc13_1[0] is LineGeometry, "Copy()");
            #endregion

            #region Test #14: CopyTo()
            CommonLib.LogStatus("Test #14: CopyTo()");
            GeometryCollection gc14 = new GeometryCollection();
            gc14.Add(new LineGeometry());
            gc14.Add(new LineGeometry());
            gc14.Add(new LineGeometry());
            gc14.Add(new LineGeometry());
            gc14.Add(new LineGeometry());

            Geometry[] gl14 = new Geometry[6];

            gc14.CopyTo(gl14, 0);

            CommonLib.GenericVerifier(gl14[1] is LineGeometry, "CopyTo()");
            #endregion

            #region Test #15: CloneCurrentValue()
            CommonLib.LogStatus("Test #15: CloneCurrentValue()");
            GeometryCollection gc15 = new GeometryCollection(new Geometry[] { new LineGeometry(new Point(12, 3), new Point(100, 22)) });
            GeometryCollection retGC15 = gc15.CloneCurrentValue();
            CommonLib.GenericVerifier(retGC15 != null && retGC15.Count == 1 && retGC15[0] is LineGeometry, "CloneCurrentValue()");
            #endregion

            #region Test #16: IndexOf()
            CommonLib.LogStatus("Test #16: IndexOf()");
            GeometryCollection gc16 = new GeometryCollection();
            LineGeometry lg16 = new LineGeometry(new Point(0, 23), new Point(32, 23));
            gc16.Add(lg16);
            int ret16 = gc16.IndexOf(lg16);
            CommonLib.GenericVerifier(ret16 == 0, "IndexOf()");
            #endregion

            #region Test #17: Insert()
            CommonLib.LogStatus("Test #17: Insert()");
            GeometryCollection gc17 = new GeometryCollection();
            gc17.Insert(0, new RectangleGeometry(new Rect(43, 12, 32.23, 12)));
            CommonLib.GenericVerifier(gc17.Count == 1 && gc17[0] is RectangleGeometry, "Insert()");
            #endregion

            #region Test #18: Remove()
            CommonLib.LogStatus("Test #18: Remove()");
            GeometryCollection gc18 = new GeometryCollection();
            LineGeometry lg18 = new LineGeometry(new Point(32, 12), new Point(1.32, -1200.32));
            gc18.Add(lg18);
            gc18.Remove(lg18);
            CommonLib.GenericVerifier(gc18.Count == 0, "Remove()");
            #endregion

            #region Test #19: RemoveAt()
            CommonLib.LogStatus("Test #19: RemoveAt()");
            GeometryCollection gc19 = new GeometryCollection();
            gc19.Add(new LineGeometry());
            gc19.Add(new RectangleGeometry());
            gc19.Add(new PathGeometry());
            gc19.Add(new EllipseGeometry());
            gc19.RemoveAt(3);
            CommonLib.GenericVerifier(gc19.Count == 3 && gc19[2] is PathGeometry, "RemoveAt()");
            #endregion

            #region Test #20: IEnumerable<Geometry>.GetEnumerator()
            CommonLib.LogStatus("IEnumerable<Geometry>.GetEnumerator()");
            GeometryCollection gc20 = new GeometryCollection();
            object ret20 = ((System.Collections.Generic.IEnumerable<Geometry>)gc20).GetEnumerator();
            CommonLib.GenericVerifier(ret20 is System.Collections.Generic.IEnumerator<Geometry>, "IEnumerable<Geometry>.GetEnumerator()");
            #endregion

            #region Test #21: IEnumerable.GetEnumerator()
            CommonLib.LogStatus("Test #21: IEnumerable.GetEnumerator()");
            GeometryCollection gc21 = new GeometryCollection();
            IEnumerator ret21 = ((IEnumerable)gc21).GetEnumerator();
            CommonLib.GenericVerifier(ret21 != null, "IEnumerable.GetEnumerator()");
            #endregion

            #region Test #22: IList.IsReadOnly()
            CommonLib.LogStatus("Test #22: IList.IsReadOnly");
            GeometryCollection gc22 = new GeometryCollection();
            bool ret22_ExpectedNoFrozen = ((IList)gc22).IsReadOnly;
            gc22.Freeze();
            bool ret22_ExpectedReadOnly = ((IList)gc22).IsReadOnly;
            CommonLib.GenericVerifier(ret22_ExpectedNoFrozen == false && ret22_ExpectedReadOnly == true, "IList.IsReadOnly");
            #endregion

            #region Test #23: IList.IsFixedSize
            CommonLib.LogStatus("Test #23: IList.IsFixedSize");
            GeometryCollection gc23 = new GeometryCollection();
            bool ret23_ExpectedNotFixedSize = ((IList)gc23).IsFixedSize;
            gc23.Freeze();
            bool ret23_ExpectedFixedSize = ((IList)gc23).IsFixedSize;
            CommonLib.GenericVerifier(ret23_ExpectedNotFixedSize == false && ret23_ExpectedFixedSize == true, "IList.IsFixedSize");
            #endregion

            #region Test #24: IList.this[index]
            CommonLib.LogStatus("Test #24: IList.this[index]");
            GeometryCollection gc24 = new GeometryCollection();
            gc24.Add(new LineGeometry());
            gc24.Add(new RectangleGeometry(new Rect(23, 12, 100, 32)));
            object ret24 = ((IList)gc24)[1];
            CommonLib.GenericVerifier(ret24 is RectangleGeometry && ((RectangleGeometry)ret24).Rect.TopLeft == new Point(23, 12), "IList.this[index]");
            #endregion

            #region Test #25: IList.Add(object)
            CommonLib.LogStatus("Test #25: IList.Add(object)");
            GeometryCollection gc25 = new GeometryCollection();
            int ret25 = ((IList)gc25).Add(new LineGeometry());
            CommonLib.GenericVerifier(ret25 == 0 && gc25.Count == 1, "IList.Add(object)");
            #endregion

            #region Test #26: IList.Contains(object)
            CommonLib.LogStatus("Test #26: IList.Contains(object)");
            GeometryCollection gc26 = new GeometryCollection();
            gc26.Add(new LineGeometry());
            RectangleGeometry rc26 = new RectangleGeometry(new Rect(2, -323, 23, 45));
            gc26.Add(rc26);
            gc26.Add(new EllipseGeometry());
            bool ret26 = ((IList)gc26).Contains(rc26);
            CommonLib.GenericVerifier(ret26 == true && ((IList)gc26).Contains(new LineGeometry()) == false, "IList.Contains(object");
            #endregion

            #region Test #27: IList.IndexOf(object)
            CommonLib.LogStatus("Test #27: IList.IndexOf(object)");
            GeometryCollection gc27 = new GeometryCollection();
            LineGeometry lg27 = new LineGeometry(new Point(2, 3), new Point(10, 23));
            gc27.Add(lg27);
            gc27.Add(new LineGeometry(new Point(2, 3), new Point(100, -12)));
            int ret27 = ((IList)gc27).IndexOf(lg27);
            CommonLib.GenericVerifier(ret27 == 0, "IList.IndexOf(object)");
            #endregion

            #region Test #28:  IList.Insert(int, object)
            CommonLib.LogStatus("Test #28: IList.Insert(int, object)");
            GeometryCollection gc28 = new GeometryCollection();
            ((IList)gc28).Insert(0, new LineGeometry());
            CommonLib.GenericVerifier(gc28.Count == 1 && gc28[0] is LineGeometry, "IList.Insert(int, object)");
            #endregion

            #region Test #29: IList.Remove(object)
            CommonLib.LogStatus("Test #29: IList.Remove(Object)");
            GeometryCollection gc29 = new GeometryCollection();
            gc29.Add(new RectangleGeometry());
            EllipseGeometry eg29 = new EllipseGeometry(new Rect(10, 10, 100, 220));
            gc29.Add(eg29);
            gc29.Add(eg29);
            gc29.Add(new LineGeometry());
            ((IList)gc29).Remove(eg29);
            CommonLib.GenericVerifier(gc29.Count == 3, "IList.Remove(ojbect)");
            #endregion

            #region Test #30: ICollection.CopyTo(Array, index)
            CommonLib.LogStatus("Test #30: ICollecction.CopyTo(Array, index)");
            GeometryCollection gc30 = new GeometryCollection();
            gc30.Add(new LineGeometry());
            gc30.Add(new EllipseGeometry());
            Geometry[] g30 = new Geometry[10];
            ((ICollection)gc30).CopyTo(g30, 4);
            CommonLib.GenericVerifier(g30[4] is LineGeometry && g30[5] is EllipseGeometry, "ICollection.CopyTo(Array, index)");
            #endregion

            #region Test #31: ICollection.SyncRoot
            CommonLib.LogStatus("Test #31: ICollection.SyncRoot");
            GeometryCollection gc31 = new GeometryCollection();
            gc31.Add(new PathGeometry());
            GeometryCollection ret31 = ((ICollection)gc31).SyncRoot as GeometryCollection;
            CommonLib.GenericVerifier(ret31 != null && ret31.Count == 1 && ret31[0] is PathGeometry, "ICollection.SyncRoot");
            #endregion

            #region Test #32: GeometryEnumerator.MoveNext()
            CommonLib.LogStatus("Test #32: GeometryEnumerator.MoveNext()");
            GeometryCollection gc32 = new GeometryCollection();
            gc32.Add(new LineGeometry());
            gc32.Add(new RectangleGeometry());
            IEnumerator ret32 = ((IEnumerable)gc32).GetEnumerator();
            ret32.MoveNext();
            ret32.MoveNext();
            CommonLib.GenericVerifier(ret32.Current != null && ret32.Current is RectangleGeometry, "GeometryEnumerator.MoveNext()");
            #endregion

            #region Test #33: GeometryEnumerator.Reset()
            CommonLib.LogStatus("Test #33: GeometryEnumerator.Reset()");
            GeometryCollection gc33 = new GeometryCollection();
            gc33.Add(new LineGeometry());
            gc33.Add(new RectangleGeometry());
            IEnumerator ret33 = ((IEnumerable)gc33).GetEnumerator();
            ret33.MoveNext();
            ret33.MoveNext();
            ret33.Reset();
            ret33.MoveNext();
            CommonLib.GenericVerifier(ret33.Current != null && ret33.Current is LineGeometry, "GeometryEnumerator.Reset()");
            #endregion
            #endregion

            #region Section IV : Error Handling
            #region Test #34: ArgumentException in .ctor(IEnumerable<Geometry>)
            CommonLib.LogStatus("Test #34: ArgumentException in .ctor(IEnumerable<Geometry)");
            try
            {
                GeometryCollection gc34 = new GeometryCollection(new Geometry[] { (Geometry)null });
            }
            catch (System.ArgumentException e)
            {
                string expectedMessage = rm.GetString("Collection_NoNull");
                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #34 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #34");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #34");
            }
            #endregion
            #region Test #35: ArgumentException for Geometry = NULL in Insert(int, Geometry)
            CommonLib.LogStatus("Test #35: ArgumentException for Geometry = NULL in Insert(int, Geometry)");
            try
            {
                GeometryCollection gc35 = new GeometryCollection();
                gc35.Insert(0, null);
            }
            catch (System.ArgumentException e)
            {
                string expectedMessage = rm.GetString("Collection_NoNull");
                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #35 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #35");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #35");
            }
            #endregion
            #region Test #36: ArgumentException for Value = NULL in Indexer
            CommonLib.LogStatus("Test #36: ArgumentException for Value = NULL in Indexer");
            try
            {
                GeometryCollection gc36 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                gc36[0] = null;
            }
            catch (System.ArgumentException e)
            {
                string expectedMessage = rm.GetString("Collection_NoNull");
                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #36 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #36");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #36");
            }
            #endregion
            #region Test #37: ArgumentNULLException for array=null for CopyTo(array, index)
            CommonLib.LogStatus("Test #37: ArgumentNULLException for array=null for CopyTo(array, index)");
            try
            {
                GeometryCollection gc37 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                gc37.CopyTo((Geometry[])null, 0);
            }
            catch (System.ArgumentNullException e)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "array");

                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #37 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #37");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #37");
            }
            #endregion

            #region Test #38: ArgumentOutOfRangeException for Index in CopyTo(array, index)
            CommonLib.LogStatus("Test #38: ArgumentOutOfRangeException for Index in CopyTo(array, index)");
            try
            {
                GeometryCollection gc38 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                gc38.CopyTo(new Geometry[] { new LineGeometry() }, 1000);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentOutOfRangeException), "index");   

                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #38 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #38");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #38");
            }
            #endregion

            #region Test #39: ArgumentNULLException for array=null for CopyTo(array, index)
            CommonLib.LogStatus("Test #39: ArgumentNULLException for array=null for ICollection.CopyTo(array, index)");
            try
            {
                GeometryCollection gc39 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                ((ICollection)gc39).CopyTo((Geometry[])null, 0);
            }
            catch (System.ArgumentNullException e)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentNullException), "array");

                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #39 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #39");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #39");
            }
            #endregion

            #region Test #40: ArgumentOutOfRangeException for Index in ICollection.CopyTo(array, index)
            CommonLib.LogStatus("Test #40: ArgumentOutOfRangeException for Index in ICollection.CopyTo(array, index)");
            try
            {
                GeometryCollection gc40 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                ((ICollection)gc40).CopyTo(new Geometry[] { new LineGeometry() }, 1000);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                string expectedMessage = Exceptions.GetExceptionMessage(typeof(ArgumentOutOfRangeException), "index");

                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #40 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #40");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #40");
            }
            #endregion
            #region Test #41: ArgumentException for Cast(value)
            CommonLib.LogStatus("Test #41: ArgumentException for Cast(value)");
            try
            {
                GeometryCollection gc41 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                ((IList)gc41)[10] = 10;
            }
            catch (System.ArgumentException e)
            {
                string expectedMessage = rm.GetString("Collection_BadType");
                expectedMessage = string.Format(expectedMessage, typeof(GeometryCollection).Name, typeof(Int32).Name, "Geometry");
                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #41 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #41");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #41");
            }
            #endregion
            #region Test #42: ArgumentException for AddHelper(Geometry)
            CommonLib.LogStatus("Test #42: ArgumentException for Cast(value)");
            try
            {
                GeometryCollection gc42 = new GeometryCollection(new Geometry[] { new LineGeometry() });
                gc42.Add(null);
            }
            catch (System.ArgumentException e)
            {
                string expectedMessage = rm.GetString("Collection_NoNull");
                if (String.Compare(e.Message, expectedMessage, false, System.Globalization.CultureInfo.InvariantCulture) == 0)
                {
                    CommonLib.LogStatus("Correct Exception is thrown:  " + e.Message);
                    CommonLib.LogStatus("Case #42 passes");
                }
                else
                {
                    CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                    CommonLib.LogFail("Test fails at case #42");
                }
            }
            catch (Exception e)
            {
                CommonLib.LogStatus("Incorrect exception being thrown: " + e.Message);
                CommonLib.LogFail("Test fails at case #42");
            }
            #endregion
            #endregion

            #region Section V : Running stage
            CommonLib.Stage = TestStage.Run;

            CommonLib.LogTest("Result for:" + objectType);

            #endregion
        }
    }
}