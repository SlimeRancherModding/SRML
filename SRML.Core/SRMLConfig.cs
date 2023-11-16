using SRML.Config.Attributes;

namespace SRML
{
    [ConfigFile("main", "config")]
    internal static class SRMLConfig
    {
        public static bool CREATE_MARKET_BUTTON = true;
        public static bool FORCE_BASIC_ERROR_HANDLER = false;
    }
}
