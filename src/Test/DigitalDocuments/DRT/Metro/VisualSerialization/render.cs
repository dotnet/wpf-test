// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;                       // for File, Stream
using System.Diagnostics;
using System.Collections;
using System.Xml;
using System.Threading;
using System.Reflection;

using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;

using System.Printing;                 // for PrintQueue. System.Printing.dll
using System.Windows.Xps;

using System.Windows.Documents;        // for FixedPage PresentationFramework.dll
using System.Windows.Markup;    // for ParserContext PresentationFramework.dll
using System.Windows.Controls;         // for FixedDocument    PresentationFramework.dll
using System.Windows.Shapes;           // for Glyphs
using System.Windows.Media.Animation;

using System.Windows.Xps.Serialization;

namespace Microsoft.PrintTest
{
    public class Clear : DrawingVisual
    {
        public Clear(double width, double height) : base()
        {
            using (DrawingContext ctx = RenderOpen())
            {
                ctx.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
            }
        }
    }

    class Render
    {
        int          _dpi;
        StreamWriter _sw;

        public Render(int dpi, string logname, string title, string left, string right)
        {
            _dpi = dpi;


            if (logname != null)
            {
                _sw = new StreamWriter(logname);

                _sw.WriteLine("<html>");

                _sw.WriteLine("<title>");
                _sw.Write(title);
                _sw.WriteLine("</title>");

                _sw.WriteLine("<head><h1><center>");
                _sw.Write(title);
                _sw.Write("<br>");
                _sw.WriteLine(DateTime.Now);
                _sw.WriteLine("</center></h1></head>");

                _sw.WriteLine("<body>");
                _sw.WriteLine("<table border=\"1\">");

                _sw.WriteLine("<tr> <th>Test Case</th> <th>Error</th> <th>{0}</th> <th>{1}</th> </tr>", left, right);
            }
        }

        public void Close()
        {
            if (_sw != null)
            {
                _sw.WriteLine("</table>");
                _sw.WriteLine("</body></html>");
                _sw.Close();
            }
        }

        static public RenderTargetBitmap Rasterize(Visual visual, double width, double height, int dpi)
        {
            int pixelWidth  = (int)( width * dpi);
            int pixelHeight = (int)(height * dpi);

            RenderTargetBitmap id = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);

            id.Render(new Clear(width * 96, height * 96));
            id.Render(visual);

            return id;
        }
 
