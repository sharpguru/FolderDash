using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderDash.Windows
{
    public partial class InputBox : Form
    {
        public string Prompt
        {
            get
            {
                return lblPrompt.Text;
            }

            set
            {
                lblPrompt.Text = value;
            }
        }

        public string Title
        {
            get
            {
                return Text;
            }

            set
            {
                Text = value;
            }
        }

        public string DefaultResponse
        {
            set
            {
                textBoxResponse.Text = value;
            }
        }

        public string Value
        {
            get
            {
                return textBoxResponse.Text;
            }
        }

        public InputBox()
        {
            InitializeComponent();
        }

        public InputBox(string prompt, string title, string defaultResponse)
        {
            Prompt = prompt;
            Title = title;
            DefaultResponse = defaultResponse;
        }
    }
}
