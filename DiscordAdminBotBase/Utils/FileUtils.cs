using System.IO;
using System.Text.RegularExpressions;

namespace DiscordAdminBotBase.Utils
{
    public static class FileUtils
    {
        // Removes any file breaking characters
        public static string MakeValidFileName(string name)
        {
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            var invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return Regex.Replace(name, invalidRegStr, "_");
        }
    }
}