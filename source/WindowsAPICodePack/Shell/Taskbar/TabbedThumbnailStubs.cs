#if N0
//!FULLAPI
using System;
namespace Microsoft.WindowsAPICodePack.Taskbar
{
    internal class TabbedThumbnailProxyWindow { public TabbedThumbnailProxyWindow(object o) { } public IntPtr WindowToTellTaskbarAbout => IntPtr.Zero; public string Text { get; set; } public void Dispose() { } }
    internal class ThumbnailToolbarProxyWindow { public ThumbnailToolbarProxyWindow(object o, ThumbnailToolBarButton[] b) { } public IntPtr WindowToTellTaskbarAbout => IntPtr.Zero; public void Dispose() { } public TaskbarWindow TaskbarWindow { get; set; } }
    internal class TabbedThumbnail : IDisposable { public IntPtr WindowHandle => IntPtr.Zero; public object WindowsControl => null; public TaskbarWindow TaskbarWindow { get; set; } public void Dispose() { } }
}
#endif
