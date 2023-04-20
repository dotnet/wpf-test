// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace Microsoft.Test.DataServices
{
    public class GreekGods : ObservableCollection<string>
    {
        public GreekGods()
        {
            Add("Aphrodite");
            Add("Apollo");
            Add("Ares");
            Add("Artemis");
            Add("Athena");
            Add("Demeter");
            Add("Dionysus");
            Add("Hephaestus");
            Add("Hera");
            Add("Hermes");
            Add("Poseidon");
            Add("Zeus");
        }
    }
}
