namespace WFCTestLib.Util {
    using System;
    using System.Collections;

    //
    // System.Collections.StringTable was removed in Beta 2, so I decided to
    // implement my own.
    //
    // StringTable is a thin wrapper around Hashtable for storing and looking up
    // strings.  Several ICollection methods were copied from
    // System.Collections.StringTable before the class was removed in Beta 2.
    //
    [Serializable]
    public class StringTable : ICollection
    {
        private Hashtable table;

        public StringTable() {
            table = new Hashtable();
        }
    
        public StringTable(int capacity) {
            table = new Hashtable(capacity);
        }
    
        public StringTable(int capacity, float loadFactor) {
            table = new Hashtable(capacity, loadFactor);
        }

        public virtual int Count {
            get { return table.Count; }
        }
   
        public virtual void Add(String key) {
            if ( !table.ContainsKey(key) )
                table.Add(key, null);
        }
        
        public virtual void Clear() {
            table.Clear();
        }       
        
        public virtual bool Contains(String key) {
            return table.ContainsKey(key);
        }
        
        public virtual void CopyTo(Array array, int index) {
            table.Keys.CopyTo(array, index);
        }
    
        public virtual void CopyTo(String[] array, int index) {
            table.Keys.CopyTo(array, index);
        }
        
        public virtual IEnumerator GetEnumerator() {
            return table.GetEnumerator();
        }

        public virtual void Remove(String key) {
            table.Remove(key);
        }
    
        public virtual String[] ToArray() {
            string[] array = new string[table.Count];
            CopyTo(array, 0);
            return array;
        }
        
        //
        // Everything below was copied from System.Collections.StringTable.
        //
        public virtual Object SyncRoot {
            get { return this; }
        }   
    
        public virtual bool IsReadOnly {
            get { return false; }
        }   

        public virtual bool IsSynchronized {
            get { return false; }
        }   

        public static StringTable Synchronized(StringTable st) {
            if (st==null)
                throw new ArgumentNullException("st");

            return new SyncStringTable(st);
        }
    
        [Serializable] private class SyncStringTable : StringTable
        {
            private StringTable _s;
            private Object _root;
    
            internal SyncStringTable(StringTable st) {
                _s = st;
                _root = st.SyncRoot;
            }
    
            public override bool IsReadOnly {
                get { return _s.IsReadOnly; }
            }
    
            public override bool IsSynchronized {
                get { return true; }
            }
    
            public override Object SyncRoot {
                get {
                    return _root;
                }
            }
    
            public override int Count { 
                get { 
                    lock (_root) {
                        return _s.Count;
                    } 
                }
            }
    
            public override void Add(String key) {
                lock (_root) {
                    _s.Add(key);
                }
            }
          
            public override void Clear() {
                lock (_root) {
                    _s.Clear();
                }
            }
    
            public override bool Contains(String key) {
                lock (_root) {
                    return _s.Contains(key);
                }
            }
          
            public override void CopyTo(Array array, int arrayIndex) {
                lock (_root) {
                    _s.CopyTo(array, arrayIndex);
                }
            }
    
            public override void CopyTo(String[] array, int arrayIndex) {
                lock (_root) {
                    _s.CopyTo(array, arrayIndex);
                }
            }
    
            public override IEnumerator GetEnumerator() {
                lock (_root) {
                    return _s.GetEnumerator();
                }
            }
    
            public override void Remove(String key) {
                lock (_root) {
                    _s.Remove(key);
                }
            }
    
            public override String[] ToArray() {
                lock (_root) {
                    return _s.ToArray();
                }
            }
        }

#if TEST
        public static void Main() {
            StringTable t = new StringTable();
            t.Add("Foo");
            t.Add("Bar");
            t.Add("Foo");
            t.Add("Bletch");

            Console.WriteLine(t.Contains("Foo"));
            Console.WriteLine(t.Contains("Bar"));
            Console.WriteLine(t.Contains("Bletch"));
            Console.WriteLine(t.Contains("foo"));
            Console.WriteLine(t.Contains("Moocow"));
        }
#endif
    }
}
