// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Types
{
    public class BC_MarkupExtension : MarkupExtension
    {
        private string _value;

        public BC_MarkupExtension() { }
        public BC_MarkupExtension(string value)
        {
            _value = value;
        }

        public BC_MarkupExtension(string value, string value2)
        {
            _value = value;
            Mode = value2;
        }

        public override object ProvideValue(
                        IServiceProvider serviceProvider)
        {
            return string.Format("Path: {0} , Mode: {1}", Path, Mode);
        }
        [MarkupExtensionBracketCharacters('(',')')]
        [MarkupExtensionBracketCharacters('[',']')]
        [ConstructorArgument("value")]
        public string Path
        {
            get { return _value; }
            set { _value = value; }
        }

        [ConstructorArgument("value2")]
        [MarkupExtensionBracketCharacters('$','^')]
        public string Mode
        {
            get; set;
        }
    }

    public class BCNested_MarkupExtension : MarkupExtension
    {
        private string _value;

        public BCNested_MarkupExtension() { }
        public BCNested_MarkupExtension(string path)
        {
            _value = path;
        }

        public override object ProvideValue(
                        IServiceProvider serviceProvider)
        {
            return Path;
        }

        [MarkupExtensionBracketCharacters('(',')')]
        [MarkupExtensionBracketCharacters('[', ']')]
        public string Path
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
