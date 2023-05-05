// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.Test.Serialization;
using System.Xml;
using System.Windows.Markup;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Validate
{
    /// <summary>
    ///    Validate parameters for SerializationHelper.SerializeObjectTree and LoadXml
    /// </summary>
    public class ParserParameterValidation
    {
        #region Construction

        /// <summary>
        ///     Constructor for ParserParameterValidation
        /// </summary>
        public ParserParameterValidation()
        {
        }

        #endregion Construction

        /// <summary>
        /// Main Entry point
        /// </summary>
        public void RunTest()
        {
            GlobalLog.LogStatus("begin run parameter validation for SerializationHelper.SerializeObjectTree");
            //Validate parameters for SaveAsXml
            ParameterValidationParserSaveAsXml1();
            ParameterValidationParserSaveAsXml2();
            ParameterValidationParserSaveAsXml3();
            ParameterValidationParserSaveAsXml4();
            ParameterValidationParserSaveAsXml5();
            //Currently internal
            //ParameterValidationParserSaveAsXml6().

            //Validate parameters for LoadXml
            ParameterValidationLoadXml1();
            ParameterValidationLoadXml2();
            ParameterValidationLoadXml3();

        }

        /// <summary>
        /// Validate parameter for function: the first SerializationHelper.SerializeObjectTree
        /// </summary>
        private void ParameterValidationParserSaveAsXml1()
        {
            bool catched = false;
            try
            {
                SerializationHelper.SerializeObjectTree(null);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }

        /// <summary>
        /// Validate parameter for function: the secont SerializationHelper.SerializeObjectTree
        /// </summary>
        private void ParameterValidationParserSaveAsXml2()
        {
            TextWriter writer = null;
            object obj = null;
            bool catched = false;
            try
            {
                SerializationHelper.SerializeObjectTree(obj, writer);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }

            obj = new object();
            try
            {
                SerializationHelper.SerializeObjectTree(obj, writer);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }


        /// <summary>
        /// Validate parameter for function: the third SerializationHelper.SerializeObjectTree
        /// </summary>
        private void ParameterValidationParserSaveAsXml3()
        {
            Stream stream = null;
            object obj = null;
            bool catched = false;
            try
            {
                SerializationHelper.SerializeObjectTree(obj, stream);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }

            obj = new object();
            try
            {
                SerializationHelper.SerializeObjectTree(obj, stream);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }

        /// <summary>
        /// Validate parameter for function: the fourth SerializationHelper.SerializeObjectTree
        /// </summary>
        private void ParameterValidationParserSaveAsXml4()
        {
            object obj = null;
            XmlTextWriter xmlWriter = null;

            bool catched = false;
            try
            {
                SerializationHelper.SerializeObjectTree(obj, xmlWriter);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }

            obj = new object();
            try
            {
                SerializationHelper.SerializeObjectTree(obj, xmlWriter);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
            xmlWriter = new XmlTextWriter(Console.Out);
            xmlWriter.Formatting = Formatting.Indented;
            try
            {
                SerializationHelper.SerializeObjectTree(obj, xmlWriter);
            }
            catch (ArgumentException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentException");
            }
        }


        /// <summary>
        /// Validate parameter for function: the fifth SerializationHelper.SerializeObjectTree
        /// </summary>
        private void ParameterValidationParserSaveAsXml5()
        {
            object obj = null;
            XamlDesignerSerializationManager manager = null;
            bool catched = false;
            try
            {
                SerializationHelper.SerializeObjectTree(obj, manager);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }

            obj = new object();
            try
            {
                SerializationHelper.SerializeObjectTree(obj, manager);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }

        /// <summary>
        /// Validate parameter for function: the first System.Windows.Markup.XamlReader.Load
        /// </summary>
        private void ParameterValidationLoadXml1()
        {
            Stream stream = null;
            bool catched = false;
            try
            {
                System.Windows.Markup.XamlReader.Load(stream);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }

        /// <summary>
        /// Validate parameter for function: the second System.Windows.Markup.XamlReader.Load
        /// </summary>
        private void ParameterValidationLoadXml2()
        {
            XmlReader reader = null;
            bool catched = false;
            try
            {
                System.Windows.Markup.XamlReader.Load(reader);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }

        /// <summary>
        /// Validate parameter for function: the third System.Windows.Markup.XamlReader.Load
        /// </summary>
        private void ParameterValidationLoadXml3()
        {
            bool catched = false;
            try
            {
                System.Windows.Markup.XamlReader.Load(null, null);
            }
            catch (ArgumentNullException)
            {
                catched = true;
            }
            catch (Exception)
            {
            }
            if (catched == false)
            {
                throw new Microsoft.Test.TestValidationException("Should Throw ArgumentNullException");
            }
        }
    }
}
