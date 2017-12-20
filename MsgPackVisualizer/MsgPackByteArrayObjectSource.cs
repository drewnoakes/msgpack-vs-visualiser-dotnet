using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace MsgPackVisualizer
{
    public class MsgPackByteArrayObjectSource : VisualizerObjectSource
    {
        public override void GetData(object target, Stream outgoingData)
        {
            if (target is byte[] buffer)
            {
                try
                {
                    outgoingData.Write(buffer, 0, buffer.Length);
                }
                catch
                {
                    // ignored
                }
            }
        }
    }
}