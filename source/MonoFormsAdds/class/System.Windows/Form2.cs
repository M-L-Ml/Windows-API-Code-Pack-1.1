using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        protected override void OnHandleCreated(EventArgs e)
        {
            this.AddTooltipsAndIconsToButtons();
            base.OnHandleCreated(e);
        }
    }

    public static class ControlExtensions2
    {
        public static int AddTooltipsAndIconsToButtons(this Control parent, Image? image = null, ToolTip? toolTip = null)
        {
            toolTip = toolTip ?? new ToolTip() { AutomaticDelay = 50, AutoPopDelay = 4000 };

            //image = image ?? Properties.Resources.Main; // Replace with your desired icon
            int numButtons = 0;


            foreach (Control control in parent.Controls)
            {
                if (control is ButtonBase button)
                {
                    toolTip.SetToolTip(button, $" {button.Text}");
                    button.TextChanged += (o, e) =>
                    {

                        if (o is ButtonBase b)
                        {
                            toolTip.SetToolTip(b, $" {b.Text}");
                        }
                    };
                    if (image != null)
                        button.Image = image;
                    button.ImageAlign = ContentAlignment.MiddleLeft;
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
