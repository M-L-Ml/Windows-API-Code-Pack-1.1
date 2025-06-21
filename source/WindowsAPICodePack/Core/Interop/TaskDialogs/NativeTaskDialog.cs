// Copyright (c) Microsoft Corporation.  All rights reserved.

using Microsoft.WindowsAPICodePack.Resources;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;

namespace Microsoft.WindowsAPICodePack.Dialogs
{
    internal class NativeTaskDialog : Form
    {
        private readonly TaskDialog outerDialog;
        private readonly NativeTaskDialogSettings settings;

        private TableLayoutPanel tableLayoutPanel;
        private Label instructionLabel;
        private Label contentLabel;
        private PictureBox mainIconPictureBox;
        private FlowLayoutPanel buttonFlowLayoutPanel;
        private Label footerLabel;
        private PictureBox footerIconPictureBox;
        private TextBox detailsTextBox;
        private LinkLabel detailsExpander;
        private CheckBox verificationCheckBox;
        private ProgressBar progressBar;

        public bool CheckBoxChecked { get; private set; }
        public int SelectedButtonId { get; private set; }
        public int SelectedRadioButtonId { get; private set; }
        public DialogShowState ShowState { get; private set; }

        internal NativeTaskDialog(NativeTaskDialogSettings settings, TaskDialog outerDialog)
        {
            this.settings = settings;
            this.outerDialog = outerDialog;

            ShowState = DialogShowState.PreShow;

            InitializeComponent();

            this.Text = settings.NativeConfiguration.windowTitle;
            UpdateInstruction(settings.NativeConfiguration.mainInstruction);
            UpdateText(settings.NativeConfiguration.content);
            UpdateFooterText(settings.NativeConfiguration.footerText);
            UpdateExpandedText(settings.NativeConfiguration.expandedInformation);
            UpdateMainIcon((TaskDialogStandardIcon)settings.NativeConfiguration.mainIcon.MainIcon);
            UpdateFooterIcon((TaskDialogStandardIcon)settings.NativeConfiguration.footerIcon.MainIcon);

            if (settings.NativeConfiguration.verificationText != null)
            {
                UpdateCheckBoxChecked(true);
                verificationCheckBox.Text = settings.NativeConfiguration.verificationText;
            }
            else
            {
                UpdateCheckBoxChecked(false);
            }

            AddButtons();
        }

        internal void NativeShow()
        {
            ShowState = DialogShowState.Showing;
            outerDialog.RaiseOpenedEvent();

            DialogResult result = this.ShowDialog(settings.NativeConfiguration.parentHandle == IntPtr.Zero ? null : new Win32Window(settings.NativeConfiguration.parentHandle));

            ShowState = DialogShowState.Closed;
            this.CheckBoxChecked = this.verificationCheckBox.Checked;
            this.SelectedRadioButtonId = 0; // Not implemented
        }

        internal void NativeClose(TaskDialogResult result)
        {
            this.DialogResult = ToDialogResult(result);
            this.Close();
        }

        internal void AssertCurrentlyShowing() => Debug.Assert(ShowState == DialogShowState.Showing, "Update*() methods should only be called while native dialog is showing");

        #region Update Methods

        internal void UpdateText(string text) => UpdateLabel(contentLabel, text);
        internal void UpdateInstruction(string instruction) => UpdateLabel(instructionLabel, instruction);
        internal void UpdateFooterText(string footerText) => UpdateLabel(footerLabel, footerText);
        internal void UpdateExpandedText(string expandedText) => UpdateTextBox(detailsTextBox, expandedText);

        internal void UpdateMainIcon(TaskDialogStandardIcon mainIcon) => UpdateIcon(mainIconPictureBox, mainIcon);
        internal void UpdateFooterIcon(TaskDialogStandardIcon footerIcon) => UpdateIcon(footerIconPictureBox, footerIcon);

        internal void UpdateCheckBoxChecked(bool cbc) { if (InvokeRequired) Invoke(new Action(() => verificationCheckBox.Visible = cbc)); else verificationCheckBox.Visible = cbc; }

        internal void UpdateProgressBarValue(int i) { if (InvokeRequired) Invoke(new Action(() => progressBar.Value = i)); else progressBar.Value = i; }
        internal void UpdateProgressBarState(TaskDialogProgressBarState state) { if (InvokeRequired) Invoke(new Action(() => progressBar.Style = state == TaskDialogProgressBarState.Marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous)); else progressBar.Style = state == TaskDialogProgressBarState.Marquee ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous; }
        internal void UpdateProgressBarRange(int? min = null, int? max = null) { if (InvokeRequired) Invoke(new Action(() => { if (min.HasValue) progressBar.Minimum = min.Value; if (max.HasValue) progressBar.Maximum = max.Value; })); else { if (min.HasValue) progressBar.Minimum = min.Value; if (max.HasValue) progressBar.Maximum = max.Value; } }

