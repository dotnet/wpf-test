// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests binding to a Nullable&lt;int&gt;.
	/// </description>
	/// </summary>
    [Test(3, "Binding", "NullT")]
    public class NullT : XamlTest
    {
        ObjectDataProvider _dso;
        TextBlock _testText;
        NullTItem _item;

        public NullT() : base(@"NullT.xaml")
        {
            InitializeSteps += new TestStep(init);
			RunSteps += new TestStep(ValidateInitial);
			RunSteps += new TestStep(UpdateValues);
			RunSteps += new TestStep(ValidateUpdate);
		}

        private TestResult init()
        {
            _dso = RootElement.Resources["DSO"] as ObjectDataProvider;
            _testText = (TextBlock)Util.FindElement(RootElement, "testText");
            if (_dso == null)
            {
                LogComment("Could not reference the DataSource");
                return TestResult.Fail;
            }
            if (_testText == null)
            {
                LogComment("Could not reference the Text");
                return TestResult.Fail;
            }

            _item = _dso.Data as NullTItem;
            if (_item == null)
            {
                LogComment("Could not reference the Item");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

		private TestResult ValidateInitial()
		{
			Status("Validating initial values");
			// it doesn't matter what we pass for expTheInt, if expHasValue is false, .Value 
			// will not be validated (we get an exception)
			return Validate("initial values", false, -1, "");
		}

		private TestResult UpdateValues()
		{
			Status("Updating values");
			_item.TheInt = 5;
			WaitForPriority(DispatcherPriority.Background);
			LogComment("Updated Values");

			return TestResult.Pass;
		}

		private TestResult ValidateUpdate()
		{
			Status("Validating updated values");
			return Validate("updated values", true, 5, "5");
		}

		private TestResult Validate(string stepName, bool expHasValue, int expTheInt, string expText)
		{
			Status("Validate");
			TestResult result = TestResult.Pass;
			if (_item.TheInt.HasValue != expHasValue)
			{
				LogComment("_item.TheInt.HasValue was: \'" + _item.TheInt.HasValue + "\' expected: \'" + expHasValue + "\'");
				result = TestResult.Fail;
			}

			if (_item.TheInt.HasValue)
			{
				if (_item.TheInt.Value != expTheInt)
				{
					LogComment("_item.TheInt was: \'" + _item.TheInt + "\' expected: \'" + expTheInt + "\'");
					result = TestResult.Fail;
				}
			}

			if (_testText.Text != expText)
			{
				LogComment("_testText.Text was: \'" + _testText.Text + "\' expected: \'" + expText + "\'");
				result = TestResult.Fail;
			}

			if (result == TestResult.Pass)
				LogComment("All values were the expected value for " + stepName);
			else
				LogComment("Some values were not the expected value for " + stepName);

			return result;
		}
	}

    public class NullTItem : INotifyPropertyChanged
    {
        private Nullable<int> _int;

        public Nullable<int> TheInt
		{
            get { return _int; }
            set
            {
                _int = value;
                RaisePropertyChangedEvent("TheInt");
            }
        }

        public NullTItem()
        {
            _int = null;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

}
