using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WFCTestLib.Util
{
    public static class WebUtil
    {
        public const int PAGE_LOAD_TIMEOUT_SECONDS = 30;
        public static Rectangle ElementRectInBrowserClient(HtmlElement element)
        {
            Rectangle ret = element.OffsetRectangle;
            HtmlElement parent = element.OffsetParent;
            while (null != parent)
            {
                ret.Offset(parent.OffsetRectangle.X, parent.OffsetRectangle.Y);
                parent = parent.OffsetParent;
            }
            return ret;
        }
        public static Rectangle ElementRectInScreen(WebBrowser wb, HtmlElement element)
        { return wb.RectangleToScreen(ElementRectInBrowserClient(element)); }

        public static Point CenterRect(Rectangle r)
        { return new Point(r.Left + r.Width / 2, r.Top + r.Height / 2); }
        public static Point ElementPointInScreen(WebBrowser wb, HtmlElement element)
        { return CenterRect(ElementRectInScreen(wb, element)); }

        public static void LoadPageSync(WebBrowser wb, string url)
        {
            using (EventListener el = new EventListener(wb, "DocumentCompleted"))
            {
                wb.Navigate(url);
                WaitForDocumentCompleted(el);
            }
        }
        public static void WaitForDocumentCompleted(EventListener el)
        { WaitForDocumentCompleted(el, 1); }

        public static void WaitForDocumentCompleted(EventListener el, int count)
        { el.Block("DocumentCompleted", TimeSpan.FromSeconds(PAGE_LOAD_TIMEOUT_SECONDS), count, true); }

        public static void WaitForNavigating(EventListener el)
        { WaitForNavigating(el, 1); }

        public static void WaitForNavigating(EventListener el, int count)
        { el.Block("Navigating", TimeSpan.FromSeconds(PAGE_LOAD_TIMEOUT_SECONDS), count, true); }

        public static void WaitForNavigated(EventListener el)
        {WaitForNavigated(el, 1);}
        
        public static void WaitForNavigated(EventListener el, int count)
        { el.Block("Navigated", TimeSpan.FromSeconds(PAGE_LOAD_TIMEOUT_SECONDS), count, true); }

        public static void Spin()
        {
            Application.DoEvents();
            System.Threading.Thread.Sleep(100);
            Application.DoEvents();
        }
    }
    public struct WebPage
    {
        private const string SERVER_NAME = "aspnet-testweb";
        public static string ServerName
        { get { return SERVER_NAME; } }
        private const string SERVER_FQDN = "aspnet-testweb.redmond.corp.microsoft.com";
        public static string ServerNameFullyQualified
        { get { return SERVER_FQDN; } }
        private WebPage(string path)
        { this._Path = path; }
        private string _Path;

        public string HttpsPath
        { get { return "https://" + SERVER_NAME + "/htmldomtests/" + _Path; } }
        public string HttpPath
        { get { return "http://" + SERVER_NAME + "/htmldomtests/" + _Path; } }
        public string UncPath
        { get { return @"\\" + SERVER_NAME + @"\htmldomtests\" + _Path.Replace('/', '\\'); } }

        public string HttpsPathFullyQualified
        { get { return "https://" + SERVER_FQDN + "/htmldomtests/" + _Path; } }
        public string HttpPathFullyQualified
        { get { return "http://" + SERVER_FQDN + "/htmldomtests/" + _Path; } }
        public string UncPathFullyQualified
        { get { return @"\\" + SERVER_FQDN + @"\htmldomtests\" + _Path.Replace('/', '\\'); } }


        public static readonly WebPage Root = new WebPage("");

        public static readonly WebPage TestDoc1 = new WebPage("TestDoc1.html");
        public static readonly WebPage TestDoc2 = new WebPage("TestDoc2.html");
        public static readonly WebPage TestDoc3 = new WebPage("TestDoc3.html");
        public static readonly WebPage TestDoc4 = new WebPage("TestDoc4.html");
        public static readonly WebPage TestDoc5 = new WebPage("TestDoc5.html");

        public static readonly WebPage SlowWebPage = new WebPage("SlowPage.aspx");
        public static readonly WebPage PostDataEcho = new WebPage("PostDataEcho.aspx");
        public static readonly WebPage JpgImage = new WebPage("Bluehills.jpg");

        public static readonly WebPage RtlDoc = new WebPage("RTL.html");
        public static readonly WebPage MetaRefresh = new WebPage("refresh.html");

        public static readonly WebPage ExternalDom = new WebPage("controlInBrowser/ExternalDom.htm");

        public static readonly WebPage DomTest = new WebPage("DOMTest.html");

        public static readonly WebPage FramesetWebPage = new WebPage("testFrameset.html");

        public static readonly WebPage TestEnable = new WebPage("testEnable.htm");
    }
}

