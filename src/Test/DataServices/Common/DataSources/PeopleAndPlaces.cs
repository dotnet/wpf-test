// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;

namespace Microsoft.Test.DataServices
{
    public class PeopleAndPlaces :ArrayList
    {
        public PeopleAndPlaces()
        {
            Add(new Place("Seattle", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Person("Marisa", "indonesian"));
            Add(new Person("Lilly", "taiwanese"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("San Jose", "CA"));
            Add(new Person("Radu", "romanian"));
            Add(new Place("Bellingham", "WA"));
            Add(new Person("Beatriz", "portuguese"));
            Add(new Person("Vincent", "belgian"));
            Add(new Person("Michael", "american"));

        }
    }
}
