using DinkToPdf;
using DinkToPdf.Contracts;
using System.Runtime.Loader;

namespace AMRent.Shared.Providers
{
    public static class PdfConverter
    {
        private static bool _initialized = false;
        private static readonly Lazy<IConverter> _converter = new(() =>
            new SynchronizedConverter(new PdfTools())
        );

        public static IConverter Instance => _converter.Value;
        private static readonly object _lock = new();

        public static void EnsureInitialized()
        {
            if (_initialized) return;

            lock (_lock)
            {
                if (_initialized) return;

                var context = new CustomAssemblyLoadContext();
                context.LoadUnmanagedLibrary(Path.Combine(AppContext.BaseDirectory, "Libraries", "DinkToPdf", "libwkhtmltox.dll"));
                _initialized = true;
            }
        }
    }

    public class Pdf
    {
        private readonly IConverter _converter;

        public Pdf()
        {
            PdfConverter.EnsureInitialized();
            _converter = PdfConverter.Instance;
        }

        public byte[] ConvertHtmlToPdf(string htmlContent)
        {
            try
            {
                var doc = new HtmlToPdfDocument
                {
                    GlobalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4
                    },
                    Objects = {
                        new ObjectSettings
                        {
                            HtmlContent = htmlContent,
                            WebSettings = { DefaultEncoding = "utf-8" }
                        }
                    }
                };
                return _converter.Convert(doc);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
    public class CustomAssemblyLoadContext : AssemblyLoadContext
    {
        public IntPtr LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllPath)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllPath);
        }
    }
}