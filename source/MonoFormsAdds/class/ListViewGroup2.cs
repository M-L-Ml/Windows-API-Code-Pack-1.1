
using System.Diagnostics;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
//used in GitUI.CommandsDialogs.BrowseDialog.DashboardControl

#if !WINDOWS_OWN 

namespace System.Windows.Forms
{

    public enum ListViewGroupCollapsedState
    {
        Default,
        Expanded,
        Collapsed
    }


    public static class ListViewGroupEx//: ListViewGroup
    {
        //private string _text;
        //private HorizontalAlignment _left;

        //public ListViewGroup2(string text, HorizontalAlignment left)
        //{
        //    _text = text;
        //    _left = left;
        //}

        //public string Name { get; set; }
        public static void CollapsedState(this ListViewGroup t, ListViewGroupCollapsedState _)
        {
            Debug.Assert(false, "TODO implement");

        }
        public static void TaskLink(this ListViewGroup t, string _)
        {

            Debug.Assert(false, "TODO implement");
        }
    }

    public static class Application2
    {

        /// <summary>
        /// TODO: Core.System.Windows.Forms Application not supports it yet
        /// Placeholder for actual implementation
        /// Stub for cross-platform compatibility, replace with actual implementation if needed.
        /// </summary>
        public static bool IsDarkModeEnabled => false;
    }





    public static class LinkClickedEventArgsExt
    {
        //e.LinkStart()
        public static int LinkStart(this LinkClickedEventArgs e)
        {
            throw new NotImplementedException("TODO implement");
        }

    }

}

namespace System.Windows
{
    /// <summary>
    /// it's a Stub for the WPF's class
    /// </summary>
    public class UIElement
    {
    }
}
#endif

