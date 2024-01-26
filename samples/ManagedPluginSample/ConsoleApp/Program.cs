using System;

namespace ManagedPluginSample
{
    class Program
    {
        static void Main(string[] args)
        {
            new SamplePluginWrapper().Run();

            var sampleClientWrapper = new SampleClientWrapper();
            sampleClientWrapper.Connect("1234567890");
            sampleClientWrapper.Disconnect();
        }
    }
}
