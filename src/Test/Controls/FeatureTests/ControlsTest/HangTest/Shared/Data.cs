using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace HangTest
{
    #region Data item

    public class Data
    {
        static int s_count;
        static Distribution _heightDistribution, _marginDistribution;

        public static void Reset(Distribution heightDistribution, Distribution marginDistribution) 
        { 
            s_count = 0;
            _heightDistribution = heightDistribution;
            _marginDistribution = marginDistribution;
        }

        public string TagName { get; set; }
        public string Info { get; set; }
        public string Path { get; set; }
        public int Depth { get; set; }
        public int Size { get; set; }
        public int SeqNo { get; private set; }
        public double Height { get; private set; }
        public double Margin { get; private set; }
        public DataCollection Children { get; set; } = new DataCollection();

        public Data()
        {
            SeqNo = s_count++;
            Height = _heightDistribution.NextDouble();
            Margin = _marginDistribution.NextDouble();
        }

        public string Label { get { return String.Format("[{0}] {1} {2} - {3}", SeqNo, Depth, Path, Size); } }
        public override string ToString() { return Label; }
    }

    public class DataRoot : Data
    {
        public DataRoot()
        {
            Path = String.Empty;
            Depth = -1;
        }

        #region File

        public static DataRoot ReadDataFromFile(string filename, string filterAttribute, string childrenTag)
        {
            DataRoot root = new DataRoot() { TagName = childrenTag };
            if (File.Exists(filename))
            {
                var doc = XDocument.Load(filename);
                using (XmlReader reader = doc.CreateReader())
                {
                    ReadSubtree(reader, filterAttribute, childrenTag, root);
                }
            }
            return root;
        }

        private static void ReadSubtree(XmlReader reader,
                                        string filterAttribute,
                                        string childrenTag,
                                        Data parent)
        {
            string parentPath = parent.Path;
            int depth = parent.Depth + 1;

            int index = 0;
            bool skip = false;
            while (skip || reader.Read())
            {
                if (reader.IsStartElement())
                {
                    var name = reader.GetAttribute(filterAttribute);
                    if (!string.IsNullOrEmpty(name))
                    {
                        string path = String.IsNullOrEmpty(parentPath) ? String.Format("{0}", index) : String.Format("{0}.{1}", parentPath, index);
                        ++index;
                        Data d = new Data() { TagName = reader.LocalName, Info = name, Path = path, Depth = depth };
                        if (reader.LocalName == childrenTag)
                        {
                            ReadSubtree(reader, filterAttribute, childrenTag, d);
                        }
                        else
                        {
                            skip = true;
                            reader.Skip();
                        }

                        d.Size = 1 + TotalSize(d.Children);

                        parent.Children.Add(d);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }
        }

        #endregion File

        #region Fixed height

        public static DataRoot CreateFixedHeightData(List<Distribution> distributionsByLevel)
        {
            DataRoot root = new DataRoot();
            CreateSubtreeAtLevel(distributionsByLevel, root);
            return root;
        }

        static void CreateSubtreeAtLevel(List<Distribution> distributionsByLevel,
                                         Data parent)
        {
            int depth = parent.Depth + 1;
            string parentPath = parent.Path;

            if (depth >= distributionsByLevel.Count)
                return;

            int childCount = distributionsByLevel[depth].NextInt(1);
            for (int index = 0; index < childCount; ++index)
            {
                string path = String.IsNullOrEmpty(parentPath) ? String.Format("{0}", index) : String.Format("{0}.{1}", parentPath, index);
                Data d = new Data() { Info = "Lorem ipse", Path = path, Depth = depth };
                CreateSubtreeAtLevel(distributionsByLevel, d);
                d.Size = 1 + TotalSize(d.Children);
                parent.Children.Add(d);
            }
        }

        #endregion Fixed height

        #region Non-uniform

        public static DataRoot CreateNonUniformData(Distribution sizeDistribution, Distribution partitionDistribution)
        {
            BaseRandom = sizeDistribution.BaseRandom;

            DataRoot root = new DataRoot();
            int size = sizeDistribution.NextInt(10);
            Partition(size, root, partitionDistribution);
            return root;
        }

        // partition a collection of given size into pieces of randomly-chosen sizes
        static void Partition(int size,
                              Data parent,
                              Distribution partitionDistribution)
        {
            int depth = parent.Depth + 1;
            string parentPath = parent.Path;
            List<int> sizes = new List<int>();

            // choose child sizes
            while (size > 0)
            {
                // allocate a child with a random fraction of the remaining size
                int childSize = 1 + (int)(partitionDistribution.NextDouble(0.4, 1.0) * size);
                sizes.Add(childSize);
                size -= childSize;
            }

            // randomly permute the child sizes
            for (int i = 1; i < sizes.Count; ++i)
            {
                int j = BaseRandom.Next(0, i + 1);
                int t = sizes[i];
                sizes[i] = sizes[j];
                sizes[j] = t;
            }

            // create the subtrees
            for (int index = 0; index < sizes.Count; ++index)
            {
                string path = String.IsNullOrEmpty(parentPath) ? String.Format("{0}", index) : String.Format("{0}.{1}", parentPath, index);
                Data d = new Data() { Info = "lorem ipse", Path = path, Depth = depth };
                Partition(sizes[index] - 1, d, partitionDistribution);
                d.Size = 1 + TotalSize(d.Children);
                parent.Children.Add(d);
            }
        }

        static Random BaseRandom { get; set; }

        #endregion Non-uniform

        public static int TotalSize(DataCollection col)
        {
            int size = 0;
            foreach (Data child in col)
            {
                size += child.Size;
            }
            return size;
        }
    }

    #endregion Data item

    public class DataCollection : ObservableCollection<Data>
    {
    }
}