        internal void UpdateButtonEnabled(int buttonID, bool enabled) { UpdateButtonState(buttonID, enabled); }
        internal void UpdateRadioButtonEnabled(int buttonID, bool enabled) { UpdateButtonState(buttonID, enabled); }
        internal void UpdateElevationIcon(int buttonId, bool showIcon) { /* Not supported in WinForms */ }

        #endregion

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(484, 321);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "WinFormsTaskDialog";
            this.FormClosing += WinFormsTaskDialog_FormClosing;

            tableLayoutPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6 };
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60));
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            mainIconPictureBox = new PictureBox { Size = new Size(48, 48), SizeMode = PictureBoxSizeMode.StretchImage };
            instructionLabel = new Label { AutoSize = true, Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold) };
            contentLabel = new Label { AutoSize = true, MaximumSize = new Size(400, 0) };
            detailsExpander = new LinkLabel { AutoSize = true, Text = "Show details" };
            detailsTextBox = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true, Dock = DockStyle.Fill, Visible = false };
            progressBar = new ProgressBar { Dock = DockStyle.Fill };
            verificationCheckBox = new CheckBox { AutoSize = true };

            //tableLayoutPanel.Controls.Add(mainIconPictureBox, 0, 0);
            //tableLayoutPanel.SetRowSpan(mainIconPictureBox, 2);
            //tableLayoutPanel.Controls.Add(instructionLabel, 1, 0);
            //tableLayoutPanel.Controls.Add(contentLabel, 1, 1);
            //tableLayoutPanel.Controls.Add(detailsExpander, 1, 2);
            //tableLayoutPanel.Controls.Add(detailsTextBox, 1, 3);
            //tableLayoutPanel.Controls.Add(progressBar, 1, 4);
            //tableLayoutPanel.Controls.Add(verificationCheckBox, 1, 5);

            detailsExpander.LinkClicked += (s, e) =>
            {
                detailsTextBox.Visible = !detailsTextBox.Visible;
                detailsExpander.Text = detailsTextBox.Visible ? "Hide details" : "Show details";
            };

            var footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 30, BackColor = SystemColors.ControlLight };
            footerIconPictureBox = new PictureBox { Size = new Size(16, 16), Location = new Point(10, 7) };
            footerLabel = new Label { AutoSize = true, Location = new Point(35, 9) };
            footerPanel.Controls.Add(footerIconPictureBox);
            footerPanel.Controls.Add(footerLabel);

            buttonFlowLayoutPanel = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10), Height = 50 };

            this.Controls.Add(tableLayoutPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(buttonFlowLayoutPanel);
            this.ResumeLayout(false);
        }

        private void WinFormsTaskDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (outerDialog.RaiseClosingEvent(this.SelectedButtonId) != 0)
            {
                e.Cancel = true;
            }
        }

        private void AddButtons()
        {
            if (settings.Buttons != null)
            {
                foreach (var button in settings.Buttons)
                {
                    var newButton = new Button { Text = button.buttonText, Tag = button.buttonId };
                    newButton.Click += (s, e) =>
                    {
                        this.SelectedButtonId = button.buttonId;
                        outerDialog.RaiseButtonClickEvent(button.buttonId);
                        // if () { return; }
                        this.DialogResult = DialogResult.OK;
                    };
                    buttonFlowLayoutPanel.Controls.Add(newButton);
                }
            }

            List<TaskDialogButtonBase> taskDialogButtonBases = this.Controls.OfType<TaskDialogButtonBase>().ToList();
            ref var commonButtons = ref settings.NativeConfiguration.commonButtons;
            foreach (var button in taskDialogButtonBases)
            {
                //  if (buttonT is TaskDialogButtonBB taskDialogButton)
                //{
                //    taskDialogButton.HostingDialog = this.outerDialog;
                //    if (taskDialogButton.Default) taskDialogButton.Default = true; // Set as default buttonT if specified
                //    if (taskDialogButton.Enabled) taskDialogButton.Enabled = true; // Set enabled state
                //    AddButton(taskDialogButton.Text, DialogResult.OK, (int)taskDialogButton.Id);
                //}
                // var dd = new System.Windows.Forms.TaskDialogButton();
                var defaultButton = DefaultButtonType.ButtonMappings.FirstOrDefault(b => b.Text == button.Name || b.Text == button.Text);
                if (defaultButton.ButtonType != 0)
                {
                    AddButton(defaultButton);
                }
            }



            foreach (var b in DefaultButtonType.ButtonMappings)
            {
                var (flag, text, result, returnId, _) = b;
                if (commonButtons.HasFlag(flag))
                {
                    AddButton(b);
                }
            }


            taskDialogButtonBases.ForEach(button =>
            {
                AddButtonBase(button);
            });
        }

        //  private void AddButton
        private void AddButton(DefaultButtonType buttonT)
        {
            (string text, DialogResult dialogResult, int buttonId) =
                (buttonT.Text, buttonT.Result, (int)buttonT.ReturnId);

            var button = new Button { Text = text, DialogResult = dialogResult, Tag = buttonId };
            button.Click += (s, e) => { this.SelectedButtonId = buttonId; };
            buttonFlowLayoutPanel.Controls.Add(button);


        }
        private void AddButtonBase(TaskDialogButtonBase button)
        {
            button.HostingDialog = this.outerDialog;
            var buttonControl = new Button { Text = button.Text, Tag = button.Id };
            buttonFlowLayoutPanel.Controls.Add(buttonControl);
            buttonControl.Click += (s, e) =>
            {
                //button.  Click?.Invoke(s,e);
                this.SelectedButtonId = (int)button.Id;
                outerDialog.RaiseButtonClickEvent(this.SelectedButtonId);
                this.DialogResult = DialogResult.OK;
            };

        }
        private void UpdateIcon(PictureBox pb, TaskDialogStandardIcon iconEnum)
        {
            Icon icon = null;
            switch (iconEnum)
            {
                case TaskDialogStandardIcon.Warning: icon = SystemIcons.Warning; break;
                case TaskDialogStandardIcon.Error: icon = SystemIcons.Error; break;
                case TaskDialogStandardIcon.Information: icon = SystemIcons.Information; break;
                case TaskDialogStandardIcon.Shield: icon = SystemIcons.Shield; break;
            }
            if (InvokeRequired) Invoke(new Action(() => pb.Image = icon?.ToBitmap())); else pb.Image = icon?.ToBitmap();
        }

        private void UpdateLabel(Label label, string text) { if (InvokeRequired) Invoke(new Action(() => label.Text = text)); else label.Text = text; }
        private void UpdateTextBox(TextBox tb, string text) { if (InvokeRequired) Invoke(new Action(() => tb.Text = text)); else tb.Text = text; }
        private void UpdateButtonState(int buttonID, bool enabled) { foreach (Control c in buttonFlowLayoutPanel.Controls) if (c is Button b && (int)b.Tag == buttonID) { if (InvokeRequired) Invoke(new Action(() => b.Enabled = enabled)); else b.Enabled = enabled; break; } }

        private DialogResult ToDialogResult(TaskDialogResult result)
        {
            switch (result) { case TaskDialogResult.Ok: return DialogResult.OK; case TaskDialogResult.Cancel: return DialogResult.Cancel; case TaskDialogResult.Yes: return DialogResult.Yes; case TaskDialogResult.No: return DialogResult.No; case TaskDialogResult.Retry: return DialogResult.Retry; case TaskDialogResult.Close: return DialogResult.Cancel; default: return DialogResult.None; }
        }

        private class Win32Window : IWin32Window { public IntPtr Handle { get; private set; } public Win32Window(IntPtr handle) { Handle = handle; } }
    }

    internal record struct DefaultButtonType(TaskDialogNativeMethods.TaskDialogCommonButtons ButtonType, string Text, DialogResult Result,
        TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds ReturnId,
        TaskDialogButton ButtonObject
        )
    {
        [return: MaybeNull]
        public static DefaultButtonType? GetStandartButtonInfoOrNull(TaskDialogButtonBase buttonBase)
        {
            var res = DefaultButtonType.ButtonMappings.FirstOrDefault(bt => bt.ButtonObject == buttonBase);
            if (res == default)
            {
                return null;
            }
            return res;
        }

        //public static implicit operator (TaskDialogNativeMethods.TaskDialogCommonButtons ButtonType, string Text, DialogResult Result, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds ReturnId)(DefaultButtonType value)
        //{
        //    return (value.ButtonType, value.Text, value.Result, value.ReturnId);
        //}

        //public static implicit operator DefaultButtonType((TaskDialogNativeMethods.TaskDialogCommonButtons ButtonType, string Text, DialogResult Result, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds ReturnId) value)
        //{
        //    return new DefaultButtonType(value.ButtonType, value.Text, value.Result, value.ReturnId);
        //}

        public TaskDialogStandardButtons TaskDialogStandardButton
        {
            get
            {
                return TaskDialog.MapButtonIdToStandardButton((int)ReturnId);
            }
        }

        public static readonly DefaultButtonType[] ButtonMappings =
            {
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.Ok, "OK", DialogResult.OK, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Ok

                    ,TaskDialogButton.OK),
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.Yes, "Yes", DialogResult.Yes, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Yes,TaskDialogButton.Yes),
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.No, "No", DialogResult.No, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.No, TaskDialogButton.No),
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.Cancel, "Cancel", DialogResult.Cancel, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Cancel, TaskDialogButton.Cancel),
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.Retry, "Retry", DialogResult.Retry, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Retry, TaskDialogButton.Retry),
                new(TaskDialogNativeMethods.TaskDialogCommonButtons.Close, "Close", DialogResult.Cancel, TaskDialogNativeMethods.TaskDialogCommonButtonReturnIds.Close, TaskDialogButton.Close)
            };
    }
}
