using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

public class IEBrowser : ApplicationContext
{
    string File;
    string Html;
    AutoResetEvent ResultEvent;

    public IEBrowser(string html,string file, AutoResetEvent resultEvent)
    {
        ResultEvent = resultEvent;
        Thread thrd = new Thread(new ThreadStart(
            delegate {
                Init(html, file);
                System.Windows.Forms.Application.Run(this);
            })); 
        // set thread to STA state before starting
        thrd.SetApartmentState(ApartmentState.STA);
        thrd.Start(); 
    }

    private void Init(string html, string file)
    {
        // create a WebBrowser control
        WebBrowser ieBrowser = new WebBrowser();
        ieBrowser.ScrollBarsEnabled = false;
        ieBrowser.ScriptErrorsSuppressed = true;
        
        // set WebBrowser event handle
        ieBrowser.DocumentCompleted += IEBrowser_DocumentCompleted;

        ieBrowser.Navigate("about:blank");
        Html = html;
        File = file;

    }

    // DocumentCompleted event handle
    void IEBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
    {
        try
        {
            WebBrowser browser = (WebBrowser)sender;
            HtmlDocument doc = browser.Document;
            doc.OpenNew(true);
            doc.Write(Html);
            Thread.Sleep(1000);
            browser.Width = doc.Body.ScrollRectangle.Width;
            browser.Height = doc.Body.ScrollRectangle.Height;

            Bitmap bitmap = new Bitmap(browser.Width, browser.Height);
            GetImage(browser.ActiveXInstance, bitmap, Color.White);

            browser.Dispose();
            bitmap.Save(File, ImageFormat.Jpeg);
        }
        catch (System.Exception)
        {
        }
        finally
        {
            ResultEvent.Set();
        }

    }

    [ComImport]
    [Guid("0000010D-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IViewObject
    {
        void Draw([MarshalAs(UnmanagedType.U4)] uint dwAspect, int lindex, IntPtr pvAspect, [In] IntPtr ptd, IntPtr hdcTargetDev, IntPtr hdcDraw, [MarshalAs(UnmanagedType.Struct)] ref RECT lprcBounds, [In] IntPtr lprcWBounds, IntPtr pfnContinue, [MarshalAs(UnmanagedType.U4)] uint dwContinue);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    public static void GetImage(object obj, Image destination, Color backgroundColor)
    {
        using (Graphics graphics = Graphics.FromImage(destination))
        {
            IntPtr deviceContextHandle = IntPtr.Zero;
            RECT rectangle = new RECT();

            rectangle.Right = destination.Width;
            rectangle.Bottom = destination.Height;

            graphics.Clear(backgroundColor);

            try
            {
                deviceContextHandle = graphics.GetHdc();

                IViewObject viewObject = obj as IViewObject;
                viewObject.Draw(1, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, deviceContextHandle, ref rectangle, IntPtr.Zero, IntPtr.Zero, 0);
            }
            finally
            {
                if (deviceContextHandle != IntPtr.Zero)
                {
                    graphics.ReleaseHdc(deviceContextHandle);
                }
            }
        }
    }
}