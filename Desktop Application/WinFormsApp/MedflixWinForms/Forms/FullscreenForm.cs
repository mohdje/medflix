using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MedflixWinforms
{
    public partial class FullscreenForm : Form
    {
        public FullscreenForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override this to handle arrows key pressed, otherwize these are ignored by Form
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right || keyData == Keys.Left)
            {
                OnKeyDown(new KeyEventArgs(keyData));
                // Handle key at form level.
                // Do not send event to focused control by returning true.
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
