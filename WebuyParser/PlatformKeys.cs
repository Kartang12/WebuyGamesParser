using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WebuyParser
{
    static class PlatformKeys
    {
        static ConcurrentDictionary<string, string> PLPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "403" },
            {"PS3", "808" },
            {"PS4", "1003" },
            {"Vita", "988" },
            {"XBox360", "782" },
            {"XBoxOne", "1000" },
            {"Nintendo DS", "769" },
            {"3DS", "972" },
            {"Wii", "795" },
            {"Wii U", "993" },
            {"Switch", "1027" }
        });

        static ConcurrentDictionary<string, string> UKPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "403"},
            {"PS3", "808"},
            {"PS4", "1003"},
            {"Vita", "988"},
            {"XBox360", "782"},
            {"XBoxOne", "1000"},
            {"Nintendo DS", "769"},
            {"3DS", "972"},
            {"Wii", "795"},
            {"Wii U", "993"},
            {"Switch", "1064"}
        });

        static ConcurrentDictionary<string, string> PTPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "403" },
            {"PS3", "808" },
            {"PS4", "1007" },
            {"XBox360", "1004" },
            {"XBoxOne", "1006" },
            {"Nintendo DS", "769" },
            {"3DS", "1008" },
            {"Wii", "795" },
            {"Wii U", "993" },
            {"Switch", "1049" },
            {"Vita", "1005" }
        });

        static ConcurrentDictionary<string, string> IEPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "403"},
            {"PS3", "808"},
            {"PS4", "998"},
            {"Vita", "983"},
            {"XBox360", "782"},
            {"XBoxOne", "995"},
            {"Nintendo DS", "769"},
            {"3DS", "972"},
            {"Wii", "795"},
            {"Wii U", "986"},
            {"Switch", "1025"}
        });

        static ConcurrentDictionary<string, string> ITPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "824"},
            {"PS3", "1013"},
            {"PS4", "1001"},
            {"Vita", "990"},
            {"XBox360", "827"},
            {"XBoxOne", "1002"},
            {"Nintendo DS", "834"},
            {"3DS", "977"},
            {"Wii", "831"},
            {"Wii U", "996"},
            {"Switch", "1037"}
        });

        static ConcurrentDictionary<string, string> ESPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "824"},
            {"PS3", "821"},
            {"PS4", "1001"},
            {"Vita", "990"},
            {"XBox360", "827"},
            {"XBoxOne", "1002"},
            {"Nintendo DS", "834"},
            {"3DS", "977"},
            {"Wii", "831"},
            {"Wii U", "996"},
            {"Switch", "1031"}
        });

        static ConcurrentDictionary<string, string> NLPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "403"},
            {"PS3", "808"},
            {"PS4", "998"},
            {"Vita", "983"},
            {"XBox360", "782"},
            {"XBoxOne", "995"},
            {"Nintendo DS", "769"},
            {"3DS", "972"},
            {"Wii", "795"},
            {"Wii U", "986"},
            {"Switch", "1016"}
        });

        static ConcurrentDictionary<string, string> ICPlatformsTable = new ConcurrentDictionary<string, string>(
        new Dictionary<string, string>()
        {
            {"PS2", "824"},
            {"PS3", "821"},
            {"PS4", "1001"},
            {"Vita", "990"},
            {"XBox360", "827"},
            {"XBoxOne", "1002"},
            {"Nintendo DS", "834"},
            {"3DS", "977"},
            {"Wii", "831"},
            {"Wii U", "996"},
            {"Switch", "1031"}
        });

        public static ConcurrentDictionary<string, ConcurrentDictionary<string, string>> CountiesKeys = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>(
        new Dictionary<string, ConcurrentDictionary<string, string>>()
        {
            {"pl", PLPlatformsTable},
            {"uk", UKPlatformsTable},
            {"pt", PTPlatformsTable},
            {"ie", IEPlatformsTable},
            {"it", ITPlatformsTable},
            {"es", ESPlatformsTable},
            {"nl", NLPlatformsTable},
            {"ic", ICPlatformsTable}
        });
    }
}
