using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        //public static void GetAllToolTips(this Control frm)
        //{
        //    IEnumerable<ToolTip> toolTips = frm.GetToolTipComponents();
        //    var toolTipList = toolTips.ToList();
        //    if (toolTipList.Count > 0)
        //    {
        //        ToolTip tt = toolTips[0];
        //        foreach (Control c in frm.Controls)
        //        {
        //            string text = tt.GetToolTip(c);

        //        }
        //    }

        //}

        public static IEnumerable<ToolTip> GetToolTipComponents(this Control frm)
        {
            IContainer parent = GetComponentsContainer(frm);

            var toolTips = parent.Components.OfType<ToolTip>();
            return toolTips;
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
                return r;
            }


            Type typeForm = frm.GetType();
            FieldInfo fieldInfo = typeForm.GetField("components", BindingFlags.Instance | BindingFlags.NonPublic);
            IContainer? parent = (IContainer?)fieldInfo.GetValue(frm);
            return parent;
        }

        public static int AddTooltipsAndIconsToButtons(this Control parent, Image? image = null, ToolTip? toolTip = null)
        {
            bool toolTCreated;
            if (toolTip == null)
            {
                toolTip = new ToolTip() { AutomaticDelay = 50, AutoPopDelay = 4000 };
                toolTCreated = true;
            }
            else
            {
                toolTip.AutomaticDelay = 50;
                toolTip.AutoPopDelay = 4000;
                toolTCreated = false;
            }
            //image = image ?? Properties.Resources.Main; // Replace with your desired icon
            int numButtons = 0;


            foreach (Control control in parent.Controls)
            {
                if (control is ButtonBase button)
                {

                    var existing = toolTCreated ? null : toolTip.GetToolTip(button);
                    if (string.IsNullOrEmpty(existing))
                    {

                        toolTip.SetToolTip(button, $"{button.Text} .");
                        button.TextChanged += (o, e) =>
                        {
                            if (o is ButtonBase b)
                            {
                                toolTip.SetToolTip(b, $"{b.Text} .");
                            }
                        };
                    }

                    if (image != null)
                    {
                        button.Image = image;
                        button.ImageAlign = ContentAlignment.MiddleLeft;
                    }
                    numButtons++;
                }
                else
                {
                    numButtons += AddTooltipsAndIconsToButtons(control, image, toolTip); // Recursively check child controls
                }
            }
            return numButtons;
        }
    }
}
