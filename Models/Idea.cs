using System;

namespace IdeaApp.Models
{
    public class Idea
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public int Impact { get; set; }

        public int Ease { get; set; }

        public int Confidence { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? ModificationDate { get; set; }

    }
    
}