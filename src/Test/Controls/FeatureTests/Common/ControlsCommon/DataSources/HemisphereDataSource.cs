using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Avalon.Test.ComponentModel
{
 
    public class Hemisphere : INotifyPropertyChanged
    {
        private string hemisphereName;
        private ObservableCollection<Region> regions;

        #region Properies
        public string HemisphereName
        {
            get { return hemisphereName; }
            set
            {
                hemisphereName = value;
                OnPropertyChanged("PlanetName");
            }
        }

        public ObservableCollection<Region> Regions
        {
            get { return regions; }
            set
            {
                regions = value;
                OnPropertyChanged("Regions");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Hemisphere(string hemisphereName, ObservableCollection<Region> regions)
        {
            this.hemisphereName = hemisphereName;
            this.regions = regions;
        }
    } 
}
