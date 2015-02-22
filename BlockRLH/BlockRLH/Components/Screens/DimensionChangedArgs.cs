using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sigma.User_Interface.Components
{
    public class DimensionChangedArgs : EventArgs
    {
        public int NewWidth { get; set; }
        public int NewHeight { get; set; }
        public int OldHeight { get; set; }
        public int OldWidth { get; set; }

        public DimensionChangedArgs(int newWidth, int newHeight, int oldWidth, int oldHeight)
        {
            this.NewHeight = newHeight;
            this.NewWidth = newWidth;

            this.OldHeight = oldHeight;
            this.OldWidth = OldWidth;
        }
    }
}
