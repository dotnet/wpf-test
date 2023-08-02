using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Concrete class of ICommand for control property test.
    /// </summary>
    public class SubICommand : ICommand
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter)
        {
            CanExecuteChanged(this, null);
        }
    }
}
