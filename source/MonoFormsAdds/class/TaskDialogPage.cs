using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !WINDOWS_OWN
namespace System.Windows.Forms
{
    /// <summary>
    /// it's a stub
    /// </summary>
    public class TaskDialogPage
    {
        public string Text { get; set; }
        public string Caption { get; set; }
        public string Heading { get; set; }
        public object Icon { get; set; }
        public object Verification { get; set; }
        public bool SizeToContent { get; set; }
    }
}
#endif
