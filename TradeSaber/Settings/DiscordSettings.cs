﻿using System;

namespace TradeSaber.Settings
{
    public class DiscordSettings
    {
        public string ID { get; init; } = null!;
        public string URL { get; init; } = null!;
        public string Token { get; init; } = null!;
        public string Secret { get; init; } = null!;
        public string MascotID { get; set; } = null!;
        public string RedirectURL { get; init; } = null!;
        public string[] Roots { get; set; } = Array.Empty<string>();
    }
}