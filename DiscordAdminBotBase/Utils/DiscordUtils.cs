using System;
using System.Collections.Generic;
using Discord;

namespace DiscordAdminBotBase.Utils
{
    public static class DiscordUtils
    {
        /// <summary>
        /// Gets the links in embeds
        /// </summary>
        /// <param name="embedList">The embeds list</param>
        /// <param name="links">The dictionary containing the links</param>
        /// <param name="counter">The counter</param>
        public static void GetEmbedsLinks(IEnumerable<IEmbed> embedList, ref Dictionary<int, string> links, ref int counter)
        {
            // Iterates over all embeds
            foreach (var embed in embedList)
            {
                // Only proceeds if there is any image
                if (embed.Image.HasValue)
                    // Adds the image link and updates the counter
                    links.Add(counter++, embed.Image.Value.Url);
                

                // Only procceeds if there is any video
                if (embed.Video.HasValue)
                    // Adds the video link and updates the counter
                    links.Add(counter++, embed.Video.Value.Url);
                

                // Only procceeds if there is any link
                if (!string.IsNullOrWhiteSpace(embed.Url))
                    // Adds the link and updates the counter
                    links.Add(counter++, embed.Url);
            }
        }

        /// <summary>
        /// Gets all the attachments links
        /// </summary>
        /// <param name="attachmentList">The attachments list</param>
        /// <param name="links">The dictionary containing the links</param>
        /// <param name="counter">The counter</param>
        public static void GetAttachmentsLinks(IEnumerable<IAttachment> attachmentList,
            ref Dictionary<int, string> links, ref int counter)
        {
            // Iterates over all the attachments
            foreach (var attachment in attachmentList)
                // Adds the attachment link and updates the counter
                links.Add(counter++, attachment.Url);
        }
    }
}