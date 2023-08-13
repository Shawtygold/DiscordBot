﻿using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.SlashCommands
{
    internal class RoleCommands : ApplicationCommandModule
    {
        #region [Role Add]

        [SlashCommand("role_add", "Add role to user.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public static async Task AddRole(InteractionContext ctx,
            [Option("user", "User to be assigned the role.")] DiscordUser user,
            [Option("role", "The role you wish to grant.")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't add a role to it."
                }));

                return;
            }

            if (member.Roles.Contains(role))
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "The user already has this role!"
                }));

                return;
            }

            try
            {
                await member.GrantRoleAsync(role);
            }
            catch (UnauthorizedException)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to add the role **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to add a role to this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Added {role.Mention} to **{member.Username}**"
            }));
        }

        #endregion

        #region [Role Info]

        [SlashCommand("role_info", "Check info for a role.")]
        public static async Task RoleInfo(InteractionContext ctx, [Option("role", "The role you want to know about.")] DiscordRole role)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            int countMembersWithRole = 0;

            IEnumerable<DiscordMember> members;

            try
            {
                members = await ctx.Guild.GetAllMembersAsync();
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to get the members of this server!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            foreach (var member in members)
            {
                if (member.Roles.Any(r => r.Name == role.Name))
                    countMembersWithRole++;
            }

            var result = DateTimeOffset.Now - role.CreationTimestamp;

            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Title = "Role info",
                Color = role.Color,
                Description = $"Name: {role.Name}\nMembers: {countMembersWithRole}\nColor: {role.Color}\nCreated: {(int)result.TotalDays} days ago",
                Footer = new() { Text = $"Id: {role.Id}" }
            }));
        }

        #endregion

        #region [Role Remove]

        [SlashCommand("role_remove", "Remove role from user.")]
        [SlashRequirePermissions(Permissions.Administrator)]
        public static async Task RemoveRole(InteractionContext ctx, [Option("user", "User to remove role.")] DiscordUser user, [Option("role", "The role you want to remove.")] DiscordRole role)
        {
            DiscordMember member;
            try
            {
                member = await ctx.Guild.GetMemberAsync(user.Id);
            }
            catch
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "Hmm. It doesn't look like this user is on the server, so I can't remove the role from him."
                }));

                return;
            }

            if (!member.Roles.Contains(role))
            {
                await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = "User does not have this role!"
                }));

                return;
            }

            try
            {
                await member.RevokeRoleAsync(role);
            }
            catch (UnauthorizedException)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Something went wrong. You or I may not be allowed to remove role from **{member.Username}**! Please check the role hierarchy and permissions."
                }));

                return;
            }
            catch (Exception ex)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Red,
                    Description = $"Hmm, something went wrong while trying to remove a role from this user!\n\nThis was Discord's response:\n> {ex.Message}\n\nIf you would like to contact the bot owner about this, please go to https://t.me/Shawtygoldq and include the following debugging information in the message:\n```{ex}\n```"
                }));

                return;
            }

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Green,
                Description = $"Removed {role.Mention} from **{member.Username}**"
            }));
        }

        #endregion
    }
}
