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

namespace Avalon.Test.CoreUI.UtilityHelper
{
  /// <summary>
  /// Property Engine Application will be used as the base class
  /// for various Property Engine Tests. 
  /// For example: WithStoryboards.cs uses PEApplication to test basic 
  /// Storyboard tests.
  /// </summary>
  public class PEApplication : Application
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
        _mainWindow.Title = _testTitle;
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
        return _tickCountMaximunAllowed;
      }
      set
      {
        if (value <= 4)
        {
          _tickCountMaximunAllowed = 4;
        }
        else
        {
          _tickCountMaximunAllowed = value;
        }
      }
    }

    /// <summary>
    /// Provide a way for derived class to know the current tick count
    /// </summary>
    /// <value>tick count. One second for one tick</value>
    protected int TestTickCountInSceonds
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
      Utilities.PrintStatus("[Tick " + TestTickCountInSceonds.ToString() + "] " + message);
    }

    /// <summary>
    /// Derived class should NOT override this class.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnStartup(StartupEventArgs e)
    {
      SetupUI();
      base.OnStartup(e);
    }

    private void SetupUI()
    {
      //We only use one window
      _mainWindow = new Window();
      _mainWindow.Title = _testTitle;
      //mainWindow has one Panel
      _mainPanel = new StackPanel();

      //The children for mainPanel should be set up by each test application
      ((System.ComponentModel.ISupportInitialize)_mainPanel).BeginInit();
      TestSetupUI(_mainPanel, _mainWindow);
      SetupTimerTickEverySecond();
      ((System.ComponentModel.ISupportInitialize)_mainPanel).EndInit();

      _mainWindow.Content = _mainPanel;

      //Fix window position for better testing experience
      _mainWindow.Left = 300;
      _mainWindow.Top = 300;

      _mainWindow.Show();
    }

    private void SetupTimerTickEverySecond()
    {
      System.Threading.Thread.Sleep(10);
      _timer = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Normal, System.Windows.Threading.Dispatcher.CurrentDispatcher);
      _timer.Interval = TimeSpan.FromSeconds(1);
      _timer.Tick += new EventHandler(OnTimerTick);
      _timer.Start();
    }

    private void OnTimerTick(object sender, EventArgs e)
    {
      _tickCount++;
      //Check Time-Out first
      if (_tickCount > _tickCountMaximunAllowed)
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
        else  //Second Chance Check
        {
          if (_exitMessage.Length > 0 && _timerSecondChance == null && _supportSecondChanceValidation)
          {
            _timerSecondChance = new System.Windows.Threading.DispatcherTimer();
            _timerSecondChance.Interval = TimeSpan.FromMilliseconds(200); //0.2 second
            _timerSecondChance.Tick += new EventHandler(timerSecondChance_Tick);
            _timerSecondChance.Start();
          }
        }
      }
    }

    private void timerSecondChance_Tick(object sender, EventArgs e)
    {
      Utilities.PrintStatus("***This is the second chance check***");
      _exitMessage += "***second chance check***";
      string saveExitMessage = _exitMessage;
      //Do not increment TickCount, derived class has no idea that it is called again
      if (TestCompleted())
      {
        ShutMeDown();
      }
      //Determine if second-chance validation suceeds
      if (_exitMessage.Length > saveExitMessage.Length)  //We failed again
      {
        Utilities.PrintStatus("***second chance FAILED again***");
        _exitMessage += "***second chance FAILED again***";
      }
      else
      {
        Utilities.PrintStatus("***second chance PASSED***");
        //We will make test success so far
        _exitMessage = "";
      }

      //No matter what, stop second chance timer
      _timerSecondChance.Stop();
    }


    private void ShutMeDown()
    {
      if (TestExitMessage.Length == 0)
      {
        Shutdown(0);
      }
      else
      {
        if (!TestExitMessage.Contains(_testTitle))
        {
          TestExitMessage += " [" + _testTitle + "]";
          Shutdown(1);
        }
      }
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


    protected virtual void TestSetupUI(StackPanel parentPanel, Window rootWindow)
    {
      if (parentPanel.Children.Count == 0)
      {
        //Test Application Needs to Override This method!
        TextBlock warningText = new TextBlock();

        warningText.Foreground = System.Windows.Media.Brushes.Red;
        warningText.FontSize = 20;
        warningText.Text = "Derived Application Class Must Override TestSetupUI!";
        parentPanel.Children.Add(warningText);
      }
    }

    private string _testTitle = "Derived Application Should Provide Better Test Title";

    private Window _mainWindow;

    private StackPanel _mainPanel;

    //Main timer that raises Tick event every second
    private System.Windows.Threading.DispatcherTimer _timer;

    //Secondary timer that gives aanother chance to check
    //Currently only one-time event
    private System.Windows.Threading.DispatcherTimer _timerSecondChance;

    private int _tickCount = 0; //Number of Ticks

    private int _tickCountMaximunAllowed = 4; //Maximun Tick Count Allowed. One second over it and the test fail due to time out

    private string _exitMessage = "";

    private bool _supportSecondChanceValidation = true; //One line switch to revert back to old behaviour (change to false)

  }
}
