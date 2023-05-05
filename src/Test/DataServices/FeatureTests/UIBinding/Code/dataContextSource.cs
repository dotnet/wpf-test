// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading; using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests binding to Relative Sources: /ParentData and /PreviousData.
	/// </description>
	/// </summary>
    [Test(3, "Binding", "DataContextSourceTest")]
    public class DataContextSourceTest : XamlTest
    {
        ObjectDataProvider _dso;

        public DataContextSourceTest() : base(@"DataContextSourceTest.xaml")
        {
			InitializeSteps += new TestStep(init);
			RunSteps += new TestStep(ValidateBindings);
		}

		private TestResult init()
		{
			Status("init");
			Page page = RootElement as Page;
			if (page == null)
			{
				LogComment("Fail - Page is null");
				return TestResult.Fail;
			}
			Border border = page.Content as Border;
			_dso = border.FindResource("DSO") as ObjectDataProvider;
			if (_dso == null)
			{
				LogComment("Fail - DataSource is null");
				return TestResult.Fail;
			}
			WaitForPriority(DispatcherPriority.Render);

			return TestResult.Pass;
		}

		private TestResult ValidateBindings()
		{
			Status("ValidateBindings");
			WaitForPriority(DispatcherPriority.Render);

			ManagementChain managers = _dso.Data as ManagementChain;

			foreach (ManagementChain.Manager manager in managers)
			{
				FrameworkElement[] employeeVisuals = Util.FindDataVisuals(RootElement, manager.Employees);

				if (employeeVisuals.Length != manager.Employees.Count)
				{
					LogComment("Visual Tree does not contain the expected number of elements");
					LogComment("Expected: " + manager.Employees.Count);
					LogComment("Actual: " + employeeVisuals.Length);
					return TestResult.Fail;
				}

				for (int i = manager.Employees.Count - 1; i >= 0; --i)
				{
					string expPrevious = "Fallback";

					if (i > 0)
						expPrevious = ((ManagementChain.Employee)manager.Employees[i - 1]).Name;

					TextBlock previousText = ((DockPanel)employeeVisuals[i]).Children[2] as TextBlock;

					if (previousText.Text != expPrevious)
					{
						LogComment("Binding to Previous failed");
						LogComment("Expected: " + expPrevious);
						LogComment("Actual: " + previousText.Text);
						return TestResult.Fail;
					}

					TextBlock parentText = ((DockPanel)employeeVisuals[i]).Children[4] as TextBlock;

					if (parentText.Text != manager.Name)
					{
						LogComment("Binding to Parent failed");
						LogComment("Expected: " + manager.Name);
						LogComment("Actual: " + parentText.Text);
						return TestResult.Fail;
					}
				}
			}

			return TestResult.Pass;
		}
	}

	#region ManagementData
    public class ManagementChain : ObservableCollection<Microsoft.Test.DataServices.ManagementChain.Manager>
    {
		public ManagementChain()
		{
			Manager _manager;

			Add(_manager = new Manager("Lane"));
			_manager.Employees.Add(new Employee("Maria"));
			_manager.Employees.Add(new Employee("Mike"));
			_manager.Employees.Add(new Employee("Corom"));
			Add(_manager = new Manager("Fredric"));
			_manager.Employees.Add(new Employee("Martha"));
			_manager.Employees.Add(new Employee("John"));
			_manager.Employees.Add(new Employee("William"));
		}

		public class Manager : Employee
		{
			public Manager(string name) : base(name)
			{
				_employees = new EmployeeList();
			}

			EmployeeList _employees;

			public EmployeeList Employees { get { return _employees; } }
		}

        public class EmployeeList : ObservableCollection<Employee>
        {
			public EmployeeList() : base()
			{
			}
		}

		public class Employee
		{
			public Employee(string name)
			{
				_name = name;
			}

			string _name;

			public string Name { get { return _name; } }
		}
	}


	#endregion

}
