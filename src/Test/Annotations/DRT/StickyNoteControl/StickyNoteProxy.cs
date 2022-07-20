// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Globalization;
using System.Windows.Controls;
using System.Windows;

namespace DRT
{
    public class StickyNoteControlProxy
    {
        public StickyNoteControlProxy() : this(false)
        {
        }

        public StickyNoteControlProxy(StickyNoteControl snc)
        {
            _stickynoteInstance = snc;
        }

        public StickyNoteControlProxy(bool isInk)
        {
            if ( isInk )
            {
                _stickynoteInstance = (StickyNoteControl)Activator.CreateInstance(StickyNoteControlType, BindingFlags.Instance | BindingFlags.NonPublic, null,
                    new object[] { InkField.GetValue(null) }, CultureInfo.InvariantCulture);
            }
            else
            {
                _stickynoteInstance = (StickyNoteControl)Activator.CreateInstance(StickyNoteControlType, BindingFlags.Instance | BindingFlags.NonPublic, null, null, CultureInfo.InvariantCulture);
            }
        }

        public StickyNoteControl StickyNoteControl
        {
            get
            {
                return _stickynoteInstance;
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _stickynoteInstance.IsExpanded;
            }
            set
            {
                _stickynoteInstance.IsExpanded = value;
            }
        }

        public string Author
        {
            get
            {
                return (string) _stickynoteInstance.GetValue(StickyNoteControl.AuthorProperty);
            }
            set
            {
                DependencyPropertyKey key = (DependencyPropertyKey)AuthorProperty.GetValue(_stickynoteInstance);
                _stickynoteInstance.SetValue(key, value);
            }
        }


        private static Assembly PresentationFrameworkAssembly
        {
            get
            {
                if ( s_assemblyPresentationFramework == null )
                {
                    s_assemblyPresentationFramework = typeof(System.Windows.FrameworkElement).Assembly;   // PresentationFramework.dll
                }

                return s_assemblyPresentationFramework;
            }
        }

        private static Type StickyNoteControlType
        {
            get
            {
                if ( s_typeStickyNoteControl == null )
                {
                    s_typeStickyNoteControl = PresentationFrameworkAssembly.GetType(StickyNoteControlTypeName);
                }

                return s_typeStickyNoteControl;
            }
        }

        private static Type StickyNoteType
        {
            get
            {
                if ( s_typeStickyNoteType == null )
                {
                    s_typeStickyNoteType = PresentationFrameworkAssembly.GetType(StickyNoteTypeTypeName);
                }

                return s_typeStickyNoteType;
            }
        }

        private static FieldInfo InkField
        {
            get
            {
                if ( s_fieldInfoInk == null )
                {
                    s_fieldInfoInk = StickyNoteType.GetField(InkFieldName);
                }

                return s_fieldInfoInk;
            }
        }

        private static FieldInfo AuthorProperty
        {
            get
            {
                if ( s_propertyInfoAuthor == null )
                {
                    s_propertyInfoAuthor = StickyNoteControlType.GetField(AuthorPropertyKey, BindingFlags.Static | BindingFlags.NonPublic);
                }
                return s_propertyInfoAuthor;
            }
        }

        private StickyNoteControl _stickynoteInstance;

        // Types
        private const string StickyNoteControlTypeName = "System.Windows.Controls.StickyNoteControl";
        private static Type s_typeStickyNoteControl;
        private const string StickyNoteTypeTypeName = "System.Windows.Controls.StickyNoteType";
        private static Type s_typeStickyNoteType;

        // Fields
        private const string AuthorPropertyKey = "AuthorPropertyKey";
        private static FieldInfo s_propertyInfoAuthor;
        private const string InkFieldName = "Ink";
        private static FieldInfo s_fieldInfoInk;

        private static Assembly s_assemblyPresentationFramework;

    }

}
