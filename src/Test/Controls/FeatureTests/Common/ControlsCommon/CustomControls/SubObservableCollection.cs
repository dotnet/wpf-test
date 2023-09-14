using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Sub-class of ObservableCollection<T> for control property test.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SubObservableCollection<T> : ObservableCollection<T>
    {
        private bool isTest;
        public bool IsTest
        {
            set { isTest = value; }
            get { return isTest; }
        }
    }
}
