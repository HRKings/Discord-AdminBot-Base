using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordAdminBotBase.Utils;

namespace DiscordAdminBotBase.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        // The part of the link to remove
        private const string MESSAGE_BASE_LINK = @"https://discordapp.com/channels/";
        
        /// <summary>
        /// Creates a file containing a JSON dump with the quantity of links sent before the specified message
        /// </summary>
        /// <param name="messageLink">The link to start searching</param>
        /// <param name="quantity">The quantity of links to retrieve</param>
        [Command("dumplinks")]
        [Summary("Save all the links in a chat")]
        public async Task DumpLinksAsync([Summary("The message link to quote")] string messageLink, [Summary("The quantities of link to save")] int quantity)
        {
            // Initializes the control boolean
            var success = false;

            // Remove the message base part and splits all the IDs into an array
            var stringIDs = messageLink.Replace(MESSAGE_BASE_LINK, "").Split('/');

            // Creates a new array to hold the IDs
            var ids = new ulong[3];

            // Parses all the IDs from strings to unsigned longs and saves the result into the array, also it stores the successes or failures into the boolean
            for (var i = 0; i < 3; i++)
                success = ulong.TryParse(stringIDs[i], out ids[i]);
            

            // If the parse fails, delete the original user command message
            if (!success)
            {
                await Context.Message.DeleteAsync();
                await Console.Out.WriteLineAsync("Could not parse some of the IDs");
                return;
            }

            // Tries to get the guild from the ID
            var sourceGuild = Context.Client.GetGuild(ids[0]);

            // If the guild is null, reply to the user
            if (sourceGuild == null)
            {
                await ReplyAsync("I'm not in the linked server");
                return;
            }

            // Tries to get the channel from the ID
            var sourceChannel = sourceGuild.GetTextChannel(ids[1]);

            // If the channel is null, reply to the user
            if (sourceChannel == null)
            {
                await ReplyAsync("I can't view the linked channel");
                return;
            }

            // Tries to get the message from the ID
            var linkedMessage = sourceChannel.GetMessageAsync(ids[2]).Result;

            // If the message is null, reply to the user
            if (linkedMessage == null)
            {
                await ReplyAsync("The linked message doesn't exist");
                return;
            }

            // Gets the cached messages
            var cachedMessages = (await sourceChannel.GetMessagesAsync(linkedMessage, Direction.Before, quantity).FlattenAsync()).ToList();

            // Puts the linked message in the list
            cachedMessages.Add(linkedMessage);

            // Reverses the list to be in chronological order
            cachedMessages.Reverse();

            // Sends the quantity of found messages in the channel
            await ReplyAsync($"Found {cachedMessages.Count} links.");

            // Creates an empty dictionary and a counter
            var links = new Dictionary<int, string>();
            var counter = 0;

            // Only proceeds if any messages are found
            if (cachedMessages.Any())
            {
                // Iterates over all messages found
                foreach (var message in cachedMessages)
                {
                    // Only tries to pick emotes if the message isn't empty
                    if (!string.IsNullOrWhiteSpace(message.Content))
                    {
                        // Tries to find an emoji tag or return null if none is present
                        var emojiTag = message.Tags.SingleOrDefault(tag => tag.Type == TagType.Emoji);
                        
                        // If an emoji tag is found
                        if(emojiTag is not null)
                            // Adds the emote link and updates the counter
                            links.Add(counter++, Emote.Parse($"<:{((Emote) emojiTag.Value).Name}:{emojiTag.Key.ToString()}>").Url);
                    }

                    // Checks to seed if there are any embeds
                    if (message.Embeds.Count > 0)
                        DiscordUtils.GetEmbedsLinks(message.Embeds, ref links, ref counter);
                    
                    // Checks to seed if there are any attachments
                    if (message.Attachments.Count > 0)
                        DiscordUtils.GetAttachmentsLinks(message.Attachments, ref links, ref counter);
                }

                // Creates a valid file name to save the links
                var fileName = FileUtils.MakeValidFileName($"{sourceGuild.Name}_{sourceChannel.Name}_{linkedMessage.Id}.json");
                
                // Writes the json on a file
                await File.WriteAllTextAsync(fileName, JsonSerializer.Serialize(links));

                // Sends the json back in the channel
                await Context.Channel.SendFileAsync(fileName, $"Here is the json containing {cachedMessages.Count} links from before the message : {linkedMessage.GetJumpUrl()}");

                // Deletes the original file
                File.Delete(fileName);
            }
        }
    }
}