using System.Collections.Generic;

namespace ChatShared.Helpers
{
    // Lớp tiện ích cung cấp danh mục các biểu tượng cảm xúc (Emoji) chuẩn Unicode.
    public static class EmojiHelper
    {
        // Bảng tra cứu Emoji theo 4 danh mục: Mặt cười, Cử chỉ, Trái tim, Đồ vật.
        public static readonly Dictionary<string, string[]> EmojiCategories = new Dictionary<string, string[]>
        {
            ["Mặt cười"] = new[] { "😀", "😁", "😂", "🤣", "😃", "😄", "😅", "😆", "😉", "😊", "😋", "😎", "😍", "😘", "🥰", "😗", "😙", "🤗", "🤩", "🤔", "🤨", "😐", "😑", "😶", "🙄", "😏", "😣", "😥", "😮", "🤐", "😯", "😪", "😫", "😴", "😌", "😛", "😜", "😝", "🤤", "😒" },
            ["Cử chỉ"] = new[] { "👍", "👎", "👌", "✌️", "🤞", "🤟", "🤘", "🤙", "👈", "👉", "👆", "👇", "☝️", "✋", "🤚", "🖐️", "🖖", "👋", "🤝", "👏", "✍️", "🙏", "💪" },
            ["Trái tim"] = new[] { "❤️", "🧡", "💛", "💚", "💙", "💜", "🖤", "🤍", "🤎", "💔", "❣️", "💕", "💞", "💓", "💗", "💖", "💘", "💝" },
            ["Đồ vật"] = new[] { "🎉", "🎊", "🎈", "🎁", "🔥", "⭐", "🌟", "💫", "✨", "🎵", "🎶", "💯", "🏆", "🥇", "🎯", "🚀" }
        };

        public static List<string> GetAllEmojis()
        {
            var all = new List<string>();
            foreach (var cat in EmojiCategories.Values)
                all.AddRange(cat);
            return all;
        }
    }
}
