using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace WebuyParser
{
    static class PlatformKeys
    {
        public static ConcurrentDictionary<string, string> platformsTable = new ConcurrentDictionary<string, string>(
    new Dictionary<string, string>()
{
            {"PS3", "808" },
            {"PS4", "1003" },
            {"XBox360", "782" },
            {"XBoxOne", "1000" },
            {"Nintendo DS", "769" },
            {"3DS", "972" },
            {"Wii", "795" },
            {"Wii U", "993" },
            {"Switch", "1027" },
            {"PS2", "403" },
            {"Vita", "988" }
});
    }
}
