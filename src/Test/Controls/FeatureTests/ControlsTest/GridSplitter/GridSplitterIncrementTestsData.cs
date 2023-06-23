using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Avalon.Test.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.Data
{
    internal static class IncrementTestData
    {
        #region Private members

        /// <summary>
        /// Populate a target dictionary from a source dictionary -- shallow copies
        /// </summary>
        private static void CloneTestVariation(Dictionary<String, Object> source, Dictionary<String, Object> target)
        {
            foreach (String key in source.Keys)
            {
                target.Add(key, source[key]);
            }
        }

        /// <summary>
        /// Utility struct that uses reflection to resolve a test action specification
        /// into a valid "Microsoft.Test.Input.UserInput class" MethodInfo that can be
        /// invoked here by the "UserInputAction" GridSplitterActionDelegate.
        /// The nested struct UIActionArgs represents the array of type-checked
        /// parameter values for the invokation.
        /// </summary>
        private struct UIActionArgs
        {
            internal struct ArgDef
            {
                public readonly Type ArgType;
                public readonly Object ArgValue;
                public override String ToString()
                {
                    return ArgValue.ToString();
                }
                internal ArgDef(Type type, Object value)
                {
                    if (value.GetType() == type)
                    {
                        ArgValue = value; ArgType = type;
                    }
                    else
                    {
                        throw new TestValidationException(
                           String.Format("Initializing variation data -- UIActionArgs: ArgValue <<{0}>> is not of type <<{1}>>"
                           , value.ToString(), type.ToString()));
                    }
                }
            }

            internal readonly String ActionName;
            internal readonly String ArgValuesString;
            internal readonly Type[] ArgTypes;
            internal readonly Object[] ArgValues;
            internal readonly MethodInfo Method;

            internal UIActionArgs(String actionName, params ArgDef[] argDefs)
            {
                ArgValues = new Object[argDefs.Length];
                ArgTypes = new Type[argDefs.Length];
                StringBuilder sb = new StringBuilder();
                for (Int32 i = 0; i < argDefs.Length; ++i)
                {
                    ArgValues[i] = argDefs[i].ArgValue;
                    ArgTypes[i] = argDefs[i].ArgType;
                    sb.Append(argDefs[i].ToString());
                    if (i < (argDefs.Length - 1))
                    {
                        sb.Append(", ");
                    }
                }

                ActionName = actionName;
                Method = (typeof(UserInput)).GetMethod(actionName, ArgTypes);
                if (null == Method)
                {
                    throw new TestValidationException(
                        String.Format("Initializing variation data -- UIActionArgs: ActionName <<{0}>> not resolved to a MethodInfo for given parameter types.", actionName));
                }
                ArgValuesString = sb.ToString();
            }
        }

        #endregion

        #region (private) GridSplitterActionDelegate Implementations

        /// <summary>
        /// Delegate that invokes one or a sequence of Microsoft.Test.Input.UserInput methods
        /// as specified by the array of UIActionArgs passed as Object[].
        /// Returns two GridDefinitionSnapShot objects: "before" and "after" the invokations
        /// </summary>
        private static void UserInputAction(out GridDefinitionSnapshot snapBefore,
                                     out GridDefinitionSnapshot snapAfter,
                                     Grid grid,
                                     GridSplitter splitter,
                                     Object[] actionArgs)
        {
            // Verify parameter expectations
            if (null == actionArgs || actionArgs.Length < 1)
            {
                throw new TestValidationException("UserInputAction 'actionArgs' parameter cannont be null or empty.");
            }

            // Extract action-argument values from untyped parameters
            //
            UIActionArgs[] uiActionArgsArray = new UIActionArgs[actionArgs.Length];
            for (Int32 i = 0; i < actionArgs.Length; ++i)
            {
                if (actionArgs[i] is UIActionArgs)
                {
                    uiActionArgsArray[i] = (UIActionArgs)actionArgs[i];
                }
                else
                {
                    throw new TestValidationException("UserInputAction 'actionArgs' parameter must pass only UIActionArgs objects.");
                }
            }

            snapBefore = GridSplitterGridExtend.GetDefinitionSnapshot(grid);

            // Perform a sequence of actions
            if (splitter.Focus())
            {
                foreach (UIActionArgs args in uiActionArgsArray)
                {
                    // Search through argument list and substitute actual Splitter
                    // or Grid arguments for placeholder values. Dummy values are
                    // used as placeholders in the test-case definitions.
                    for (Int32 j = 0; j < args.ArgValues.Length; ++j)
                    {
                        if (args.ArgTypes[j] == typeof(GridSplitter)) args.ArgValues[j] = splitter;
                        else if (args.ArgTypes[j] == typeof(Grid)) args.ArgValues[j] = grid;
                    }
                    QueueHelper.WaitTillQueueItemsProcessed();
                    args.Method.Invoke(null, args.ArgValues);
                    TestLog.Current.LogStatus(
                        String.Format(">>> UserInputAction performed. Method= \"{0}\" -- parameters: \"{1}\""
                        , args.Method.Name, args.ArgValuesString));
                }
            }
            else
            {
                throw new TestValidationException("UserInputAction delegate 'splitter.Focus()' Failed.");
            }

            snapAfter = GridSplitterGridExtend.GetDefinitionSnapshot(grid);
            TestLog.Current.LogStatus("UserInputAction Finished");
        }

        #endregion

        #region (private) GridSplitterVerifyDelegate Implementations

        /// <summary>
        /// Delegate that is specified as an entry in test variation data
        /// and invoked by GridSplitterIncrementTests.RunIncrementTests().
        /// The arguments to the call come from the same test
        /// variation entry. (See class GridSplitterVariationContext)
        /// </summary>
        private static TestResult VerifyIncrementChanges(Int32 variationIndex,
                                                  Grid grid,
                                                  GridSplitter splitter,
                                                  GridSplitterActionDelegate action,
                                                  Object[] actionArgs,
                                                  Object[] verifyArgs)
        {
            TestLog.Current.LogStatus(
                "Test index: " + variationIndex.ToString() + ", Entered VerifyIncrementChanges");

            // Verify parameter expectations
            if (null == verifyArgs || verifyArgs.Length != 2
                || !(verifyArgs[0] is GridDefinitionSnapshot.RowDiff[])
                || !(verifyArgs[1] is GridDefinitionSnapshot.ColumnDiff[]))
            {
                throw new TestValidationException(
                    "VerifyIncrementChanges 'verifyArgs' parameter has wrong structure");
            }
 
            GridDefinitionSnapshot snapBefore;
            GridDefinitionSnapshot snapAfter;

            // Invoke the action delegate
            //
            action(out snapBefore, out snapAfter, grid, splitter, actionArgs);

            // Process the resulting snapshots to accomplish verification
            //
            GridDefinitionSnapshot.RowDiff[] actualRowDiffs = snapBefore.CompareRows(snapAfter);
            GridDefinitionSnapshot.ColumnDiff[] actualColumnDiffs = snapBefore.CompareColumns(snapAfter);

            // Extract values from untyped parameters
            //
            GridDefinitionSnapshot.RowDiff[] expectedRowDiffs = (GridDefinitionSnapshot.RowDiff[])verifyArgs[0];
            GridDefinitionSnapshot.ColumnDiff[] expectedColumnDiffs = (GridDefinitionSnapshot.ColumnDiff[])verifyArgs[1];

            // Execute the verification details
            //
            if (!GridDefinitionSnapshot.RowDiff.CompareArrays(expectedRowDiffs, actualRowDiffs))
            {
                StringBuilder sb = new StringBuilder("\n<< Row Errors (Note--only first two fields are significant)>>");
                sb.AppendLine();
                for (Int32 i = 0; i < actualRowDiffs.Length; ++i)
                {
                    sb.AppendFormat("Index {0} << EXPECTED >> {1}\n        <<  ACTUAL  >> {2}\n",
                        i.ToString(), (expectedRowDiffs[i]).ToString(), (actualRowDiffs[i]).ToString());
                    sb.AppendLine();
                }
                throw new TestValidationException(sb.ToString());
            }

            if (!GridDefinitionSnapshot.ColumnDiff.CompareArrays(expectedColumnDiffs, actualColumnDiffs))
            {
                StringBuilder sb = new StringBuilder("\n<< Column Errors (Note--only first two fields are significant)>>");
                sb.AppendLine();
                for (Int32 i = 0; i < actualColumnDiffs.Length; ++i)
                {
                    sb.AppendFormat("Index {0} << EXPECTED >> {1}\n        <<  ACTUAL  >> {2}\n",
                        i.ToString(), (expectedColumnDiffs[i]).ToString(), (actualColumnDiffs[i]).ToString());
                    sb.AppendLine();
                }
                throw new TestValidationException(sb.ToString());
            }

            TestLog.Current.LogStatus("Finished VerifyIncrementChanges");
            return TestResult.Pass;
        }
        
        #endregion

        #region Public methods

        /// <summary>
        /// Test-case specification data for the "KeyboardIncrement" test variation.
        /// Builds a dictionary of test specifications that are executed by 
        /// GridSplitterResizeAndIncrementTests.RunTests()
        /// </summary>
        public static Dictionary<String, Object>[] InitializeKeyboardIncrementVariations()
        {
            Dictionary<String, Object> testVariation;
            Dictionary<String, Object>[] testVariations = new Dictionary<String, Object>[24];
            //---- [0 ] comment --------------------------
            // 'previous' is non-existent expect unchanged for GridResizeBehavior.PreviousAndNext
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)0);
            testVariation.Add("SplitterColor", Brushes.Red);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)35);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs",
                new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[0] = testVariation;

            //---- [1 ] comment -------------------------
            // 'previous' is non-existent expect unchanged for GridResizeBehavior.PreviousAndNext
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["ActionArgs"] = new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) };
            testVariations[1] = testVariation;

            //---- [2 ] comment --------------------------
            // 'next' is non-existent expect unchanged for GridResizeBehavior.PreviousAndNext
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["Grid.Column"] = (Int32)3;
            testVariations[2] = testVariation;

            //---- [3 ] comment -------------------------
            // Column [1] shifted Right ( 3 * increment)
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["Grid.Column"] = (Int32)1;
            testVariation["KeyboardIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[]
            { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right"))
            };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(150,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,150),
                          new GridDefinitionSnapshot.ColumnDiff(-150,150),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)}
             };
            testVariations[3] = testVariation;

            //---- [4 ] comment -------------------------
            // Column [1] shifted Left (3 * increment)
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["Grid.Column"] = (Int32)1;
            testVariation["KeyboardIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[]
            { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left"))
            };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-150,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,-150),
                          new GridDefinitionSnapshot.ColumnDiff(150,-150),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)}
             };
            testVariations[4] = testVariation;

            //---- [5 ]comment --------------------------
            // Column [1] shifted Right (3 * increment + remaining width)
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["Grid.Column"] = (Int32)1;
            testVariation["KeyboardIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[]
            { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right"))
            };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(175,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,175),
                          new GridDefinitionSnapshot.ColumnDiff(-175,175),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
             };
            testVariations[5] = testVariation;

            //---- [6 ] comment -------------------------
            // (note: Column widths are assumed to be 175 pixel units each)
            // Column [1] shifted Right (3 * increment + remaining width)
            // then shifted Left ( 4 * increment)
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["Grid.Column"] = (Int32)1;
            testVariation["KeyboardIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[]
            { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
              new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left"))
            };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-25,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,-25),
                          new GridDefinitionSnapshot.ColumnDiff(25,-25),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)}
             };
            testVariations[6] = testVariation;

            //---- [7 ] comment -------------------------
            // 'GridBravo' Column[2] constrained by MinWidth=165. This limits
            // GridSplitter movement 'Key.Right' to less than one increment.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridBravo");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Blue);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(10,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,10),
                          new GridDefinitionSnapshot.ColumnDiff(-10,10),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[7] = testVariation;

            //---- [8 ] comment -------------------------
            // 'GridBravo' Column[0] constrained by MinWidth=165. This limits
            // GridSplitter movement 'Key.Leftt' to less than one increment.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[7], testVariation);
            testVariation["ActionArgs"] = new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) };
            testVariation["VerifyArgs"] = new Object[]
                 { new GridDefinitionSnapshot.RowDiff[]
                            { new GridDefinitionSnapshot.RowDiff(0,0),
                              new GridDefinitionSnapshot.RowDiff(0,0),
                              new GridDefinitionSnapshot.RowDiff(0,0),
                              new GridDefinitionSnapshot.RowDiff(0,0)},
                   new GridDefinitionSnapshot.ColumnDiff[]
                            { new GridDefinitionSnapshot.ColumnDiff(-10,0),
                              new GridDefinitionSnapshot.ColumnDiff(0,-10),
                              new GridDefinitionSnapshot.ColumnDiff(10,-10),
                              new GridDefinitionSnapshot.ColumnDiff(0,0)}
                 };
            testVariations[8] = testVariation;

            //---- [9 ] comment -------------------------
            // GridResizeBehavior.PreviousAndNext
            // 'previous is non-existent so System.Windows.Input.Key.Right has no effect
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)0);
            testVariation.Add("SplitterColor", Brushes.Coral);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndCurrent);
            testVariation.Add("WidthHeight", new Size(10, 10));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[9] = testVariation;

            //---- [10] comment -------------------------
            // GridResizeBehavior.PreviousAndNext
            // 'previous is non-existent, so System.Windows.Input.Key.Left has no effect
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[9], testVariation);
            testVariation["ActionArgs"] = new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) };
            testVariations[10] = testVariation;

            //---- [11 ] comment ------------------------
            // GridResizeBehavior.PreviousAndCurrent
            // First in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.CadetBlue);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndCurrent);
            testVariation.Add("MinWidthHeight", new Size(10, 10));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Left);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] 
            { 
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left"))
            });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(50,0),
                          new GridDefinitionSnapshot.ColumnDiff(-50,50),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[11] = testVariation;

            //---- [12] comment -------------------------
            // GridResizeBehavior.PreviousAndCurrent
            // Second in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[11], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[12] = testVariation;

            //---- [13] comment -------------------------
            // GridResizeBehavior.PreviousAndCurrent
            // Third in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[11], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[13] = testVariation;

            //---- [14] comment -------------------------
            // GridResizeBehavior.PreviousAndCurrent
            // �current� Column MinWidth constraint limits compression
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridCharlie");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.HotPink);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndCurrent);
            testVariation.Add("WidthHeight", new Size(20, 20));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Center);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Center);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(10,0),
                          new GridDefinitionSnapshot.ColumnDiff(-10,10),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[14] = testVariation;

            //---- [15 ] comment ------------------------
            // GridResizeBehavior.PreviousAndCurrent
            //�current� Column MaxWidth constraint limits expansion
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[14], testVariation);
            testVariation["ActionArgs"] = new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-10,0),
                          new GridDefinitionSnapshot.ColumnDiff(10,-10),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)}
             };
            testVariations[15] = testVariation;

            //---- [16] comment -------------------------
            // GridResizeBehavior.CurrentAndNext
            // 'next' is non-existent so System.Windows.Input.Key.Right has no effect
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)3);
            testVariation.Add("SplitterColor", Brushes.Orange);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.CurrentAndNext);
            testVariation.Add("WidthHeight", new Size(10, 10));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[16] = testVariation;

            //---- [17] comment -------------------------
            // GridResizeBehavior.CurrentAndNext
            // 'previous is non-existent, so System.Windows.Input.Key.Left has no effect
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[16], testVariation);
            testVariation["ActionArgs"] = new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) };
            testVariations[17] = testVariation;

            //---- [18] comment ------------------------
            // GridResizeBehavior.CurrentAndNext
            // First in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)2);
            testVariation.Add("SplitterColor", Brushes.Cyan);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.CurrentAndNext);
            testVariation.Add("MinWidthHeight", new Size(10, 10));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Left);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] 
            { 
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right"))
            });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(-50,0),
                          new GridDefinitionSnapshot.ColumnDiff(50,-50)
                        }
            });
            testVariations[18] = testVariation;

            //---- [19] comment -------------------------
            // GridResizeBehavior.CurrentAndNext
            // Second in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[18], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;
            testVariations[19] = testVariation;

            //---- [20] comment -------------------------
            // GridResizeBehavior.CurrentAndNext
            // Third in group of 3 tests to show any differences due
            // to  HorizontalAlignment -- given GridSplitter.MinWidth and MinHeight are specified
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[18], testVariation);
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;
            testVariations[20] = testVariation;

            //---- [21] comment -------------------------
            // GridResizeBehavior.CurrentAndNext
            // �current� Column MinWidth constraint limits compression
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridCharlie");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)2);
            testVariation.Add("SplitterColor", Brushes.Magenta);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.CurrentAndNext);
            testVariation.Add("WidthHeight", new Size(20, 20));
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Center);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Center);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[]
            {
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Left")) }
            );
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(-10,0),
                          new GridDefinitionSnapshot.ColumnDiff(10,-10)
                        }
            });
            testVariations[21] = testVariation;

            //---- [22] comment ------------------------
            // GridResizeBehavior.CurrentAndNext
            //�current� Column MaxWidth constraint limits expansion
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[21], testVariation);
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Right")) 
            };
            testVariation["VerifyArgs"] = new Object[]
             { new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
               new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(10,0),
                          new GridDefinitionSnapshot.ColumnDiff(-10,10)}
             };
            testVariations[22] = testVariation;

            //---- [23 ] comment -------------------------
            // 'GridBravo' Row[2] constrained by MinHeight=165. This limits
            // GridSplitter movement 'Key.Down' to less than one increment.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridDelta");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", Brushes.Magenta);
            testVariation.Add("ResizeDirection", GridResizeDirection.Rows);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("KeyboardIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[] { new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Down")) });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(10,0),
                          new GridDefinitionSnapshot.RowDiff(0,10),
                          new GridDefinitionSnapshot.RowDiff(-10,10),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[23] = testVariation;

            return testVariations;
        }

        /// <summary>
        /// Test-case specification data for the "DragIncrement" test variation.
        /// Builds a dictionary of test specifications that are executed by 
        /// GridSplitterResizeAndIncrementTests.RunTests()
        /// </summary>
        public static Dictionary<String, Object>[] InitializeDragIncrementVariations()
        {
            SolidColorBrush[] brushes = new SolidColorBrush[] { Brushes.Red, Brushes.Green, Brushes.Blue };

            Grid dummyGrid = new Grid();
            GridSplitter dummySplitter = new GridSplitter();

            Int32 idx = 0;
            Dictionary<String, Object> testVariation;
            Dictionary<String, Object>[] testVariations = new Dictionary<String, Object>[17];
            //---- [0 ] comment --------------------------
            // Simplest single-increment drag test: Row direction, positive delta
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterColor", brushes[idx++%brushes.Length]);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Stretch);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("DragIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[]
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)50),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)50)
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(50,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,50),
                          new GridDefinitionSnapshot.ColumnDiff(-50,50),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[0] = testVariation;

            //---- [1] comment --------------------------
            // Simplest single-increment drag test: Row direction, negative delta
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-50)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-50))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-50,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,-50),
                          new GridDefinitionSnapshot.ColumnDiff(50,-50),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[1] = testVariation;

            //---- [2 ] comment --------------------------
            //Dragging should snap to closest drag increment multiple, on positive displacement.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)60;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)35),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)35)
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(60,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,60),
                          new GridDefinitionSnapshot.ColumnDiff(-60,60),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[2] = testVariation;

            //---- [3 ] comment --------------------------
            // Dragging should snap to closest drag increment stop.
            // Positive delta and less than 50% of increment
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["DragIncrement"] = (Double)60;
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)25),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)25)
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[3] = testVariation;

            //---- [4 ] comment --------------------------
            // Dragging should snap to closest drag increment stop.
            // Negative delta and more than 50% of increment.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)60;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-35)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-35))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-60,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,-60),
                          new GridDefinitionSnapshot.ColumnDiff(60,-60),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[4] = testVariation;

            //---- [5 ] comment --------------------------
            // Dragging should snap to closest drag increment stop.
            // Negative delta and less than 50% of increment.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)60;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-25)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-25))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[5] = testVariation;

            //---- [6 ] comment --------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250)
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(175,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,175),
                          new GridDefinitionSnapshot.ColumnDiff(-175,175),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[6] = testVariation;

            //---- [7 ] comment --------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-250)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-250))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-175,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,-175),
                          new GridDefinitionSnapshot.ColumnDiff(175,-175),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[7] = testVariation;

            //---- [8 ] comment --------------------------
            //EXPECT: KeyPress "Escape" should cancel drag and pre-drag state should be restored.
            //ACTUAL: Known Bug
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation.Add("KNOWN_BUG", true);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250)
                    ),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Escape")),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[8] = testVariation;

            //---- [9 ] comment -------------------------
            //EXPECT: KeyPress "Escape" should cancel drag and pre-drag state should be restored.
            //NOTE: This is like test case #8 except that at least one "completed" drag has occured
            //      when a "canceled" drag is attempted.
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)50),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)50)
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)100),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)100)
                    ),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Escape")),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(50,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,50),
                          new GridDefinitionSnapshot.ColumnDiff(-50,50),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[9] = testVariation;

            //---- [10 ] comment -------------------------
            //EXPECT: KeyPress "Up" should cancel drag and pre-drag state should be restored.
            //ACTUAL: Known Bug
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[0], testVariation);
            testVariation.Add("KNOWN_BUG", true);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["DragIncrement"] = (Double)50;
            testVariation["ActionArgs"] = new Object[] 
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)250)
                    ),
                new UIActionArgs("KeyPress", new UIActionArgs.ArgDef(typeof(String), "Up")),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter))
            };
            testVariation["VerifyArgs"] = new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            };
            testVariations[10] = testVariation;

            //---- [11] comment -------------------------
            // This was intended to raise a known bug
            // GridResizeDirection.Columns + GridResizeBehavior.PreviousAndCurrent + HorizontalAlignment.Right
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterMinWidth", 10);
            testVariation.Add("SplitterMinHeight", 10);
            testVariation.Add("SplitterColor", brushes[idx++ % brushes.Length]);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.PreviousAndCurrent);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Right);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("DragIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[]
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-100)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(-100))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
            });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(-100,0),
                          new GridDefinitionSnapshot.ColumnDiff(100,-100),
                          new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[11] = testVariation;

            //---- [12] comment -------------------------
            // Similar to #11 except HorizontalAlignment.Left
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[11], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Left;

            testVariations[12] = testVariation;

            //---- [13] comment -------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[11], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;

            testVariations[13] = testVariation;

            //---- [14] comment -------------------------
            // This was intended to raise known bug
            // GridResizeDirection.Columns + GridResizeBehavior.CurrentAndNext + HorizontalAlignment.Left
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            testVariation.Add("PanelGridResourceKey", "GridAlpha");
            testVariation.Add("Grid.Row", (Int32)1);
            testVariation.Add("Grid.Column", (Int32)1);
            testVariation.Add("SplitterMinWidth", 10);
            testVariation.Add("SplitterMinHeight", 10);
            testVariation.Add("SplitterColor", brushes[idx++ % brushes.Length]);
            testVariation.Add("ResizeDirection", GridResizeDirection.Columns);
            testVariation.Add("ResizeBehavior", GridResizeBehavior.CurrentAndNext);
            testVariation.Add("HorizontalAlignment", HorizontalAlignment.Left);
            testVariation.Add("VerticalAlignment", VerticalAlignment.Stretch);
            testVariation.Add("DragIncrement", (Double)50);
            testVariation.Add(
                "Action", new GridSplitterActionDelegate(UserInputAction));
            testVariation.Add("ActionArgs", new Object[]
            {
                new UIActionArgs("MouseLeftDown", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
                new UIActionArgs("MouseMove",
                    new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(100)),
                    new UIActionArgs.ArgDef(typeof(Int32), (Int32)(100))
                    ),
                new UIActionArgs("MouseLeftUp", new UIActionArgs.ArgDef(typeof(GridSplitter), dummySplitter)),
            });
            testVariation.Add(
                "Verify", new GridSplitterVerifyDelegate(VerifyIncrementChanges));
            testVariation.Add(
                "VerifyArgs", new Object[] 
            {new GridDefinitionSnapshot.RowDiff[]
                        { new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0),
                          new GridDefinitionSnapshot.RowDiff(0,0)},
             new GridDefinitionSnapshot.ColumnDiff[]
                        { new GridDefinitionSnapshot.ColumnDiff(0,0),
                          new GridDefinitionSnapshot.ColumnDiff(100,0),
                          new GridDefinitionSnapshot.ColumnDiff(-100,100),
                          new GridDefinitionSnapshot.ColumnDiff(0,0)
                        }
            });
            testVariations[14] = testVariation;

            //---- [15] comment -------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[14], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Right;

            testVariations[15] = testVariation;

            //---- [16] comment -------------------------
            //-------------------------------------------
            testVariation = new Dictionary<String, Object>();
            CloneTestVariation(testVariations[14], testVariation);
            testVariation["SplitterColor"] = brushes[idx++ % brushes.Length];
            testVariation["HorizontalAlignment"] = HorizontalAlignment.Center;

            testVariations[16] = testVariation;

            //-------------------------------------------
            return testVariations;
        }
        
        #endregion
    }
}
