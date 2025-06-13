using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#if !WINDOWS_OWN


namespace System.Windows.Forms
{

    /// as Base class
    using TaskDialogButtonBB = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton;
    public static class TaskDialogButtonBBExtensions
    {
        public static void PerformClick(this TaskDialogButtonBB b)
        {
            throw new NotImplementedException("TODO implement");
        }
    }
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
        public event EventHandler? Created;
    }
    public sealed class LinkClicked2EventArgs : LinkClickedEventArgs
    {
        public LinkClicked2EventArgs(string linkText, int linkStart, int linkLength) : base(linkText)
        {
            LinkStart = linkStart;
            LinkLength = linkLength;
        }

        public int LinkStart { get; }
        public int LinkLength { get; }
    }
    /// <summary>
    /// This is still stub not tested at all.
    /// see also Microsoft.WindowsAPICodePack.Dialogs.TaskDialog
    /// </summary>
    public static class TaskDialog
    {
        public static TaskDialogButtonBB ShowDialog(nint handle, TaskDialogPage page)
        {

            return ShowDialog(page);
        }

        public static TaskDialogButtonBB ShowDialog(IWin32Window owner, TaskDialogPage page)
        {
            return ShowDialog(page);
        }

        public static TaskDialogButtonBB ShowDialog(TaskDialogPage page)
        {
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialog d = new();
            foreach (var b in page.Buttons)
            {
                d.Controls.Add(b);
            }
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult taskDialogResult;
            if (Environment.OSVersion.Platform  == PlatformID.Win32NT)
            {
                taskDialogResult = d.Show();
            }
            else
            {
                Debug.Assert(false);
                throw new NotImplementedException(" not implemented ");
               // TODO: use derived from System.Windows.Forms.CommonDialog  Forms.Form  f = new();
                // taskDialogResult = d.Show(owner: null);
            }


            var result = taskDialogResult;

            //TODO:  find button with result;
            return (TaskDialogButtonBB)(d.Controls.FirstOrDefault(b => (b as TaskDialogButton)?.Text == result.ToString()) ?? d.Controls.FirstOrDefault());
        }
    }

    public class TaskDialogIcon: //System.Drawing.Icon : IDisposable
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
    public class TaskDialogButton : TaskDialogButtonBB
    {
        public static string MakeNonEmpty(string? t) => string.IsNullOrWhiteSpace(t) ? "<SomeDefaultText>" : t;
        public TaskDialogButton(string name) : base(MakeNonEmpty(name), MakeNonEmpty(name))
        {
        }

        public static readonly TaskDialogButton Cancel = new TaskDialogButton("Cancel");
        public static readonly TaskDialogButton Yes = new TaskDialogButton("Yes");
        public static readonly TaskDialogButton No = new TaskDialogButton("No");
        public static readonly TaskDialogButton OK = new TaskDialogButton("OK");
    }
    public sealed class TaskDialogCommandLinkButton : Microsoft.WindowsAPICodePack.Dialogs.TaskDialogCommandLink
    {
        public TaskDialogCommandLinkButton(string name, bool enabled = true)
            : base(name, TaskDialogButton.MakeNonEmpty(name))
        {
            Enabled = enabled;
        }

        public TaskDialogCommandLinkButton(string name, string text) : base(name, text)
        {
            Enabled = true;
        }
    }
}
#endif
