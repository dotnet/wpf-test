using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System;

namespace Microsoft.Test.Controls
{
    public class Model : INotifyPropertyChanged
    {
        ScrollingItemCollection _items = new ScrollingItemCollection();
        public ScrollingItemCollection Items
        {
            get { return _items; }
        }

        public static Model Create()
        {
            Model model = new Model();
            ScrollingItemCollection items = model.Items;
            for (int i=0; i<50; ++i)
            {
                items.Add(new ScrollingItem { Height = 29 });
            }

            items[10].Height = 599;

            return model;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class ScrollingItem : INotifyPropertyChanged
    {
        static int s_Count;

        public ScrollingItem()
        {
            _seqno = s_Count++;
        }

        int _seqno;
        public int SeqNo { get { return _seqno; } }

        double _height;
        public double Height
        {
            get { return _height; }
            set { _height = value; OnPropertyChanged("Height"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}", SeqNo);
        }
    }

    public class ScrollingItemCollection : ObservableCollection<ScrollingItem>
    {
    }
}
