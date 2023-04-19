using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Projects
{
    /// <summary>
    /// A reference used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Reference
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference(string fileName)
            : this(fileName, "")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference(string fileName, string hintPath)
        {
            FileName = fileName;
            HintPath = hintPath;
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

        /// <summary>
        /// </summary>
        public string HintPath
        {
            get
            {
                return _hintPath;
            }
            set
            {
                _hintPath = value;
            }
        }

        private string _fileName;
        private string _hintPath;
    }
}
