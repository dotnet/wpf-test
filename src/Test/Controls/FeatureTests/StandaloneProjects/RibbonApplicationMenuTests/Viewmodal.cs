using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Controls.Ribbon;

namespace RibbonApplicationMenuTests
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel(FrameworkElement element)
        {
            _element = element;
        }

        #region Public Properties

        public FontFamily FontFamily
        {
            get
            {
                return _fontFamily;
            }
            set
            {
                if (value != _fontFamily)
                {
                    _fontFamily = value;
                    OnPropertyChanged("FontFamily");
                }
            }
        }

        public double FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                if (value != _fontSize)
                {
                    _fontSize = value;
                    OnPropertyChanged("FontSize");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Commands

        public DelegateCommand AlignObjectsLeftCommand
        {
            get
            {
                if (_alignObjectsLeftCommand == null)
                {
                    _alignObjectsLeftCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _alignObjectsLeftCommand;
            }
        }

        public DelegateCommand AlignObjectsCenteredHorizontalCommand
        {
            get
            {
                if (_alignObjectsCenteredHorizontalCommand == null)
                {
                    _alignObjectsCenteredHorizontalCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _alignObjectsCenteredHorizontalCommand;
            }
        }

        public DelegateCommand AlignObjectsRightCommand
        {
            get
            {
                if (_alignObjectsRightCommand == null)
                {
                    _alignObjectsRightCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _alignObjectsRightCommand;
            }
        }

        public DelegateCommand AlignObjectsBottomCommand
        {
            get
            {
                if (_alignObjectsBottomCommand == null)
                {
                    _alignObjectsBottomCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _alignObjectsBottomCommand;
            }
        }

        public DelegateCommand BoldCommand
        {
            get
            {
                if (_boldCommand == null)
                {
                    _boldCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _boldCommand;
            }
        }

        public DelegateCommand BottomBorderCommand
        {
            get
            {
                if (_bottomBorderCommand == null)
                {
                    _bottomBorderCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _bottomBorderCommand;
            }
        }

        public DelegateCommand BulletCommand
        {
            get
            {
                if (_bulletCommand == null)
                {
                    _bulletCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _bulletCommand;
            }
        }

        public DelegateCommand ChangeCaseCommand
        {
            get
            {
                if (_changeCaseCommand == null)
                {
                    _changeCaseCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _changeCaseCommand;
            }
        }

        public DelegateCommand CharacterScalingCommand
        {
            get
            {
                if (_characterScalingCommand == null)
                {
                    _characterScalingCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _characterScalingCommand;
            }
        }

        public DelegateCommand ClearFormattingCommand
        {
            get
            {
                if (_clearFormattingCommand == null)
                {
                    _clearFormattingCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _clearFormattingCommand;
            }
        }

        public DelegateCommand CopyCommand
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _copyCommand;
            }
        }

        public DelegateCommand CutCommand
        {
            get
            {
                if (_cutCommand == null)
                {
                    _cutCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _cutCommand;
            }
        }

        public DelegateCommand DecreaseDecimalsCommand
        {
            get
            {
                if (_decreaseDecimalsCommand == null)
                {
                    _decreaseDecimalsCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _decreaseDecimalsCommand;
            }
        }

        public DelegateCommand DistributeParagraphCommand
        {
            get
            {
                if (_distributeParagraphCommand == null)
                {
                    _distributeParagraphCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _distributeParagraphCommand;
            }
        }

        public DelegateCommand DocumentMapCommand
        {
            get
            {
                if (_documentMapCommand == null)
                {
                    _documentMapCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _documentMapCommand;
            }
        }

        public DelegateCommand DraftViewCommand
        {
            get
            {
                if (_draftViewCommand == null)
                {
                    _draftViewCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _draftViewCommand;
            }
        }

        public DelegateCommand FindCommand
        {
            get
            {
                if (_findCommand == null)
                {
                    _findCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _findCommand;
            }
        }

        public DelegateCommand FontColorNoPreviewCommand
        {
            get
            {
                if (_fontColorNoPreviewCommand == null)
                {
                    _fontColorNoPreviewCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _fontColorNoPreviewCommand;
            }
        }

        public DelegateCommand FormatPainterCommand
        {
            get
            {
                if (_formatPainterCommand == null)
                {
                    _formatPainterCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _formatPainterCommand;
            }
        }

        public DelegateCommand FullScreenCommand
        {
            get
            {
                if (_fullScreenCommand == null)
                {
                    _fullScreenCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _fullScreenCommand;
            }
        }

        public DelegateCommand FunctionCommand
        {
            get
            {
                if (_functionCommand == null)
                {
                    _functionCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _functionCommand;
            }
        }

        public DelegateCommand GridlinesCommand
        {
            get
            {
                if (_gridlinesCommand == null)
                {
                    _gridlinesCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _gridlinesCommand;
            }
        }

        public DelegateCommand GrowFontCommand
        {
            get
            {
                if (_growFontCommand == null)
                {
                    _growFontCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _growFontCommand;
            }
        }

        public DelegateCommand HeaderAndFooterCommand
        {
            get
            {
                if (_headerAndFooterCommand == null)
                {
                    _headerAndFooterCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _headerAndFooterCommand;
            }
        }

        public DelegateCommand HeaderOrFooterCommand
        {
            get
            {
                if (_headerOrFooterCommand == null)
                {
                    _headerOrFooterCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _headerOrFooterCommand;
            }
        }

        public DelegateCommand HighlightCommand
        {
            get
            {
                if (_highlightCommand == null)
                {
                    _highlightCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _highlightCommand;
            }
        }

        public DelegateCommand ItalicCommand
        {
            get
            {
                if (_italicCommand == null)
                {
                    _italicCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _italicCommand;
            }
        }

        public DelegateCommand IncreaseDecimalsCommand
        {
            get
            {
                if (_increaseDecimalsCommand == null)
                {
                    _increaseDecimalsCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _increaseDecimalsCommand;
            }
        }

        public DelegateCommand LineSpacingCommand
        {
            get
            {
                if (_lineSpacingCommand == null)
                {
                    _lineSpacingCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _lineSpacingCommand;
            }
        }

        public DelegateCommand MessageBarCommand
        {
            get
            {
                if (_messageBarCommand == null)
                {
                    _messageBarCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _messageBarCommand;
            }
        }

        public DelegateCommand MultiLangDocLibCommand
        {
            get
            {
                if (_multiLangDocLibCommand == null)
                {
                    _multiLangDocLibCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _multiLangDocLibCommand;
            }
        }

        public DelegateCommand NumberingCommand
        {
            get
            {
                if (_numberingCommand == null)
                {
                    _numberingCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _numberingCommand;
            }
        }

        public DelegateCommand OutlineViewCommand
        {
            get
            {
                if (_outlineViewCommand == null)
                {
                    _outlineViewCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _outlineViewCommand;
            }
        }

        public DelegateCommand Paste
        {
            get
            {
                if (_paste == null)
                {
                    _paste = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _paste;
            }
        }

        public DelegateCommand PrintLayoutViewCommand
        {
            get
            {
                if (_printLayoutViewCommand == null)
                {
                    _printLayoutViewCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _printLayoutViewCommand;
            }
        }

        public DelegateCommand RulerCommand
        {
            get
            {
                if (_rulerCommand == null)
                {
                    _rulerCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _rulerCommand;
            }
        }

        public DelegateCommand SelectCommand
        {
            get
            {
                if (_selectCommand == null)
                {
                    _selectCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _selectCommand;
            }
        }

        public DelegateCommand ShadeMergeFieldsCommand
        {
            get
            {
                if (_shadeMergeFieldsCommand == null)
                {
                    _shadeMergeFieldsCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _shadeMergeFieldsCommand;
            }
        }

        public DelegateCommand ShowParagraphMarks
        {
            get
            {
                if (_showParagraphMarks == null)
                {
                    _showParagraphMarks = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _showParagraphMarks;
            }
        }

        public DelegateCommand ShrinkFontCommand
        {
            get
            {
                if (_shrinkFontCommand == null)
                {
                    _shrinkFontCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _shrinkFontCommand;
            }
        }

        public DelegateCommand SortUpCommand
        {
            get
            {
                if (_sortUpCommand == null)
                {
                    _sortUpCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _sortUpCommand;
            }
        }

        public DelegateCommand SubscriptCommand
        {
            get
            {
                if (_subscriptCommand == null)
                {
                    _subscriptCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _subscriptCommand;
            }
        }

        public DelegateCommand SuperscriptCommand
        {
            get
            {
                if (_superscriptCommand == null)
                {
                    _superscriptCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _superscriptCommand;
            }
        }

        public DelegateCommand StrikethroughCommand
        {
            get
            {
                if (_strikethroughCommand == null)
                {
                    _strikethroughCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _strikethroughCommand;
            }
        }

        public DelegateCommand StyleCommand
        {
            get
            {
                if (_styleCommand == null)
                {
                    _styleCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _styleCommand;
            }
        }

        public DelegateCommand ThumbnailsCommand
        {
            get
            {
                if (_thumbnailsCommand == null)
                {
                    _thumbnailsCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _thumbnailsCommand;
            }
        }

        public DelegateCommand UnderlineCommand
        {
            get
            {
                if (_underlineCommand == null)
                {
                    _underlineCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _underlineCommand;
            }
        }

        public DelegateCommand WebLayoutViewCommand
        {
            get
            {
                if (_webLayoutViewCommand == null)
                {
                    _webLayoutViewCommand = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _webLayoutViewCommand;
            }
        }

        public DelegateCommand New
        {
            get
            {
                if (_new == null)
                {
                    _new = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _new;
            }
        }

        public DelegateCommand Open
        {
            get
            {
                if (_open == null)
                {
                    _open = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _open;
            }
        }

        public DelegateCommand Save
        {
            get
            {
                if (_save == null)
                {
                    _save = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _save;
            }
        }

        public DelegateCommand SaveAs
        {
            get
            {
                if (_saveAs == null)
                {
                    _saveAs = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _saveAs;
            }
        }

        public DelegateCommand Print
        {
            get
            {
                if (_print == null)
                {
                    _print = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _print;
            }
        }

        public DelegateCommand Prepare
        {
            get
            {
                if (_prepare == null)
                {
                    _prepare = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _prepare;
            }
        }

        public DelegateCommand Send
        {
            get
            {
                if (_send == null)
                {
                    _send = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _send;
            }
        }

        public DelegateCommand Publish
        {
            get
            {
                if (_publish == null)
                {
                    _publish = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _publish;
            }
        }

        public DelegateCommand Workflows
        {
            get
            {
                if (_workflows == null)
                {
                    _workflows = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _workflows;
            }
        }

        public DelegateCommand Close
        {
            get
            {
                if (_close == null)
                {
                    _close = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _close;
            }
        }

        public DelegateCommand Exit
        {
            get
            {
                if (_exit == null)
                {
                    _exit = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _exit;
            }
        }

        public DelegateCommand Options
        {
            get
            {
                if (_options == null)
                {
                    _options = new DelegateCommand(DefaultExecute, DefaultCanExecute);
                }
                return _options;
            }
        }

        public DelegateCommand ApplicationButton
        {
            get
            {
                if (_applicationButton == null)
                {
                    _applicationButton = new DelegateCommand(ApplicationButtonExecute, DefaultCanExecute);
                }
                return _applicationButton;
            }
        }

        public DelegateCommand CustomizeQAT
        {
            get
            {
                if (_customizeQAT == null)
                {
                    _customizeQAT = new DelegateCommand(CustomizeQATExecute, DefaultCanExecute);
                }
                return _customizeQAT;
            }
        }

        public PreviewDelegateCommand<object> FontFamilyCommand
        {
            get
            {
                if (_fontFamilyCommand == null)
                {
                    _fontFamilyCommand = new PreviewDelegateCommand<object>(FontFamilyExecute, DefaultCanExecute, FontFamilyPreview, FontFamilyCancelPreview);
                }
                return _fontFamilyCommand;
            }
        }

        public PreviewDelegateCommand<object> FontSizeCommand
        {
            get
            {
                if (_fontSizeCommand == null)
                {
                    _fontSizeCommand = new PreviewDelegateCommand<object>(FontSizeExecute, DefaultCanExecute, FontSizePreview, FontSizeCancelPreview);
                }
                return _fontSizeCommand;
            }
        }

        #endregion

        #region Private Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void DefaultExecute()
        {
        }

        private void ApplicationButtonExecute()
        {
            ApplicationCommands.Close.Execute(null, _element);
        }

        private void CustomizeQATExecute()
        {
            MessageBox.Show("Add/Remove commands", "Customize QuickAccessToolBar");
        }

        private void FontFamilyExecute(object parameter)
        {
            RibbonComboBoxItem item = (RibbonComboBoxItem)parameter;
            _actualDocumentFont = null;
            FontFamily = (FontFamily)_fontFamilyconverter.ConvertFromInvariantString((string)item.Content);
        }

        private void FontFamilyPreview(object parameter)
        {
            RibbonComboBoxItem item = (RibbonComboBoxItem)parameter;
            if (_actualDocumentFont == null)
            {
                _actualDocumentFont = FontFamily;
            }
            FontFamily = (FontFamily)_fontFamilyconverter.ConvertFromInvariantString((string)item.Content);
        }

        private void FontFamilyCancelPreview()
        {
            if (_actualDocumentFont != null)
            {
                FontFamily = _actualDocumentFont;
                _actualDocumentFont = null;
            }
        }

        private void FontSizeExecute(object parameter)
        {
            RibbonComboBoxItem item = (RibbonComboBoxItem)parameter;
            _actualDocumentFontSize = double.NaN;
            FontSize = double.Parse((string)item.Content);
        }

        private void FontSizePreview(object parameter)
        {
            RibbonComboBoxItem item = (RibbonComboBoxItem)parameter;
            if (double.IsNaN(_actualDocumentFontSize))
            {
                _actualDocumentFontSize = FontSize;
            }
            FontSize = double.Parse((string)item.Content);
        }

        private void FontSizeCancelPreview()
        {
            if (!double.IsNaN(_actualDocumentFontSize))
            {
                FontSize = _actualDocumentFontSize;
                _actualDocumentFontSize = double.NaN;
            }
        }

        private bool DefaultCanExecute()
        {
            return true;
        }

        private bool DefaultCanExecute(object parameter)
        {
            return true;
        }

        #endregion

        #region Private Data

        FrameworkElement _element;
        FontFamilyConverter _fontFamilyconverter = new FontFamilyConverter();
        FontFamily _actualDocumentFont;
        FontFamily _fontFamily;
        double _actualDocumentFontSize = double.NaN;
        double _fontSize;
        DelegateCommand _alignObjectsLeftCommand;
        DelegateCommand _alignObjectsCenteredHorizontalCommand;
        DelegateCommand _alignObjectsRightCommand;
        DelegateCommand _alignObjectsBottomCommand;
        DelegateCommand _boldCommand;
        DelegateCommand _bottomBorderCommand;
        DelegateCommand _bulletCommand;
        DelegateCommand _changeCaseCommand;
        DelegateCommand _characterScalingCommand;
        DelegateCommand _clearFormattingCommand;
        DelegateCommand _copyCommand;
        DelegateCommand _cutCommand;
        DelegateCommand _decreaseDecimalsCommand;
        DelegateCommand _distributeParagraphCommand;
        DelegateCommand _documentMapCommand;
        DelegateCommand _draftViewCommand;
        DelegateCommand _findCommand;
        DelegateCommand _fontColorNoPreviewCommand;
        DelegateCommand _formatPainterCommand;
        DelegateCommand _fullScreenCommand;
        DelegateCommand _functionCommand;
        DelegateCommand _gridlinesCommand;
        DelegateCommand _growFontCommand;
        DelegateCommand _headerAndFooterCommand;
        DelegateCommand _headerOrFooterCommand;
        DelegateCommand _highlightCommand;
        DelegateCommand _italicCommand;
        DelegateCommand _increaseDecimalsCommand;
        DelegateCommand _lineSpacingCommand;
        DelegateCommand _messageBarCommand;
        DelegateCommand _multiLangDocLibCommand;
        DelegateCommand _numberingCommand;
        DelegateCommand _outlineViewCommand;
        DelegateCommand _paste;
        DelegateCommand _printLayoutViewCommand;
        DelegateCommand _rulerCommand;
        DelegateCommand _selectCommand;
        DelegateCommand _shadeMergeFieldsCommand;
        DelegateCommand _showParagraphMarks;
        DelegateCommand _shrinkFontCommand;
        DelegateCommand _sortUpCommand;
        DelegateCommand _subscriptCommand;
        DelegateCommand _superscriptCommand;
        DelegateCommand _strikethroughCommand;
        DelegateCommand _styleCommand;
        DelegateCommand _thumbnailsCommand;
        DelegateCommand _underlineCommand;
        DelegateCommand _webLayoutViewCommand;
        DelegateCommand _new;
        DelegateCommand _open;
        DelegateCommand _save;
        DelegateCommand _saveAs;
        DelegateCommand _print;
        DelegateCommand _prepare;
        DelegateCommand _send;
        DelegateCommand _publish;
        DelegateCommand _workflows;
        DelegateCommand _close;
        DelegateCommand _exit;
        DelegateCommand _options;
        DelegateCommand _applicationButton;
        DelegateCommand _customizeQAT;
        PreviewDelegateCommand<object> _fontFamilyCommand;
        PreviewDelegateCommand<object> _fontSizeCommand;

        #endregion
    }
}
