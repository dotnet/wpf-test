// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControlLibrary
{

  public class FunnyControl : Control
  {
    static FunnyControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FunnyControl), new FrameworkPropertyMetadata(typeof(FunnyControl)));
    }

    public FunnyControl()
    {
      InitializeCommands();
    }

    private static void InitializeCommands()
    {
      


      CommandManager.RegisterClassCommandBinding(typeof(FunnyControl), new CommandBinding(OpenDetailPageCommand,
        delegate(object sender, ExecutedRoutedEventArgs e)
        {
          FunnyControl control = sender as FunnyControl;
          if (control != null)
          {
            control.OnOpenDetailPageCommand();
          }

        },
        delegate(object sender, CanExecuteRoutedEventArgs e)
        {
          e.CanExecute = true;
          e.Handled = true;
          
        }
      ));


    }


    public static readonly RoutedCommand OpenDetailPageCommand = new RoutedCommand("OpenDetailPageCommand", typeof(FunnyControl));


    private void OnOpenDetailPageCommand()
    {
      DetailWindow dw = new DetailWindow();
      dw.ShowDialog();
    }

  }
}
