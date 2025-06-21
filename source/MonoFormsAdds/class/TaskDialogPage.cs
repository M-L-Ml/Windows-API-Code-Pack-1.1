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
    using Microsoft.WindowsAPICodePack.Dialogs;
    using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        public TaskDialogIcon Icon { get; set; }
        public TaskDialogVerificationCheckBox Verification { get; set; }
        public bool SizeToContent { get; set; }

        /// <summary>
        /// TaskDialogButton
        /// </summary>
        public Collection<TaskDialogButtonBB> Buttons { get; set; } = new();
        public bool AllowCancel { get; set; }
        public TaskDialogButtonBB DefaultButton { get; set; }
        public string Footnote { get; set; }
        public event EventHandler? Created;

        internal Microsoft.WindowsAPICodePack.Dialogs.TaskDialog CreateDialog()
        {
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialog r = new()
            {
                Text = Text,
                Icon = Icon.StandardIcon,
                Caption = Caption,
                FooterText = Footnote,
                InstructionText = Heading,
                // AllowCancel = page.AllowCancel,
                DefaultButtonObj = DefaultButton
            };
            //TODO: use https://www.nuget.org/packages/cmdwtf.Luminous.Windows.Forms https://github.com/cmdwtf/Luminous/blob/main/Luminous.Windows.Forms/TaskDialog/TaskDialogForm.cs
            // or https://github.com/gitextensions/PSTaskDialog
            /// "           This code is part of the T8SuitePro.
            //This code was written by matiasclaesson.
            //The original code is available here:
            //https://github.com/mattiasclaesson/T8SuitePro.git"
            // ==https://github.com/mattiasclaesson/TuningSuites/blob/master/PSTaskDialog/PSTaskDialog/frmTaskDialog.cs 
            // == https://github.com/Andrew414/dmark/blob/e5ae552d6c5df593bde280b8fe00967d2519f92d/C%23/PSTaskDialog/PSTaskDialog/frmTaskDialog.cs - what was used in the past days of Mono through an dll
            // == https://github.com/mRemoteNG/mRemoteNG/blob/v1.78.2-dev/mRemoteNG/UI/TaskDialog/frmTaskDialog.cs
            // ==https://github.com/lextudio/codebeautifiercollection/tree/master/thirdparties/PSTaskDialog

            return r;
        }
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
        public static TaskDialogButtonBase ShowDialog(nint handle, TaskDialogPage page)
        {

            return ShowDialog(page);
        }

        public static TaskDialogButtonBase ShowDialog(IWin32Window owner, TaskDialogPage page)
        {
            return ShowDialog(page);
        }

        public static Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButtonBase
            ShowDialog(TaskDialogPage page)
        {
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialog d = page.CreateDialog();
            foreach (var b in page.Buttons)
            {
                d.Controls.Add(b);
            }
            Microsoft.WindowsAPICodePack.Dialogs.TaskDialogResult taskDialogResult;
            if (Environment.OSVersion.Platform != PlatformID.Other)
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
            return d.LastFiredButton;
            //  find button with result;
            //return (TaskDialogButtonBB)(d.Controls.FirstOrDefault(b => (b as TaskDialogButton)?.Text == result.ToString()) ?? d.Controls.FirstOrDefault());
        }
    }

    public class TaskDialogIcon //System.Drawing.Icon : IDisposable
    {
        public static readonly TaskDialogIcon Warning = new(TaskDialogStandardIcon.Warning);

        /// <summary>
        ///   Gets a standard <see cref="TaskDialogIcon"/> instance where the task dialog
        ///   contains an icon consisting of white X in a circle with a red background.
        /// </summary>
        public static readonly TaskDialogIcon Error = new(TaskDialogStandardIcon.Error);
        public static readonly TaskDialogIcon Information = new(TaskDialogStandardIcon.Information);

        public TaskDialogIcon(TaskDialogStandardIcon icon)
        {
            StandardIcon = icon;
        }

        public TaskDialogStandardIcon StandardIcon { get; }
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
        public static new TaskDialogButtonBB Cancel => TaskDialogButtonBB.Cancel;
        public static new TaskDialogButtonBB Yes => TaskDialogButtonBB.Yes;
        public static new TaskDialogButtonBB No => TaskDialogButtonBB.No;
        public static new TaskDialogButtonBB OK => TaskDialogButtonBB.OK;
        public static new TaskDialogButtonBB Close => TaskDialogButtonBB.Close;
        public static new TaskDialogButtonBB Help => TaskDialogButtonBB.Help;
        //public static readonly TaskDialogButton Cancel = new TaskDialogButton("Cancel");
        //public static readonly TaskDialogButton Yes = new TaskDialogButton("Yes");
        //public static readonly TaskDialogButton No = new TaskDialogButton("No");
        //public static readonly TaskDialogButton OK = new TaskDialogButton("OK");
        //public static readonly TaskDialogButton Close = new TaskDialogButton("Close");
        //public static readonly TaskDialogButton Help = new TaskDialogButton("Help");
    }
    public sealed class TaskDialogCommandLinkButton : Microsoft.WindowsAPICodePack.Dialogs.TaskDialogCommandLink
    {
        public TaskDialogCommandLinkButton(string? text, string? descriptionText = default, bool enabled = true, bool allowCloseDialog = true)
            : base(TaskDialogButton.MakeNonEmpty(text), text: text)
        {
            DescriptionText = descriptionText;
            Enabled = enabled;
        }



        public string DescriptionText { get; set; }
    }
}
#endif
