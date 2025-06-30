using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
/// <summary>
/// Fixing non displayed Button text in Linux : workaround 
/// </summary>
namespace System.Windows.Forms
{
    public class Form2 : Form
    {
        public Form2()
        {
        }
        //   public IContainer ComponentsContainer => ;
        protected override void OnHandleCreated(EventArgs e)
        {
            List<ToolTip> toolTips;
            var toolTip = (ToolTip?)GetService(typeof(ToolTip));
            if (toolTip != null)
            {
                //seems almost never
                toolTips = new List<ToolTip> { toolTip };
            }
            else
                toolTips = this.GetToolTipComponents().ToList();

            this.AddTooltipsAndIconsToButtons(toolTip: toolTips.FirstOrDefault());
            base.OnHandleCreated(e);

        }
    }


    public static class ControlExtensions2
    {
        /// <summary>
        /// Fixing non displayed Button text in Linux : workaround 
        /// </summary>
        public static int AddTooltipsAndIconsToButtons(this Control parent, Image? image = null, ToolTip? toolTip = null)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                // this bug is not on Windows
                return 0;
            }

            var command = new AddTooltipsAndIconsToButtonsCommand() { Image = image, ToolTip = toolTip };
            return TraverseControls(parent, command);
        }

        public static IEnumerable<ToolTip> GetToolTipComponents(this Control frm)
        {
            IContainer? parent = GetComponentsContainer(frm);

            var toolTips = parent?.Components.OfType<ToolTip>();
            return toolTips ?? Enumerable.Empty<ToolTip>();
        }

        public static IContainer? GetComponentsContainer(this Control frm)
        {
            if (frm == null)
            {
                throw new ArgumentNullException(nameof(frm), "Control cannot be null.");
            }
            var r = frm.Container;
            if (r != null)
            {
                //never happens, but just in case
                return r;
            }


            Type typeForm = frm.GetType();
            // "components" is private field in Form , default name used by Form designer tools, it's private so we need to use reflection
            FieldInfo fieldInfo = typeForm.GetField("components", BindingFlags.Instance | BindingFlags.NonPublic);
            IContainer? parent = (IContainer?)fieldInfo?.GetValue(frm);
            return parent;
        }

        public interface ITraverseCommand
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="control"></param>
            /// <returns>control was changed</returns>
            bool Execute(Control control);
            void InitForParent(Control parent);
        }

        /// <summary>
        /// 
        /// </summary>
        public class AddTooltipsAndIconsToButtonsCommand : ITraverseCommand
        {
            public Image? Image { get; init; }
            public ToolTip? ToolTip { get; set; }

            /// <summary>
            /// on WSL Ubuntu labels display text correctly while buttons do not
            /// </summary>
            public static Label Label = new Label() { Text = "_" };
            //  public ToolTip? ToolTip { get; init; }
            public bool Execute(Control control)
            {
                if (control is not Button button)
                {
                    return false;
                }
                bool changed = false;
                string? existing = ToolTCreated ? null : ToolTip.GetToolTip(button);
                if (string.IsNullOrEmpty(existing))
                {
                    ToolTip.SetToolTip(button, $"{button.Text} .");

                    changed = true;
                    button.TextChanged += (o, e) =>
                    {
                        if (o is Button b)
                        {
                            ToolTip.SetToolTip(b, $"{b.Text} .");
                        }
                    };
                }
                if (button.Image == null)
                {
                    if (Image != null)
                    {
                        button.Image = Image;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        changed = true;
                    }
                    else
                    {
                        Label.Size = button.Size;
                        Label.Text = button.Text;
                        var im = Label.CaptureControlAsImage();
                        button.Image = im;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                        changed = true;
                    }
                }
                return changed;
            }
            bool ToolTCreated = false;

            public void InitForParent(Control parent)
            {
                var existing = parent.GetToolTipComponents().FirstOrDefault();
                if (existing == null && ToolTip == null)
                {
                    ToolTip = new ToolTip() { };
                    ToolTCreated = true;
                    SetToolTipFastTiming(ToolTip!);
                    return;
                }
                if (existing != null)
                {
                    SetToolTipFastTiming(existing);
                    ToolTip = existing;
                    //if we have a toolTip, we can set the timing
                }

                ToolTCreated = false;

            }
        }



        public static void SetToolTipFastTiming(this ToolTip toolTip)
        {
            toolTip.AutomaticDelay = 50;
            toolTip.AutoPopDelay = 4000;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="command"></param>
        /// <returns>affectedControls</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static int TraverseControls(this Control parent, ITraverseCommand command)
        {
            int affectedControls = 0;
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent), "Parent control cannot be null.");
            }
            command.InitForParent(parent);
            foreach (Control control in parent.Controls)
            {
                command.Execute(control);
                affectedControls++;
                if (control.Controls.Count > 0)
                {
                    affectedControls += TraverseControls(control, command);
                }
            }
            return affectedControls;
        }

        ///, bool shrink = false
        public static Image CaptureControlAsImage(this Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (control.Width <= 0 || control.Height <= 0)
                throw new ArgumentException("Control must have valid dimensions");

            try
            {
                // Create bitmap with control's dimensions
#pragma warning disable CA1416 // Validate platform compatibility
                Bitmap bitmap = new Bitmap(control.Width, control.Height);
#pragma warning restore CA1416 // Validate platform compatibility

                // Capture the control's visual appearance
                control.DrawToBitmap(bitmap, new Rectangle(0, 0, control.Width, control.Height));

                return bitmap;
            }
            catch (ArgumentException ex)
            {
                // Handle cases where bitmap might be too large for the system
                throw new InvalidOperationException($"Failed to capture control image: {ex.Message}", ex);
            }
        }
        public static Bitmap ControlScreenShot(this Control form, bool clientAreaOnly)
        {
            var fullSizeBitmap = new Bitmap(width: form.Width, form.Height, format : PixelFormat.Format32bppArgb);
            // .Net 4.7+
            // fullSizeBitmap.SetResolution(form.DeviceDpi, form.DeviceDpi);

            form.DrawToBitmap(fullSizeBitmap, new Rectangle(Point.Empty, form.Size));
            if (!clientAreaOnly) return fullSizeBitmap;

            Point p = form.PointToScreen(Point.Empty);
            var clientRect =
                new Rectangle(new Point(p.X - form.Bounds.X, p.Y - form.Bounds.Y), form.ClientSize);

            var clientAreaBitmap = fullSizeBitmap.Clone(clientRect, PixelFormat.Format32bppArgb);
            fullSizeBitmap.Dispose();
            return clientAreaBitmap;
        }
    }

}
