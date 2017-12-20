using System;
using System.IO;
using Dasher;
using Microsoft.VisualStudio.DebuggerVisualizers;

namespace MsgPackVisualizer.Sample
{
    internal static class Program
    {
        private static void Main()
        {
            var stream = new MemoryStream();
            var packer = new Packer(stream);

            packer.PackMapHeader(8);

            packer.Pack("Int32");
            packer.Pack(1234);

            packer.Pack("Null");
            packer.PackNull();

            packer.Pack("String");
            packer.Pack("Hello, World!");

            packer.Pack("Float32");
            packer.Pack(12.34f);

            packer.Pack("Float64");
            packer.Pack(12.34d);

            packer.Pack("True");
            packer.Pack(true);

            packer.Pack("False");
            packer.Pack(true);

            packer.Pack("Array");
            packer.PackArrayHeader(3);
            packer.Pack(1234);
            packer.PackNull();
            packer.Pack("Foo");

            packer.Flush();

            var bytes = stream.GetBuffer();

            Console.WriteLine($"Packed {bytes.Length} bytes");

            // Show the visualizer (via code)
            var developmentHost = new VisualizerDevelopmentHost(
                bytes,
                typeof(MsgPackByteArrayVisualizer),
                typeof(MsgPackByteArrayObjectSource)
            );

            developmentHost.ShowVisualizer();
        }
    }
}