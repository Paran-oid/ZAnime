﻿namespace Zanime.Server.Models.Main.DTO.Comment_Model
{
    public class CommentVMDisplay
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public int Likes { get; set; }
        public int AnimeID { get; set; }
        public string UserId { get; set; }
    }
}