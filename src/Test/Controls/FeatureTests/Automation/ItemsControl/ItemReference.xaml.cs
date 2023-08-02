using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls.ItemReference
{
    public partial class ItemReference : Page
    {
        Model _model;

        public ItemReference()
        {
            InitializeModel();
            InitializeComponent();
        }

        void InitializeModel()
        {
            _model = new Model();
            DataContext = _model;
        }

        void btn_Replace(object sender, EventArgs e)
        {
            _model.RefreshStringData();
        }

        void btn_GC(object sender, RoutedEventArgs e)
        {
            _model.DoGC();
        }
    }

    public class Model : INotifyPropertyChanged
    {
         List<Stock> _stocks = new List<Stock>();
         List<string> _stringData;
         List<StockStruct> _valueTypeData;
         Random _rng = new Random();
         static MethodInfo _miCleanup;

         public List<string> StringData { get { return _stringData; } }
         public List<StockStruct> ValueTypeData { get { return _valueTypeData; } }

         public Model()
         {
             _miCleanup =
                 typeof(System.Windows.Data.BindingOperations).GetMethod("Cleanup",
                     BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

             _stocks.Add(new Stock("ABC", 24));
             _stocks.Add(new Stock("DEF", 57));
             _stocks.Add(new Stock("GHI", 41));

             RefreshStringData();
             RefreshValueTypeData();
         }

         public void RefreshStringData()
         {
             ChangePrices();

             List<string> list = new List<string>(_stocks.Count);
             foreach (Stock stock in _stocks)
             {
                 list.Add(String.Format("{0} trading at {1}", stock.Symbol, stock.Price));
             }

             _stringData = list;
             OnPropertyChanged(nameof(StringData));
         }

         public void RefreshValueTypeData()
         {
             ChangePrices();

             List<StockStruct> list = new List<StockStruct>(_stocks.Count);
             foreach (Stock stock in _stocks)
             {
                 list.Add(new StockStruct(stock));
             }

             _valueTypeData = list;
             OnPropertyChanged(nameof(ValueTypeData));
         }

         void ChangePrices()
         {
             // don't change the first stock.  It gets a new string that's Object.Equals to the old one.
             int delta = 0;

             foreach (Stock stock in _stocks)
             {
                 stock.Price += delta;

                 // change the next stock by {-1, 0, +1}
                 delta = _rng.Next(3) - 1;
             }
         }

         public void DoGC()
         {
             GetMemory();
         }

         public static long GetMemory()
         {
             long result = GC.GetTotalMemory(true);
             GC.WaitForPendingFinalizers();
             // while (BindingOperations.Cleanup())
             while ((bool)_miCleanup.Invoke(null, null))
             {
                 result = GC.GetTotalMemory(true);
                 GC.WaitForPendingFinalizers();
             }
             return result;
         }

         public event PropertyChangedEventHandler PropertyChanged;

         void OnPropertyChanged(string name)
         {
             PropertyChangedEventHandler handler = PropertyChanged;
             if (handler != null)
                 handler(this, new PropertyChangedEventArgs(name));
         }
    }

    public class Stock
    {
        public string Symbol { get; private set; }
        public int Price { get; set; }

        public Stock(string symbol, int price)
        {
            Symbol = symbol;
            Price = price;
        }
    }

    public struct StockStruct
    {
        string _symbol;
        int _price;

        public StockStruct(Stock stock)
        {
            _symbol = stock.Symbol;
            _price = stock.Price;
        }

        public string Symbol { get { return _symbol; } }
        public int Price { get { return _price; } }
    }

}
