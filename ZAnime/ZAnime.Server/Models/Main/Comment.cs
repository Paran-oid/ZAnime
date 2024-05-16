﻿using Zanime.Server.Models.Core;

namespace Zanime.Server.Models.Main
{
    public class Comment
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; } = 0;
        public string UserId { get; set; }
        public User User { get; set; }

        public void LikeComment()
        {
            Likes++;
        }

        public void DislikeComment()
        {
            Likes--;
        }
    }
}