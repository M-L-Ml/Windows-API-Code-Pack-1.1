//Copyright (c) Microsoft Corporation.  All rights reserved.

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    /// <summary>Implements a button that can be hosted in a task dialog.</summary>
    public class TaskDialogButton : TaskDialogButtonBase
    {
        private bool useElevationIcon;

        /// <summary>Creates a new instance of this class.</summary>
        public TaskDialogButton() { }

        /// <summary>Creates a new instance of this class with the specified property settings.</summary>
        /// <param name="name">The name of the button.</param>
        /// <param name="text">The button label.</param>
        public TaskDialogButton(string name, string text) : base(name, text) { }

        /// <summary>Gets or sets a value that controls whether the elevation icon is displayed.</summary>
        public bool UseElevationIcon
        {
            get => useElevationIcon;
            set
            {
                CheckPropertyChangeAllowed("ShowElevationIcon");
                useElevationIcon = value;
                ApplyPropertyChange("ShowElevationIcon");
            }
        }


        public static readonly TaskDialogButton Cancel = new TaskDialogButton(){ Name = "Cancel" //, Id=2
        };
        public static readonly TaskDialogButton Yes = new TaskDialogButton { Name = "Yes" };
        public static readonly TaskDialogButton No = new TaskDialogButton{ Name = "No"};
        public static readonly TaskDialogButton OK = new TaskDialogButton{ Name = "OK"};
        public static readonly TaskDialogButton Close = new TaskDialogButton{ Name = "Close"};
        public static readonly TaskDialogButton Help = new TaskDialogButton{ Name = "Help"};
        public static readonly TaskDialogButton Retry = new TaskDialogButton{ Name = "Retry" };
    }
}
