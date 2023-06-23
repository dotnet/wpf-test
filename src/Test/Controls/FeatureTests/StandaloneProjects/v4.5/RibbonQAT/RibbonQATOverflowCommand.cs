using System;
using System.Windows.Input;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Ribbon QAT overflow command
    /// </summary>
    public class RibbonQATOverflowCommand : ICommand
    {
        public RibbonQATOverflowCommand(RibbonQATOverflowViewModel vm)
        {
            this.vm = vm;
        }

        private RibbonQATOverflowViewModel vm;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
            }
            remove
            {
            }
        }

        public void Execute(object parameter)
        {
            vm.ExecuteCommandStatus = "Command fired";
        }
    }
}
