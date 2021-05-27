// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Microsoft.Test.Integration
{
    /// <summary>
    /// Provides a helper class that implements the ITestContract interface.
    /// </summary>
	public class TestContract : ITestContract
	{
        /// <summary>
        /// 
        /// </summary>
        public TestContract()
        {
            _output.CollectionChanged += new EventHandler(_input_output_CollectionChanged);
            _input.CollectionChanged += new EventHandler(_input_output_CollectionChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Merge(ITestContract itc)
        {
            if (itc == null)
            {
                throw new ArgumentNullException();
            }

            if (itc.SecurityLevel == TestCaseSecurityLevel.FullTrust)
            {
                this.SecurityLevel = TestCaseSecurityLevel.FullTrust;
            }

            if (!String.IsNullOrEmpty(itc.Area))
                this.Area = itc.Area;

            if (!String.IsNullOrEmpty(itc.Title))
            {
                if (String.IsNullOrEmpty(this.Title))
                {
                    this.Title = itc.Title;
                }
                else
                {
                    this.Title += "_" + itc.Title;
                }
            }

            if (itc.Disabled)
                this.Disabled = true;

            if (itc.Priority > this.Priority)
                this.Priority = itc.Priority;

            if (itc.Timeout != 0)
                this.Timeout = itc.Timeout;

            Input.AddRange(itc.Input);

            Output.AddRange(itc.Output);

            SupportFiles.AddRange(itc.SupportFiles);
        }

        void _input_output_CollectionChanged(object sender, EventArgs e)
        {
            CustomListEventArgs args = e as CustomListEventArgs;
            
            if (args != null && args.Action == CustomListActions.Add)
            {
                StorageItem si = (StorageItem)args.Object;

                if (args.CustomList == _output)
                {
                    si.StorageItemType = StorageItemType.Output;
                }
                else
                {
                    si.StorageItemType = StorageItemType.Input;
                }
            }
        }


        #region ITestContract Members




        ///<summary>
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public StorageItemCollection Input
        {
            get
            {
                return _input;
            }
        }

        ///<summary>
        ///</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public StorageItemCollection Output
        {
            get
            {
                return _output;
            }
        }


        ///<summary>
        ///</summary>
        public string Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
            }
        }

        ///<summary>
        ///</summary>
        public TestCaseSecurityLevel SecurityLevel
        {
            get
            {
                return _securityLevel;
            }
            set
            {
                _securityLevel = value;
            }
        }

        ///<summary>
        ///</summary>
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        ///<summary>
        ///</summary>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public StringCollection SupportFiles
        {
            get
            {
                return _supportFiles;
            }
        }

        ///<summary>
        ///</summary>
        [DefaultValue(false)]
        public bool IsRoot
        {
            get
            {
                return _isRoot;
            }
            set
            {
                _isRoot = value;
            }
        }


        ///<summary>
        ///</summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        }


        ///<summary>
        ///</summary>
        [DefaultValue(false)]
        public bool Disabled
        {
            get
            {
                return _disabled;
            }
            set
            {
                _disabled = value;
            }
        }

        /// <summary> 
        /// </summary>
        [DefaultValue(60)]
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeSupportFiles()
        {
            return _isRoot;
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeInput()
        {
            return _isRoot;
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeOutput()
        {
            return _isRoot;
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializePriority()
        {
            return _isRoot && _priority != -1;
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeSecurityLevel()
        {
            return _isRoot && _securityLevel == TestCaseSecurityLevel.FullTrust;
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeTitle()
        {
            return _isRoot && !String.IsNullOrEmpty(_title);
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeDescription()
        {
            return _isRoot && !String.IsNullOrEmpty(_description);
        }

        ///<summary>
        ///</summary>
        public bool ShouldSerializeArea()
        {
            return _isRoot && !String.IsNullOrEmpty(_area);
        }

        



        #endregion

        bool _disabled = false;
        int _priority = -1;
        bool _isRoot = false;
        private StringCollection _supportFiles = new StringCollection();
        private StorageItemCollection _input = new StorageItemCollection();
        private StorageItemCollection _output = new StorageItemCollection();
        private string _description = "";
        private string _title = "";
        private string _area = "";
        private TestCaseSecurityLevel _securityLevel = TestCaseSecurityLevel.PartialTrust;
        private int _timeout = 60;

	}
}
