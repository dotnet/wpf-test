// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// provide various options to generate a visual tree
    /// </summary>
    public class VisualTreeOptions
    {
        /// <summary>
        /// A delegate to get a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public delegate T GetValue<T>();

        #region Fields

        private Thickness _extraMargin = new Thickness(-10, -10, -10, -10);
        private int _addCount = 0;
        private int _modifyPositionCount = 0;
        private int _modifyPropertyCount = 0;
        private int _removeCount = 0;
        private int _brushCount = 20;
        private int _penCount = 20;
        private GetValue<Visibility> _getIsVisible;
        private GetValue<bool> _getIsHitTestVisible;
        private GetValue<bool> _getIsEnabled;
        private bool _randomPanelTransform = false;

        /// <summary>
        /// apply modification for all elements interested
        /// </summary>
        public const int AllElements = int.MaxValue;

        #endregion 

        #region Properties

        /// <summary>
        /// extra margin to place children
        /// </summary>
        public Thickness ExtraMargin
        {
            get
            {
                return this._extraMargin;
            }
            set
            {
                this._extraMargin = value;
            }
        }

        /// <summary>
        /// number of child elements to generate
        /// </summary>
        public int AddCount
        {
            get
            {
                return this._addCount;
            }
            set
            {
                this._addCount = value;
            }
        }

        /// <summary>
        /// number of child elements to modify
        /// </summary>
        public int ModifyPositionCount
        {
            get
            {
                return this._modifyPositionCount;
            }
            set
            {
                this._modifyPositionCount = value;
            }
        }

        /// <summary>
        /// number of child elements to modify
        /// </summary>
        public int ModifyPropertiesCount
        {
            get
            {
                return this._modifyPropertyCount;
            }
            set
            {
                this._modifyPropertyCount = value;
            }
        }

        /// <summary>
        /// number of child elements to remove
        /// </summary>
        public int RemoveCount
        {
            get
            {
                return this._removeCount;
            }
            set
            {
                this._removeCount = value;
            }
        }

        /// <summary>
        /// number of random brushes to use
        /// </summary>
        public int BrushCount
        {
            get
            {
                return this._brushCount;
            }
            set
            {
                this._brushCount = value;
            }
        }

        /// <summary>
        /// number of random pens to use
        /// </summary>
        public int PenCount
        {
            get
            {
                return this._penCount;
            }
            set
            {
                this._penCount = value;
            }
        }

        /// <summary>
        ///  IsVisible flag
        /// </summary>
        public GetValue<Visibility> GetIsVisible
        {
            get
            {
                return this._getIsVisible;
            }
            set
            {
                this._getIsVisible = value;
            }
        }

        /// <summary>
        /// IsHitTestVisible flag
        /// </summary>
        public GetValue<bool> GetIsHitTestVisible
        {
            get
            {
                return this._getIsHitTestVisible;
            }
            set
            {
                this._getIsHitTestVisible = value;
            }
        }

        /// <summary>
        /// IsEnabledVisible flag
        /// </summary>
        public GetValue<bool> GetIsEnabled
        {
            get
            {
                return this._getIsEnabled;
            }
            set
            {
                this._getIsEnabled = value;
            }
        }

        /// <summary>
        /// the panel needs to be transformed or not
        /// </summary>
        public bool RandomPanelTransform
        {
            get
            {
                return this._randomPanelTransform;
            }
            set
            {
                this._randomPanelTransform = value;
            }
        }

        #endregion
    }
}
