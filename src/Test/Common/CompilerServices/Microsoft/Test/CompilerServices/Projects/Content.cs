using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Projects
{
    /// <summary>
    /// A content resource used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Content
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content(string fileName)
            : this(fileName, "Always")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content(string fileName, string copyToOutputDirectory)
        {
            FileName = fileName;
            CopyToOutputDirectory = copyToOutputDirectory;
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
        public string CopyToOutputDirectory
        {
            get
            {
                return _copyToOutputDirectory;
            }
            set
            {
                _copyToOutputDirectory = value;
            }
        }
        private string _copyToOutputDirectory;
    }
}
