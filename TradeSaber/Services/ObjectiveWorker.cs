using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public class ObjectiveWorker : IHostedService
    {
        private DateTime _nextTime;
        private readonly IHost _host;
        private readonly Random _random;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private const string _storesURL = "https://raw.githubusercontent.com/TradeSaber/Stores/master/objective_data.json";

        public ObjectiveWorker(IHost host, Random random, ILogger<ObjectiveWorker> logger, HttpClient httpClient)
        {
            _host = host;
            _random = random;
            _logger = logger;
            _httpClient = httpClient;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Initializing worker...");
            _nextTime = DateTime.Today.AddDays(1);
            _logger.LogInformation($"Next Cycle: {_nextTime}");
            _ = Run(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Shutting down...");
            return Task.CompletedTask;
        }

        private async Task Run(CancellationToken token)
        {
            while (true)
            {
                await Task.Delay(6000, token);
                if (token.IsCancellationRequested)
                {
                    break;
                }
                if (_nextTime >= DateTime.Today)
                {
                    return;
                }
                _nextTime = DateTime.Today;
                _logger.LogInformation($"Next Cycle: {_nextTime}");
                _logger.LogInformation("Fetching store...");
                HttpResponseMessage response = await _httpClient.GetAsync(_storesURL, token);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Deserializing store...");
                    Objective.Data? data = await JsonSerializer.DeserializeAsync<Objective.Data>(await response.Content.ReadAsStreamAsync(token), cancellationToken: token);
                    if (data is not null)
                    {
                        using IServiceScope scope = _host.Services.CreateScope();

                        ObjectiveIconService objectiveIconService = scope.ServiceProvider.GetRequiredService<ObjectiveIconService>();
                        TradeContext tradeContext = scope.ServiceProvider.GetRequiredService<TradeContext>();
                        int rangeIndex = _random.Next(0, data.ObjectiveRange.Length);
                        int range = data.ObjectiveRange[rangeIndex];

                        var oldActiveObjectives = await tradeContext.Objectives.Where(o => o.Active && !o.Special).ToListAsync(cancellationToken: token);
                        foreach (var objective in oldActiveObjectives)
                        {
                            objective.Active = false;
                        }
                        for (int i = 0; i < range; i++)
                        {
                            var types = ActiveTypes();
                            int typeIndex = _random.Next(0, types.Length);
                            Objective.Type type = types[typeIndex];

                            _logger.LogInformation($"Generating random objective of type {type}...");
                            string? subject = null;
                            string template = "";

                            switch (type)
                            {
                                case Objective.Type.PlayLevel:

                                    int levelIndex = _random.Next(0, data.Levels.Length);
                                    Objective.Data.Level level = data.Levels[levelIndex];
                                    
                                    StringBuilder builder = new("Play and beat {name} by {author}");
                                    subject = $"hash:{level.Hash}|";
                                    if (level.Key is not null)
                                        subject += $"key:{level.Key}|";
                                    if (level.LevelDifficulty is not null)
                                        subject += $"difficulty:{(int)level.LevelDifficulty}|";
                                    if (level.Characteristic is not null)
                                        subject += $"characteristic:{level.Characteristic}|";
                                    if (FiftyFifty())
                                    {
                                        int maxIndex = _random.Next(0, data.MaxPercents.Length);
                                        float max = data.MaxPercents[maxIndex];
                                        subject += $"max:{max}|";
                                        builder.Append(" and get at least a {max}");
                                    }
                                    builder.Append('.');
                                    template = builder.ToString();
                                    break;
                                case Objective.Type.PlayXLevels:

                                    int playRangeIndex = _random.Next(0, data.PlayRange.Length);
                                    int playRange = data.PlayRange[playRangeIndex];

                                    subject = playRange.ToString();
                                    template = "Play and beat {x} levels.";

                                    break;
                                case Objective.Type.UseModifier:

                                    int modifierIndex = _random.Next(0, data.Modifiers.Length);
                                    Objective.Data.Modifier modifier = data.Modifiers[modifierIndex];

                                    subject = ((int)modifier).ToString();
                                    template = "Play and beat a map with the {modifier} modifier.";
                                    break;
                                case Objective.Type.SessionLength:

                                    int sessionIndex = _random.Next(0, data.SessionLengths.Length);
                                    float session = data.SessionLengths[sessionIndex];

                                    subject = session.ToString();
                                    template = "Play {x} minutes of maps today.";

                                    break;
                                case Objective.Type.WinMultiplayerMatch:

                                    template = "Win a multiplayer match.";

                                    break;
                                case Objective.Type.PlayMultiplayerMatch:

                                    template = "Play a multiplayer match.";

                                    break;
                            }

                            int tirIndex = _random.Next(0, data.TirRewards.Length);
                            float tir = data.TirRewards[tirIndex];

                            int xpIndex = _random.Next(0, data.XPRewards.Length);
                            float xp = data.XPRewards[xpIndex];

                            Pack? pack = null;
                            if (data.PackRewards.Length > 0)
                            {
                                int randomPackCount = _random.Next(0, data.PackRewards.Length);
                                Guid id = data.PackRewards[randomPackCount].Pack;
                                pack = await tradeContext.Packs.FindAsync(new object[] { id }, cancellationToken: token);
                            }

                            Media icon = await objectiveIconService.Get(type);
                            Objective objective = new()
                            {
                                Active = true,
                                ID = Guid.NewGuid(),
                                TirReward = tir == default ? null : tir,
                                XPReward = xp == default ? null : xp,
                                ObjectiveType = type,
                                Template = template,
                                Subject = subject,
                                Icon = icon,
                                PackRewards = pack == null ? new List<Pack.Reference>() : new List<Pack.Reference> { new Pack.Reference { Pack = pack } }
                            };

                            tradeContext.Objectives.Add(objective);
                        }
                        await tradeContext.SaveChangesAsync(token);
                    }
                }
                else
                {

                    _logger.LogWarning("Could not get store.");
                }
            }
        }

        private bool FiftyFifty()
        {
            return _random.Next() % 2 == 0;
        }

        private static Objective.Type[] ActiveTypes()
        {
            return new Objective.Type[]
            {
                Objective.Type.PlayLevel,
                Objective.Type.PlayXLevels,
                Objective.Type.UseModifier,
                Objective.Type.SessionLength,
                Objective.Type.WinMultiplayerMatch,
                Objective.Type.PlayMultiplayerMatch,
            };
        }
    }
}