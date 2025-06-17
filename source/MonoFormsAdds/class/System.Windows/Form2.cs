using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
            IContainer? parent = (IContainer?)fieldInfo.GetValue(frm);
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

        public class AddTooltipsAndIconsToButtonsCommand : ITraverseCommand
        {
            public Image? Image { get; init; }
            public ToolTip? ToolTip { get; set; }
            //  public ToolTip? ToolTip { get; init; }
            public bool Execute(Control control)
            {
                if (control is not ButtonBase button)
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
                        if (o is ButtonBase b)
                        {
                            ToolTip.SetToolTip(b, $"{b.Text} .");
                        }
                    };
                }

                if (Image != null)
                {
                    button.Image = Image;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
                    changed = true;
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


        public static int AddTooltipsAndIconsToButtons(this Control parent, Image? image = null, ToolTip? toolTip = null)
        {


            var command = new AddTooltipsAndIconsToButtonsCommand() { Image = image, ToolTip = toolTip };
            return TraverseControls(parent, command);
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
    }
}
