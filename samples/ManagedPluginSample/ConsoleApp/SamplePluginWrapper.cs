using System;
using System.IO;

namespace ManagedPluginSample
{
    public class SamplePluginWrapper
    {
        dynamic _japaneseEraProvider;

        public bool IsAvailable { get; }

        public SamplePluginWrapper()
        {
            var directory = "../Plugin/bin/Debug/net8.0";
            // var directory = "../Plugin/bin/Release/net8.0";

            var dllName = "ManagedPluginSample.Plugin.dll";
            var typeFullName = "ManagedPluginSample.Plugin.JapaneseEraProvider";

            try
            {
                var pluginLoader = new PluginLoader(Path.Combine(directory, dllName));
                _japaneseEraProvider = pluginLoader.CreateInstance(typeFullName);
                IsAvailable = true;
            }
            catch (Exception ex)
            {
                IsAvailable = false;
                Console.WriteLine(ex);
            }
        }

        public void Run()
        {
            if (!IsAvailable)
            {
                Console.WriteLine($"[{nameof(SamplePluginWrapper)}] Plugin is not available.");
                return;
            }

            try
            {
                var years = new int[]{ 2024, 1999, 1974, 1924, 1899, 1867 };
                foreach (int year in years)
                {
                    var era = (int)_japaneseEraProvider.GetJapaneseEra(year) switch
                    {
                        1 => "明治",
                        2 => "大正",
                        3 => "昭和",
                        4 => "平成",
                        5 => "令和",
                        _ => "Unknown",
                    };

                    Console.WriteLine($"[{nameof(SamplePluginWrapper)}] 西暦{year}年は和暦では「{era}」です。");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
