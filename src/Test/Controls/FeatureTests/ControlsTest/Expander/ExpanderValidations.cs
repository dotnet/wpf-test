//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// All classes in this file are some helper ones, which used for validate testing result of Expander
//---------------------------------------------------------------------------


#region Using directives

using System;
using System.Windows;
using System.Windows.Controls;
using Avalon.Test.ComponentModel.Utilities;
using Avalon.Test.ComponentModel.UnitTests;
using System.Windows.Media;
using Microsoft.Test.Logging;

#endregion



namespace Avalon.Test.ComponentModel.Validations
{
    /// <summary>
    /// Calss to validate Disabled Behavior
    /// </summary>
    class ExpanderDisabledBehaviorValidation : IValidation
    {

        #region IValidate Members

        /// <summary>
        /// validate Disabled Behavior
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            Expander expander = validationParams[0] as Expander;
            string[] args = (string[])validationParams[1];
            return expander.IsExpanded == bool.Parse(args[1]) && !expander.IsKeyboardFocused && !expander.IsKeyboardFocusWithin;
        }
        #endregion
    }



    /// <summary>
    /// A class to compare DependencyProperties of two Control
    /// </summary>
    class CompareDPValidation : IValidation
    {
        #region IValidate Members

        /// <summary>
        /// validate Expander's DependencyProperty.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        /// validationParams should be a two-dimentional array, the first element is the searching root in which to search control by its ID
        /// the second element is a array. For this validation is designed to comapre two DPs of two Controls, the first elements in this
        /// array are the IDs of the two controls. the later two elements are the names of the DPs.
        public bool Validate(params object[] validationParams)
        {
            string proName;
            string comName;
            object[] args = (object[])validationParams[1];
            Object element1 = VisualTreeUtils.FindPartByName((Visual)validationParams[0],(string)args[0]);
            Object element2 = VisualTreeUtils.FindPartByName((Visual)validationParams[0],(string)args[1]);
            //Object element1 = ControlTestHelper.GetControlByNameFromVisualTree((string)args[0], (Visual)validationParams[0]);
            //Object element2 = ControlTestHelper.GetControlByNameFromVisualTree((string)args[1], (Visual)validationParams[0]);
            int length = (args.Length - 2) / 2;
            for (int i = 0; i < length; i++)
            {
                proName = (string)args[2 * i + 2];
                comName = (string)args[2 * i + 3];
                if (!ObjectFactory.GetObjectProperty(element2, comName).Equals(ObjectFactory.GetObjectProperty(element1, proName)))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion
    }



    /// <summary>
    /// A class to compare status of two control
    /// </summary>
    class CompareValidation: IValidation 
    {
        #region IValidate Members

        /// <summary>
        /// validate Expander's status.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        /// validationParams should be a two-dimentional array, the first element is the searching root in which to search control by its ID
        /// the second element is a array. For this validation is designed to comapre two propertyies of two Controls, the first elements in this
        /// array are the IDs of the two controls. the later two elements are the names of the properties.
        public bool Validate(params object[] validationParams)
        {
            string proName;
            string comName;
            object[] args = (object[])validationParams[1];
            Object element1 = LogicalTreeHelper.FindLogicalNode((FrameworkElement)validationParams[0],(string)args[0]);
            Object element2 = LogicalTreeHelper.FindLogicalNode((FrameworkElement)validationParams[0], (string)args[1]);
            int length = (args.Length - 2)/2;
            if (length <= 0)
            {
                return false;
            }
            for (int i = 0; i <length; i++)
            {
                proName = (string)args[2*i+2];
                comName = (string)args[2*i+3];
                if (!ObjectFactory.GetObjectProperty(element2, comName).Equals(ObjectFactory.GetObjectProperty(element1, proName)))
                {
                    GlobalLog.LogStatus(" the value of " + proName + " does not equal to  " + comName);
                    return false;
                }
            }
            return true;
        }
        #endregion
    }



    /// <summary>
    /// A class to validate the Expander's ExpandDirection with standard criteria.
    /// </summary>
    class ExpanderDirectionValidation : IValidation 
    {
        #region IValidate Members

        /// <summary>
        /// validate Expander's ExpanderDirection.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            Expander expander = validationParams[0] as Expander;
            object[] args = (object[])validationParams[1];
            ExpandDirection direction = (ExpandDirection)Convert.ToInt32(args[0]);
            return PMEUtils.Current.VerifyProperty(expander, "ExpandDirection", direction);
        }

        #endregion
    }



    /// <summary>
    /// Class to check control type.
    /// </summary>
    class ControlTypeValidation : IValidation
    {
        /// <summary>
        /// test control type name 
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            Control control = validationParams[0] as Control;
            object[] args = (object[])validationParams[1];
            return control.DependencyObjectType.Name.Equals(args[0]);
        }
    }



    /// <summary>
    /// A class to validate IsExpanded and ExpandDirection.
    /// </summary>
    class ExpanderValidation : IValidation
    {
        #region IValidate Members

        
        /// <summary>
        /// validate Expander's IsExpanded and ExpandDirection.
        /// </summary>
        /// <param name="validationParams"></param>
        /// <returns></returns>
        public bool Validate(params object[] validationParams)
        {
            Expander expander = validationParams[0] as Expander;
            string[] args = (string[])validationParams[1];
            return expander.IsExpanded == bool.Parse((string)args[0]) && expander.ExpandDirection == (ExpandDirection)int.Parse(args[1]);
        }

        #endregion
    }
}

