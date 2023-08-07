﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    internal class CommandModule : BaseCommandModule
    {
        [Command("coin")]
        public async Task CoinCommand(CommandContext ctx)
        {
            string[] sidesСoin = new[] { "Орел", "Решка" };
            Random rnd = new();
            await ctx.Channel.SendMessageAsync("Тебе выпало: " + sidesСoin[rnd.Next(0, sidesСoin.Length)]);
        }

        [Command("random")]
        public async Task RandomCommand(CommandContext ctx, int from, int to)
        {
            var random = new Random();
            await ctx.Channel.SendMessageAsync($"Рандомное число: {random.Next(from, to)}");
        }
    }
}
