using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;
using MsgPackVisualizer.Properties;

namespace MsgPackVisualizer
{
    public partial class MsgPackBufferViewer : Form
    {
        public MsgPackBufferViewer()
        {
            InitializeComponent();
        }

        public void SetObjectProvider(IVisualizerObjectProvider objectProvider)
        {
            var bytes = new MemoryStream();
            using (var output = objectProvider.GetData())
                output.CopyTo(bytes);

            richTextBox.Text = MessagePackFormatter.ToJsonString(bytes.ToArray());

            ActiveControl = richTextBox;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // allow ESC to close
            if (e.KeyCode == Keys.Escape)
                Close();
            base.OnKeyUp(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            var s = Settings.Default;

            Width = s.WindowWidth;
            Height = s.WindowHeight;
            Left = s.WindowLeft;
            Top = s.WindowTop;

            base.OnLoad(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var s = Settings.Default;

            s.WindowWidth = Width;
            s.WindowHeight = Height;
            s.WindowLeft = Left;
            s.WindowTop = Top;

            s.Save();

            base.OnClosing(e);
        }
    }
}