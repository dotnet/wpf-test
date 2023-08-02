using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    using TestHandlers = ListPagerTestEventHandlers;
    using Validation = ListPagerTestValidationCommon;

    /// <summary>
    /// 
    /// </summary>
    public static class ListPagerTestEvent
    {
        // example XTC // --------------------------------------------------------------------------
        //<ListPagerTestEvent.ChangePageIndexAndVerifyEvent TargetElement="{SceneTreeSearch:PROXY}"
        //                                                  Handlers="{SceneTreeSearch:HANDLERS}"
        //                                                  OldIndex="4"
        //                                                  NewIndex="1"/>
        // -----------------------------------------------------------------------------------------
        /// <summary>
        /// Method to attach an event handler, verify and change the page index,
        /// and then verifiy that ListPager fired the CurrentPageIndexChanged event.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Handlers">An instantiated ListPagerTestEventHandlers object</param>
        /// <param name="OldIndex">The expected 'before' CurrentPageIndex expected value</param>
        /// <param name="NewIndex">The intended 'after' CurrentPageIndex property value</param>
        /// <returns></returns>
        public static bool ChangePageIndexAndVerifyEvent(ContentControl TargetElement,
                                                         ContentControl Handlers,
                                                         int OldIndex,
                                                         int NewIndex)
        {
                UIListPager thePager = (UIListPager)TargetElement.Content;
                TestHandlers handlers = (TestHandlers)Handlers.Content;

                bool retval = true;
                if (thePager.CurrentPageIndex != OldIndex)
                {
                    TestLog.Current.LogEvidence("!!!ERROR: CurrentPageIndex is unexpected value.");
                    TestLog.Current.LogEvidence("..Detail: Actual value == "
                        + thePager.CurrentPageIndex.ToString() + "; Expected value == " + OldIndex.ToString());
                    retval = false;
                }
                else
                {
                    thePager.CurrentPageIndexChanged += handlers.PageIndexChangedHandler;
                    thePager.CurrentPageIndex = NewIndex;

                    if (handlers.PageIndexChangedCount != 1)
                    {
                        TestLog.Current.LogEvidence("!!!ERROR: Unexpected - ListPager did not fire CurrentPageIndexChanged event exactly once.");
                        TestLog.Current.LogEvidence("..Detail: Actual count == " + handlers.PageIndexChangedCount);
                        retval = false;
                    }
                }
                return retval;
        }

        // example XTC // --------------------------------------------------------------------------
        //<ListPagerTestEvent.ExecuteCommandAndVerifyEvent TargetElement="{SceneTreeSearch:PROXY}"
        //                                                 Handlers="{SceneTreeSearch:HANDLERS}"
        //                                                 OldIndex="2"
        //                                                 CommandOrdinal="0"/>
        // -----------------------------------------------------------------------------------------
        /// <summary>
        /// Method to attach an event handler, verify the current page index, execute a specific
        /// UIListPagerCommand to change the index, and then verify that ListPager fired the
        /// CurrentPageIndexChanged event.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Handlers">An instantiated ListPagerTestEventHandlers object</param>
        /// <param name="OldIndex">The expected 'before' CurrentPageIndex expected value</param>
        /// <param name="CommandOrdinal">A value encoding which command to execute</param>
        /// <returns></returns>
        public static bool ExecuteCommandAndVerifyEvent(ContentControl TargetElement,
                                                        ContentControl Handlers,
                                                        int OldIndex,
                                                        int CommandOrdinal)
        {
            bool retval = true;

            UIListPager thePager = (UIListPager)TargetElement.Content;
            TestHandlers handlers = (TestHandlers)Handlers.Content;
            
            int pageCount = thePager.PageCount;
            if (CommandOrdinal < 0 )
            {
                throw new ArgumentOutOfRangeException("CommandOrdinal", "Allowable range is 0 .. (builtInCmdCount + pageCount)");
            }

            UIListPagerCommand[] commands = new UIListPagerCommand[Validation.builtInCmdCount + pageCount];
            commands[0] = thePager.FirstCommand;
            commands[1] = thePager.LastCommand;
            commands[2] = thePager.NextCommand;
            commands[3] = thePager.PreviousCommand;

            int j = Validation.builtInCmdCount;
            foreach( UIListPagerCommand cmd in thePager.PageCommands)
            {
                commands[j++] = cmd;
            }

            if ( CommandOrdinal >= (Validation.builtInCmdCount + pageCount))
            {
                TestLog.Current.LogEvidence("!!!ERROR: (PageCount + 4) <= CommandOrdinal parameter value. Either PageCount in error, or XTC parameter in errror.");
                TestLog.Current.LogEvidence("..Detail: (PageCount + 4) = " + (pageCount+4).ToString() + "; CommandOrdinal = " + CommandOrdinal.ToString());
                retval = false;
            }
            else if (thePager.CurrentPageIndex != OldIndex)
            {
                TestLog.Current.LogEvidence("!!!ERROR: CurrentPageIndex is unexpected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value == "
                    + thePager.CurrentPageIndex.ToString() + "; Expected value == " + OldIndex.ToString());
            }
            else if (commands[CommandOrdinal].CanExecute(null) == false)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Requested Command #" + CommandOrdinal.ToString() + " CanExecute() == false.");
                retval = false;
            }
            else
            {
                thePager.CurrentPageIndexChanged += handlers.PageIndexChangedHandler;
                commands[CommandOrdinal].Execute(null);

                if (handlers.PageIndexChangedCount != 1)
                {
                    TestLog.Current.LogEvidence("!!!ERROR: Unexpected - ListPager did not fire CurrentPageIndexChanged event exactly once.");
                    TestLog.Current.LogEvidence("..Detail: Actual count == " + handlers.PageIndexChangedCount);
                    retval = false;
                }
            }
            return retval;
        }

        // example XTC // --------------------------------------------------------------------------
        //<ListPagerTestEvent.ChangePageSizeAndVerifyEvent TargetElement="{SceneTreeSearch:PROXY}"
        //                                               Handlers="{SceneTreeSearch:HANDLERS}"
        //                                               OldSize=""
        //                                               NewSize=""/>
        // -----------------------------------------------------------------------------------------
        /// <summary>
        /// Method to attach an event handler, verify the current page size, assign a new page size,
        /// and then verify that ListPager fired the PageSizeChanged event.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Handlers">An instantiated ListPagerTestEventHandlers object</param>
        /// <param name="OldSize">The expected 'before' PageSize</param>
        /// <param name="NewSize">The expected 'after' PageSize</param>
        /// <returns></returns>
        public static bool ChangePageSizeAndVerifyEvent(ContentControl TargetElement,
                                                        ContentControl Handlers,
                                                        int OldSize,
                                                        int NewSize)
        {
            UIListPager thePager = (UIListPager)TargetElement.Content;
            TestHandlers handlers = (TestHandlers)Handlers.Content;

            bool retval = true;
            if (thePager.PageSize != OldSize)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Current PageSize is unexpected value.");
                TestLog.Current.LogEvidence("..Detail: Actual value == "
                    + thePager.PageSize.ToString() + "; Expected value == " + OldSize.ToString());
                retval = false;
            }
            else
            {
                thePager.PageSizeChanged += handlers.PageSizeChangedHandler;
                thePager.PageSize = NewSize;

                if (handlers.PageSizeChangedCount != 1)
                {
                    TestLog.Current.LogEvidence("!!!ERROR: Unexpected - ListPager did not fire PageSizeChanged event exactly once.");
                    TestLog.Current.LogEvidence("..Detail: Actual count == " + handlers.PageSizeChangedCount);
                    retval = false;
                }
            }
            return retval;
        }

        // example XTC // --------------------------------------------------------------------------
        //<ListPagerTestEvent.ChangeItemsSourceAndVerifyEvent TargetElement="{SceneTreeSearch:PROXY}"
        //                                                    Handlers="{SceneTreeSearch:HANDLERS}"
        //                                                    Items="{SceneTreeSearch:LISTPAGERDATA_2"/>
        // -----------------------------------------------------------------------------------------
        /// <summary>
        /// Method to attach an event handler, replace the ItemsSource managed by the ListPager, and then verify
        /// that ListPager fired the ItemsSourceChanged event.
        /// </summary>
        /// <param name="TargetElement">The ListPager instance</param>
        /// <param name="Handlers">An instantiated ListPagerTestEventHandlers object</param>
        /// <param name="Items">ItemsControl whose Items collection elements will be used to construct
        /// an array for the new ListPager.ItemsSource</param>
        /// <returns></returns>
        public static bool ChangeItemsSourceAndVerifyEvent(ContentControl TargetElement, ContentControl Handlers, ItemsControl Items)
        {

            UIListPager thePager = (UIListPager)TargetElement.Content;
            TestHandlers handlers = (TestHandlers)Handlers.Content;
            int itemsCount = Items.Items.Count;

            // Have to copy elements into a collection that does not expose INotifyCollectionChanged
            // ListPager explicitly tests for that, and throws an exception in that case.
            object[] itemsArray = new object[itemsCount];
            (Items.Items).CopyTo(itemsArray, 0);

            thePager.ItemsSourceChanged += handlers.ItemsSourceChangedHandler;
            thePager.ItemsSource = itemsArray;

            bool retval = true;
            if (handlers.ItemsSourceChangedCount != 1)
            {
                TestLog.Current.LogEvidence("!!!ERROR: Unexpected - ListPager did not fire ItemsSourceChanged exactly once.");
                TestLog.Current.LogEvidence("..Detail: Actual count == " + handlers.ItemsSourceChangedCount);
                retval = false;
            }
            return retval;
        }
    }

    /// <summary>
    /// An event-handler & counter pair for the three ListPager events. An instance of this class is a required
    /// argument to each of the ListPagerTestEvent static class' methods. (Note the three source-code regions)
    /// </summary>
    public class ListPagerTestEventHandlers
    {
        public ListPagerTestEventHandlers()
        {
            pageIndexChangedCount = 0;
            pageIndexChangedHandler = new EventHandler(handlePageIndexChanged);

            itemsSourceChangedCount = 0;
            itemsSourceChangedHandler = new EventHandler(handleItemsSourceChanged);

            pageSizeChangedCount = 0;
            pageSizeChangedHandler = new EventHandler(handlePageSizedChanged);
        }

        #region pageIndex

        private int pageIndexChangedCount;
        public int PageIndexChangedCount { get { return pageIndexChangedCount; } }

        private void handlePageIndexChanged(Object sender, EventArgs args)
        {
            pageIndexChangedCount++;
        }
        private EventHandler pageIndexChangedHandler;

        public EventHandler PageIndexChangedHandler
        {
            get { return pageIndexChangedHandler; }
        }

        #endregion
        // ==============================================================================
        #region itemsSource

        private int itemsSourceChangedCount;
        public int ItemsSourceChangedCount { get { return itemsSourceChangedCount; } }

        private void handleItemsSourceChanged(Object sender, EventArgs args)
        {
            itemsSourceChangedCount++;
        }
        private EventHandler itemsSourceChangedHandler;

        public EventHandler ItemsSourceChangedHandler
        {
            get { return itemsSourceChangedHandler; }
        }

        #endregion
        // ==============================================================================
        #region pageSize
    
        private int pageSizeChangedCount;
        public int PageSizeChangedCount { get { return pageSizeChangedCount; } }

        private void handlePageSizedChanged(Object sender, EventArgs args)
        {
            pageSizeChangedCount++;
        }
        private EventHandler pageSizeChangedHandler;

        public EventHandler PageSizeChangedHandler
        {
            get { return pageSizeChangedHandler; }
        }

        #endregion

    }
}
