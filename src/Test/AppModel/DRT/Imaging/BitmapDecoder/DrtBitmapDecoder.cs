using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DRT
{
    public class DrtBitmapDecoder
    {
        static int Main(string[] args)
        {
            var logger = new Logger("DrtBitmapDecoder", "Microsoft",
                "Testing decode the image file that created with Asynchronous");

            var imageFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Image.png");

            using var fileStream = new FileStream(imageFile,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                4096,
                FileOptions.Asynchronous);

            var passed = false;

            try
            {
                // See https://github.com/dotnet/wpf/issues/4355
                _ = BitmapDecoder.Create(fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                passed = true;
            }
            catch (ArgumentException)
            {
                passed = false;
            }

            if (passed)
            {
                logger.Log("Passed");
                return 0;
            }
            else
            {
                logger.Log("ERROR: - DrtBitmapDecoder. Can not decode the async file stream.");
                return 1;
            }
        }
    }
}
