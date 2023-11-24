//using System;
//using System.Collections;
//using System.ComponentModel;
//using System.Drawing;
//using System.Net;
//using System.Diagnostics;
//using System.Reflection;
//using System.Security;
//using System.Security.Permissions;
//using System.Windows.Forms;
//using System.Drawing.Printing;

//namespace ReflectTools.AutoPME {
//    //
//    // Delegate methods used by AutoTest.AutoTestEvents()
//    //
//    public class AutoTestEventDelegates {
//        public static void AddingNewEventHandler(object o, AddingNewEventArgs e)			    { AutoTest.EventRaised(o, e); }
//        public static void BindingCompleteEventHandler(object o, BindingCompleteEventArgs e)		    { AutoTest.EventRaised(o, e); }
//        public static void CacheVirtualItemsEventHandler(object o, CacheVirtualItemsEventArgs e)            { AutoTest.EventRaised(o, e); }
//        public static void CancelEventHandler(object o, CancelEventArgs e) { AutoTest.EventRaised(o, e); }
//        public static void ColumnClickEventHandler(object o, ColumnClickEventArgs e)                        { AutoTest.EventRaised(o, e); }
//        public static void ColumnReorderedEventHandler(object o, ColumnReorderedEventArgs e)  { AutoTest.EventRaised(o, e); }
//        public static void ContentsResizedEventHandler(object o, ContentsResizedEventArgs e)                { AutoTest.EventRaised(o, e); }
//        public static void ControlEventHandler(object o, ControlEventArgs e)                                { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewAutoSizeModeEventHandler(object o, DataGridViewAutoSizeModeEventArgs e)                          { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewCellCancelEventHandler(object o, DataGridViewCellCancelEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewCellEventHandler(object o, DataGridViewCellEventArgs e)                                          { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewCellMouseEventHandler(object o, DataGridViewCellMouseEventArgs e)                                { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewCellPaintingEventHandler(object o, DataGridViewCellPaintingEventArgs e)                          { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewCellStateChangedEventHandler(object o, DataGridViewCellStateChangedEventArgs e)                  { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewColumnEventHandler(object o, DataGridViewColumnEventArgs e)                                      { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewColumnStateChangedEventHandler(object o, DataGridViewColumnStateChangedEventArgs e)              { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewDataErrorEventHandler(object o, DataGridViewDataErrorEventArgs e)                                { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewRowEventHandler(object o, DataGridViewRowEventArgs e)                                            { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewEditingControlShowingEventHandler(object o, DataGridViewEditingControlShowingEventArgs e)        { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewRowPostPaintEventHandler(object o, DataGridViewRowPostPaintEventArgs e)                          { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewRowPrePaintEventHandler(object o, DataGridViewRowPrePaintEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewRowsAddedEventHandler(object o, DataGridViewRowsAddedEventArgs e)                                { AutoTest.EventRaised(o, e); }
//        public static void DataGridViewRowsDeletedEventHandler(object o, DataGridViewRowsDeletedEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void DateRangeEventHandler(object o, DateRangeEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void DownloadDataCompletedEventHandler(object o, DownloadDataCompletedEventArgs e)    { AutoTest.EventRaised(o, e); }
//        public static void DoWorkEventHandler(object o, DoWorkEventArgs e)				    { AutoTest.EventRaised(o, e); }
//        public static void DragEventHandler(object o, DragEventArgs e)                                      { AutoTest.EventRaised(o, e); }
//        public static void DrawItemEventHandler(object o, DrawItemEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void DrawListViewItemEventHandler(object o, DrawListViewItemEventArgs e)              { AutoTest.EventRaised(o, e); }
//        public static void DrawListViewSubItemEventHandler(object o, DrawListViewSubItemEventArgs e)        { AutoTest.EventRaised(o, e); }
//        public static void DrawTreeNodeEventHandler(object o, DrawTreeNodeEventArgs e)                      { AutoTest.EventRaised(o, e); }
//        public static void EventHandler(object o, EventArgs e)                                              { AutoTest.EventRaised(o, e); }
//        public static void FormClosedEventHandler(object o, FormClosedEventArgs e)			    { AutoTest.EventRaised(o, e); }
//        public static void GiveFeedbackEventHandler(object o, GiveFeedbackEventArgs e)                      { AutoTest.EventRaised(o, e); }
//        public static void HelpEventHandler(object o, HelpEventArgs e)                                      { AutoTest.EventRaised(o, e); }
//        public static void InputLanguageChangedEventHandler(object o, InputLanguageChangedEventArgs e)      { AutoTest.EventRaised(o, e); }
//        public static void InputLanguageChangingEventHandler(object o, InputLanguageChangingEventArgs e)    { AutoTest.EventRaised(o, e); }
//        public static void InvalidateEventHandler(object o, InvalidateEventArgs e)                          { AutoTest.EventRaised(o, e); }
//        public static void ItemCheckEventHandler(object o, ItemCheckEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void ItemCheckedEventHandler(object o, ItemCheckedEventArgs e)                        { AutoTest.EventRaised(o, e); }
//        public static void ItemDragEventHandler(object o, ItemDragEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void KeyEventHandler(object o, KeyEventArgs e)                                        { AutoTest.EventRaised(o, e); }
//        public static void KeyPressEventHandler(object o, KeyPressEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void LabelEditEventHandler(object o, LabelEditEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void LayoutEventHandler(object o, LayoutEventArgs e)                                  { AutoTest.EventRaised(o, e); }
//        public static void LinkClickedEventHandler(object o, LinkClickedEventArgs e)                        { AutoTest.EventRaised(o, e); }
//        public static void LinkLabelLinkClickedEventHandler(object o, LinkLabelLinkClickedEventArgs e)      { AutoTest.EventRaised(o, e); }
//        public static void ListChangedEventHandler(object o, ListChangedEventArgs e)						{ AutoTest.EventRaised(o, e); }
//        public static void ListControlConvertEventHandler(object o, ListControlConvertEventArgs e)			{ AutoTest.EventRaised(o, e); }   
//        public static void MaskInputRejectedEventHandler(object o, MaskInputRejectedEventArgs e)            { AutoTest.EventRaised(o, e); }
//        public static void MeasureItemEventHandler(object o, MeasureItemEventArgs e)                        { AutoTest.EventRaised(o, e); }
//        public static void MouseEventHandler(object o, MouseEventArgs e)                                    { AutoTest.EventRaised(o, e); }
//        public static void NavigateEventHandler(object o, NavigateEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void NodeLabelEditEventHandler(object o, NodeLabelEditEventArgs e)                    { AutoTest.EventRaised(o, e); }
//        public static void PaintEventHandler(object o, PaintEventArgs e)                                    { AutoTest.EventRaised(o, e); }
//        public static void PrintEventHandler(object o, PrintEventArgs e)                                    { AutoTest.EventRaised(o, e); }
//        public static void PrintPageEventHandler(object o, PrintPageEventArgs e)                            { AutoTest.EventRaised(o, e); }
//        public static void ProgressChangedEventHandler(object o, ProgressChangedEventArgs e)		{ AutoTest.EventRaised(o, e); }
//        public static void PropertyTabChangedEventHandler(object o, PropertyTabChangedEventArgs e)		{ AutoTest.EventRaised(o, e); }
//        public static void PropertyValueChangedEventHandler(object o, PropertyValueChangedEventArgs e)		{ AutoTest.EventRaised(o, e); }
//        public static void QueryAccessibilityHelpEventHandler(object o, QueryAccessibilityHelpEventArgs e)  { AutoTest.EventRaised(o, e); }
//        public static void QueryContinueDragEventHandler(object o, QueryContinueDragEventArgs e)            { AutoTest.EventRaised(o, e); }
//        public static void QueryPageSettingsEventHandler(object o, QueryPageSettingsEventArgs e)            { AutoTest.EventRaised(o, e); }
//        public static void QuestionEventHandler(object o, QuestionEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void RetrieveVirtualItemEventHandler(object o, RetrieveVirtualItemEventArgs e)        { AutoTest.EventRaised(o, e); }
//        public static void ScrollEventHandler(object o, ScrollEventArgs e)                                  { AutoTest.EventRaised(o, e); }
//        public static void SearchForVirtualItemEventHandler(object o, SearchForVirtualItemEventArgs e)      { AutoTest.EventRaised(o, e); }
//        public static void SplitterCancelEventHandler(object o, SplitterCancelEventArgs e)					{ AutoTest.EventRaised(o, e); }
//        public static void SplitterEventHandler(object o, SplitterEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void StatusBarPanelClickEventHandler(object o, StatusBarPanelClickEventArgs e)        { AutoTest.EventRaised(o, e); }
//        //	public static void TaskCancelledEventHandler(object o, TaskCancelledEventArgs e)		    { AutoTest.EventRaised(o, e); }
//        //	public static void TaskCompletedEventHandler(object o, TaskCompletedEventArgs e)		    { AutoTest.EventRaised(o, e); }
//        //	public static void TaskFailedEventHandler(object o, TaskFailedEventArgs e)		    { AutoTest.EventRaised(o, e); }
//        public static void TableLayoutCellPaintEventHandler(object o, TableLayoutCellPaintEventArgs e)          { AutoTest.EventRaised(o, e); }
//        public static void TreeNodeMouseClickEventHandler(object o, TreeNodeMouseClickEventArgs e)          { AutoTest.EventRaised(o, e); }
//        public static void TreeViewCancelEventHandler(object o, TreeViewCancelEventArgs e)                  { AutoTest.EventRaised(o, e); }
//        public static void TreeViewEventHandler(object o, TreeViewEventArgs e)                              { AutoTest.EventRaised(o, e); }
//        public static void TypeValidationEventHandler(object o, TypeValidationEventArgs e)					{ AutoTest.EventRaised(o, e); }
//        public static void UICuesEventHandler(object o, UICuesEventArgs e)                                  { AutoTest.EventRaised(o, e); }
//        public static void HtmlElementEventHandler(object o, HtmlElementEventArgs e)						{ AutoTest.EventRaised(o, e); }
//    //	public static void UserPreferenceChangingEventHandler(object o,UserPreferenceChangingEventArgs e)    {AutoTest.EventRaised(o, e); }
//        // Gone in 31229.00
////		public static void VirtualItemStateChangedEventHandler(object o, VirtualItemStateChangedEventArgs e) { AutoTest.EventRaised(o, e); }
//        public static void ToolStripItemClickedEventHandler(object o, ToolStripItemClickedEventArgs e)		{ AutoTest.EventRaised(o, e); }
//    }
//}
