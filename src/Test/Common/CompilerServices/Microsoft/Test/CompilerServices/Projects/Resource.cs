using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Projects
{
    /// <summary>
    /// A resource used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Resource
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName)
            : this(fileName, "embedded")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName, string fileStorage)
            : this(fileName, fileStorage, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName, string fileStorage, bool localizable)
        {
            FileName = fileName;
            FileStorage = fileStorage;
            Localizable = localizable;
        }

        /// <summary>
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        private string _fileName;

        /// <summary>
        /// </summary>
        public string FileStorage
        {
            get
            {
                return _fileStorage;
            }
            set
            {
                _fileStorage = value;
            }
        }
        private string _fileStorage;

        /// <summary>
        /// </summary>
        public bool Localizable
        {
            get
            {
                return _localizable;
            }
            set
            {
                _localizable = value;
            }
        }
        private bool _localizable;
    }
}
