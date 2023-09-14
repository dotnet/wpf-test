// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.UtilityHelper
{
  /// <summary>
  /// Setup the environment required for HwndSource'd tests
  /// </summary>
  public class HwndApplication
  {
    /// <summary>
    /// Exit message from this test
    /// </summary>
    /// <value>The default message is '', any other value documents Error condition</value>
    public string TestExitMessage
    {
      get
      {
        return _exitMessage;
      }
      protected set
      {
        _exitMessage = value;
      }
    }

    /// <summary>
    /// Derived class should set appropriate Test Title
    /// </summary>
    /// <value>Test Title String</value>
    protected string TestTitle
    {
      get
      {
        return _testTitle;
      }
      set
      {
        _testTitle = value;
// Now we are dealing with a Visual, which is not guaranteed to have a title
//        mainWindow.Text = _testTitle.
      }
    }

    /// <summary>
    /// The maximum time, in seconds, this test can take
    /// </summary>
    /// <value>timeout in seconds</value>
    protected int TestTimeOutInSeconds
    {
      get
      {
        return _tickCountMaximumAllowed;
      }
      set
      {
        if (value <= 4)
        {
          _tickCountMaximumAllowed = 4;
        }
        else
        {
          _tickCountMaximumAllowed = value;
        }
      }
    }

    /// <summary>
    /// Provide a way for derived class to know the current tick count
    /// </summary>
    /// <value>tick count. One second for one tick</value>
    protected int TestTickCountInSeconds
    {
      get
      {
        return _tickCount;
      }
    }

    /// <summary>
    /// This is a helper method to ptint out message with tick info prefixed
    /// </summary>
    /// <param name="message">Message to display to the test console</param>
    protected void TestPrintStatusWithTick(string message)
    {
      Utilities.PrintStatus("[Tick " + TestTickCountInSeconds.ToString() + "] " + message);
    }

    /// <summary>
    /// Start the application.
    /// </summary>
    public int Run()
    {
      SetupUI();
      return _testStatus;
    }

    /// <summary>
    /// Initialize the basic environment.
    /// </summary>
    private void SetupUI()
    {

      _sourceWindow = SourceHelper.CreateHwndSource( 800, 700,0,0);

      TestSetupUI(_sourceWindow);
      SetupTimerTickEverySecond();

      _dispatcher = Dispatcher.CurrentDispatcher;
      Dispatcher.Run();

    }

    /// <summary>
    /// Initialize the timinig element.
    /// </summary>
    private void SetupTimerTickEverySecond()
    {
      _timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal, Dispatcher.CurrentDispatcher);
      _timer.Interval = TimeSpan.FromSeconds(1);
      _timer.Tick += new EventHandler(OnTimerTick);
      _timer.Start();
    }

    /// <summary>
    /// Handle testing at each tick.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerTick(object sender, EventArgs e)
    {
      _tickCount++;
      if (_tickCount > _tickCountMaximumAllowed)
      {
        TestExitMessage += "Time Out!";
        ShutMeDown();
      }
      else  //We need to check if we can shutdown because all tests are completed
      {
        if (TestCompleted())
        {
          ShutMeDown();
        }
      }
    }

    /// <summary>
    /// Shutdown the application.
    /// </summary>
    private void ShutMeDown()
    {
      if (TestExitMessage.Length == 0)
      {
        _testStatus = 0;
      }
      else
      {
        _testStatus = 1;
        if (!TestExitMessage.Contains(_testTitle))
        {
          TestExitMessage += " [" + _testTitle + "]";
        }
      }

      _dispatcher.InvokeShutdown();
    }

    /// <summary>
    /// Derived application MUST override this method so that they
    /// can return true when no more tests are needed. Do NOT call base.
    /// </summary>
    /// <returns>True to shut down application. false to keep going</returns>
    protected virtual bool TestCompleted()
    {
      return false;
    }

    /// <summary>
    /// Derived class MUST override this method so that they can set up
    /// necessary UI. Do NOT call base.
    /// </summary>
    /// <param name="sourceWindow"></param>
    protected virtual void TestSetupUI(HwndSource sourceWindow)
    {
      StackPanel sp = new StackPanel();
      Button btn = new Button();
      btn.Content = "Please override HwndApplication.TestSetupUI";
      sp.Children.Add(btn);

      sourceWindow.RootVisual = sp;
   }

    private int    _testStatus;
    private string _testTitle = "Derived Application Should Provide Better Test Title";
    private string _exitMessage = "";
    
    private HwndSource     _sourceWindow;
    private Dispatcher _dispatcher;

    private DispatcherTimer _timer;
    private int     _tickCount = 0; //Number of Ticks
    private int     _tickCountMaximumAllowed = 4; //Maximum Tick Count Allowed. One second over it and the test fail due to time out
  }
}
