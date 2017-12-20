using System.Diagnostics;
using Microsoft.VisualStudio.DebuggerVisualizers;
using MsgPackVisualizer;

[assembly: DebuggerVisualizer(
               typeof(MsgPackByteArrayVisualizer),
               typeof(MsgPackByteArrayObjectSource),
               Target = typeof(byte[]),
               Description = "MsgPack Buffer Visualizer")
]

namespace MsgPackVisualizer
{
    public class MsgPackByteArrayVisualizer : DialogDebuggerVisualizer
    {
        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider)
        {
            using (var viewer = new MsgPackBufferViewer())
            {
                viewer.SetObjectProvider(objectProvider);
                viewer.ShowDialog();
            }
        }
    }
}