        static public void Save(RenderTargetBitmap id, string filename)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(id));
            
            Stream imageStreamDest = new System.IO.FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.None);

            encoder.Save(imageStreamDest);
        }

        static public Byte [] GetPixels(BitmapSource id)
        {
            int width  = id.PixelWidth;
            int height = id.PixelHeight;
            int stride = width * 4;

            Byte[] pixels = new Byte[stride * height];

            FormatConvertedBitmap converter = new FormatConvertedBitmap();

            converter.BeginInit();
            converter.Source = id;
            converter.DestinationFormat = PixelFormats.Bgra32;
            converter.EndInit();

            converter.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return pixels;
        }

        static public double Diff(RenderTargetBitmap one, RenderTargetBitmap two)
        {
            int width  = one.PixelWidth;
            int height = one.PixelHeight;
            int size   = width * height * 4;
            int stride = width * 4;

            Debug.Assert(width  == two.PixelWidth,   "width does not match");
            Debug.Assert(height == two.PixelHeight, "height does not match");

            Byte[] p1 = GetPixels(one);
            Byte[] p2 = GetPixels(two);

            double sum = 0;

            for (int s = 0; s < size; s++)
            {
                int diff = p1[s] - p2[s];

                if (diff != 0)
                {
                    sum += Math.Abs(diff);
                }
            }

            return sum / size / 255;  // average difference
        }

        public void Compare(Visual vBefore, Visual vAfter, string name, double width, double height, string comment, double duration)
        {
            Console.Write(name);

            width  /= 96;
            height /= 96;

            RenderTargetBitmap before = Rasterize(vBefore, width, height, _dpi);

            Save(before, name + "0.png");

            RenderTargetBitmap after = Rasterize(vAfter, width, height, _dpi);

            Save(after, name + "1.png");

            double error = Diff(before, after);

            Console.WriteLine(" error = {0:F2}%   " + comment, error * 100);
            Console.WriteLine("  " + duration);

            if (_sw != null)
            {
                _sw.Write("<tr> <td><center>" + name + "<br>" + comment + "<br>" + duration + "<center></td> ");
                _sw.Write("<td>{0:F2}%</td>", error * 100);

                string n = "\"" + name + "0.png" + "\"";

                _sw.Write("<td><img src=" + n + " width=\"400\" height=\"400\"/><br>");
                _sw.Write("<a href=" + n + "><center>" + n + "</center></td>");

                n = "\"" + name + "1.png" + "\"";

                _sw.Write("<td><img src=" + n + " width=\"400\" height=\"400\"/><br>");
                _sw.Write("<a href=" + n + "><center>" + n + "</center></td>");

                _sw.WriteLine("</tr>");
            }
        }
    }

    class Toolbox
    {
        public static void SaveAsXml(string filename, Visual visual, bool AsElement)
        {
            if (AsElement)
            {
                Stream stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                stream.SetLength(0);

                XmlTextWriter xmlWriter = new XmlTextWriter(stream, null);

                try
                {
                    xmlWriter.Formatting = System.Xml.Formatting.Indented;
                    xmlWriter.Indentation = 4;

                    XamlWriter.Save(visual as UIElement, xmlWriter); //, null);
                }
                catch (Exception) // Saving without indentation
                {
                    // xmlWriter.Close();

                    xmlWriter = new XmlTextWriter(stream, null);

                    XamlWriter.Save(visual as UIElement, xmlWriter); //, null);
                }
                finally
                {
                    xmlWriter.Close();
                }

                stream.Close();
            }
            else
            {
                FileStream    _stream;
                XmlTextWriter _writer;
                
		        _stream = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite);
                _writer = new System.Xml.XmlTextWriter(_stream, System.Text.Encoding.UTF8);
                
                _writer.Formatting = System.Xml.Formatting.Indented;
                _writer.Indentation = 4;
                _writer.WriteStartElement("FixedDocument");
                _writer.WriteAttributeString("xmlns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                _writer.WriteAttributeString("xmlns:x", "http://schemas.microsoft.com/winfx/2006/xaml");

                _writer.WriteStartElement("PageContent");
                _writer.WriteStartElement("FixedPage");
                _writer.WriteAttributeString("Width",  "816");
                _writer.WriteAttributeString("Height", "1056");
                _writer.WriteAttributeString("Background", "White");
                _writer.WriteStartElement("Canvas");

                System.IO.StringWriter resString = new StringWriter(); // System.Text.Encoding.UTF8);

                System.Xml.XmlTextWriter resWriter = new System.Xml.XmlTextWriter(resString);
                resWriter.Formatting = System.Xml.Formatting.Indented;
                resWriter.Indentation = 4;

                System.IO.StringWriter bodyString = new StringWriter(); // System.Text.Encoding.UTF8);

                System.Xml.XmlTextWriter bodyWriter = new System.Xml.XmlTextWriter(bodyString);
                bodyWriter.Formatting = System.Xml.Formatting.Indented;
                bodyWriter.Indentation = 4;
		
                VisualTreeFlattenerSaveAsXml(visual, resWriter, bodyWriter, filename);

                resWriter.Close();
                bodyWriter.Close();

                _writer.Flush();
                _writer.WriteRaw(resString.ToString());
                _writer.WriteRaw(bodyString.ToString());

                _writer.WriteEndElement();
                _writer.WriteEndElement();
                _writer.WriteEndElement();

                _writer.WriteEndElement(); // FixedDocument
                _writer.Close();
                _stream.Close();
                _writer = null;
                _stream = null;
            }

            Console.WriteLine(filename + " saved. " + DateTime.Now);
        }
	
	private static void VisualTreeFlattenerSaveAsXml(Visual visual, XmlWriter resWriter, XmlWriter bodyWriter, string fileName)
	{
		Assembly reachFrameworkAssembly = typeof(System.Windows.Xps.Packaging.XpsDocument).Assembly;
		
                Type visualTreeFlattenerType = reachFrameworkAssembly.GetType(
			"System.Windows.Xps.Serialization.VisualTreeFlattener", 
			true /* throw on error */, 
			false /* ignore case */
			);
			
		MethodInfo visualTreeFlattener_SaveAsXmlMethod = visualTreeFlattenerType.GetMethod(
			"SaveAsXml",
			BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.DeclaredOnly
			);
			
		visualTreeFlattener_SaveAsXmlMethod.Invoke(
			null, 
			new object[] {visual, resWriter, bodyWriter, fileName }
			);
	}

        static public void UpdateLayout(UIElement page, Size sz)
        {
            page.Measure(sz);
            page.Arrange(new Rect(new Point(), sz));
            page.UpdateLayout();
        }

        static Visual CreateXpsLimitVisual()
        {
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                double large = 2e38;
                double tiny  = 1e-40;
                
                ctx.DrawRectangle(Brushes.White, null, new Rect(-tiny, -large, large, large));
                ctx.DrawRectangle(Brushes.White, null, new Rect(-tiny, tiny, large, large));

                PathGeometry path = new PathGeometry();

                for (int i = 0; i < 64000; i ++)
                {
                    path.AddGeometry(new RectangleGeometry(new Rect(0, 0, i, i), 100, 100, Transform.Identity));
                }
                
                ctx.DrawGeometry(Brushes.Black, null, path);
            }
            
            return dv;
        }
        
        static public Visual Load(string fileName, out Size size)
        {
            if ((fileName == "xpslimit") || File.Exists(fileName))
            {
                Console.WriteLine("Loading " + fileName);

                object elm;
                 
                if (fileName == "xpslimit")
                {
                    elm = CreateXpsLimitVisual();
                }
                else
                {                                
                    Stream s = File.OpenRead(fileName);

                    elm = XamlReader.Load(s);
                    s.Close();
                }
                
                Console.WriteLine(fileName + " loaded");
                
                double width = 8.5 * 96;
                double height = 11 * 96;

                if (elm is FixedDocument)
                {
                    FixedDocument panel = elm as FixedDocument;

                    Console.WriteLine("Calling GetPageRoot");
                
                    elm = panel.Pages[0].GetPageRoot(false /*forceReload*/);

                    Console.WriteLine("GetPageRoot returns");
                
                //  width = panel.PageWidth;
                //  height = panel.PageHeight;
                }

                if (elm is FlowDocument)
                {
                    FlowDocument doc = elm as FlowDocument;
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
                
                    Console.WriteLine("Calling GetPage");
                
                    DocumentPage p = paginator.GetPage(0);
                
                    Console.WriteLine("GetPage returns");
                
                    if (p != null)
                    {
                        size = p.Size;

                        return p.Visual;
                    }
                }

                FrameworkElement page = elm as FrameworkElement;

                if (page != null)
                {
                    double w = page.Width;
                    double h = page.Height;

                    if ((w != 0) && !double.IsNaN(w))
                    {
                        width = w;
                    }

                    if ((h != 0) && !double.IsNaN(h))
                    {
                        height = h;
                    }
                }

                size = new Size(width, height);

                return elm as Visual;
            }

            size = new Size(0, 0);

            return null;
        }

        static public Visual LoadVisual(string filename, out Size size)
        {
            Visual v = Load(filename, out size);

            if (v != null)
            {
                UIElement page = v as UIElement;

                if (page != null)
                {
                    Console.WriteLine("Calling UpdateLayout");
                
                    UpdateLayout(page, size);

                    Console.WriteLine("UpdateLayout returns");
                
                    v = page;
                }
            }
            else
            {
                Console.WriteLine("Missing file " + filename);
            }

            return v;
        }

        /// <summary>
        /// Convert UIElement to Visual using AlphaFlattener
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="page"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Visual FlattenToVisual(PrintQueue queue, Visual page, double width, double height)
        {
            XpsDocumentWriter pad = PrintQueue.CreateXpsDocumentWriter(queue);
            
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                FrameworkElement elm = new FrameworkElement();

                ResourceDictionary res = elm.Resources;

                res.Add("Destination", ctx);

                if ((width > 0) && (height > 0))
                {
                    ctx.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));
                }

                VisualTreeFlattenerSaveAsXml(elm, null, null, null);

                pad.Write(page); // as Visual
            }

            return dv;
        }
    }
}   
