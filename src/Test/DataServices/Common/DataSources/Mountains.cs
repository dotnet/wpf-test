// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    public class Mountains : ObservableCollection<Mountain>
    {
        public Mountains()
        {
            // Whistler
            Mountain whistler = new Mountain("Whistler");
            Lift bigRedExpress = new Lift("Big Red Express");
            bigRedExpress.Runs.Add("Headwall");
            bigRedExpress.Runs.Add("Fisheye");
            bigRedExpress.Runs.Add("Jimmy's Joker");
            Lift garbanzoExpress = new Lift("Garbanzo Express");
            garbanzoExpress.Runs.Add("Raven");
            Lift orangeChair = new Lift("Orange chair");
            orangeChair.Runs.Add("Orange peel");
            orangeChair.Runs.Add("Banana peel");
            orangeChair.Runs.Add("Upper Dave Murray Downhill");
            whistler.Lifts.Add(bigRedExpress);
            whistler.Lifts.Add(garbanzoExpress);
            whistler.Lifts.Add(orangeChair);

            // Stevens Pass
            Mountain stevensPass = new Mountain("Stevens Pass");
            Lift tyeMill = new Lift("Tye Mill");
            tyeMill.Runs.Add("Roller coaster");
            tyeMill.Runs.Add("Skid road");
            tyeMill.Runs.Add("Crest trail");
            Lift jupiterChair = new Lift("Jupiter chair");
            jupiterChair.Runs.Add("Corona bowl");
            jupiterChair.Runs.Add("Lower gemini");
            Lift southernCrossChair = new Lift("Southern cross chair");
            southernCrossChair.Runs.Add("Orion");
            southernCrossChair.Runs.Add("Aquarius face");
            stevensPass.Lifts.Add(tyeMill);
            stevensPass.Lifts.Add(jupiterChair);
            stevensPass.Lifts.Add(southernCrossChair);

            // Crystal Mountain
            Mountain crystal = new Mountain("Crystal Mountain");
            Lift rainierExpress = new Lift("Rainier Express");
            rainierExpress.Runs.Add("Iceberg ridge");
            rainierExpress.Runs.Add("Pro course");
            rainierExpress.Runs.Add("Lucky shot");
            Lift greenValley = new Lift("Green Valley");
            greenValley.Runs.Add("Green back");
            greenValley.Runs.Add("Northway ridge");
            crystal.Lifts.Add(rainierExpress);
            crystal.Lifts.Add(greenValley);

            this.Add(whistler);
            this.Add(stevensPass);
            this.Add(crystal);
        }
    }

    public class Mountain
    {
        private ObservableCollection<Lift> _lifts;

        public ObservableCollection<Lift> Lifts
        {
            get { return _lifts; }
            set { _lifts = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Mountain(string name)
        {
            this._name = name;
            _lifts = new ObservableCollection<Lift>();
        }
    }

    public class Lift
    {
        private ObservableCollection<string> _runs;

        public ObservableCollection<string> Runs
        {
            get { return _runs; }
            set { _runs = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Lift(string name)
        {
            this._name = name;
            _runs = new ObservableCollection<string>();
        }
    }
}
