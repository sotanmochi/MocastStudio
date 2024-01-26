namespace ManagedPluginSample.Plugin
{
    public enum JapaneseEra
    {
        Unknown,
        Meiji,
        Taisho,
        Showa,
        Heisei,
        Reiwa
    }

    public class JapaneseEraProvider
    {
        public JapaneseEra GetJapaneseEra(int year)
        {
            if (year < 1868)
            {
                return JapaneseEra.Unknown;
            }
            else if (year < 1912)
            {
                return JapaneseEra.Meiji;
            }
            else if (year < 1926)
            {
                return JapaneseEra.Taisho;
            }
            else if (year < 1989)
            {
                return JapaneseEra.Showa;
            }
            else if (year < 2019)
            {
                return JapaneseEra.Heisei;
            }
            else
            {
                return JapaneseEra.Reiwa;
            }
        }
    }
}
