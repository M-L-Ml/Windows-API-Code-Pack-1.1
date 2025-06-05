using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if !WINDOWS_OWN

/// as Base class

namespace System.Windows.Forms
{
    using TaskDialogButtonBB = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton;
    /// <summary>
    /// it's a stub
    /// </summary>
//#if !FULLAPI
    // Stub implementation
    public class TaskDialogPage
    {
        public string Text { get; set; }
        public string Caption { get; set; }
        public string Heading { get; set; }
        public object Icon { get; set; }
        public TaskDialogVerificationCheckBox Verification { get; set; }
        public bool SizeToContent { get; set; }

        /// <summary>
        /// TaskDialogButton
        /// </summary>
        public Collection<TaskDialogButtonBB> Buttons { get; set; } = new();
        public bool AllowCancel { get; set; }
        public TaskDialogButton DefaultButton { get; set; }
        public string Footnote { get; set; }
    }
       // see also Microsoft.WindowsAPICodePack.Dialogs.TaskDialog
    public static class TaskDialog
    {
        public static TaskDialogButtonBB ShowDialog(nint handle, TaskDialogPage page)
        {
            throw new NotImplementedException();
        }

        public static TaskDialogButtonBB ShowDialog(IWin32Window owner, TaskDialogPage page)
        {
            throw new NotImplementedException();
        }

        public static TaskDialogButtonBB ShowDialog(TaskDialogPage page)
        {
            throw new NotImplementedException();
        }
    }

    public class TaskDialogIcon// : IDisposable
    {
        public static readonly TaskDialogIcon Warning = new();

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of white X in a circle with a red background.
        /// </summary>
        public static readonly TaskDialogIcon Error = new();
        public static readonly TaskDialogIcon Information = new();
    }

    //    new TaskDialogVerificationCheckBox
    //                {
    //                    Text = Instance._rememberChoice.Text
    //},


    /// <summary>
    /// stub
    /// </summary>
    public class TaskDialogVerificationCheckBox //: TaskDialogButtonBB
    {
        public bool Checked { get; set; }
        public string Text { get; set; }
    }
    public class TaskDialogButton: TaskDialogButtonBB
    {
       // private string _text;

        public TaskDialogButton(string name):base(name, name)
        {
           // _text = text;
        }

        public static readonly TaskDialogButton Cancel = new TaskDialogButton("Cancel");
        public static readonly TaskDialogButton Yes = new TaskDialogButton("Yes");
        public static readonly TaskDialogButton No = new TaskDialogButton("No");
        public static readonly TaskDialogButton OK = new TaskDialogButton("OK");
    }
    public sealed class TaskDialogCommandLinkButton :Microsoft.WindowsAPICodePack.Dialogs.TaskDialogCommandLink
    {
        public TaskDialogCommandLinkButton(string name, bool enabled = true) : base(name, "")
        {
            Enabled = enabled;
        }
    }
}
#endif
