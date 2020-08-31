
using System;
using IdeaApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace IdeaApp.DTO
{


    public class UserProfileDTO
    {
        public string Email { get; set; }
        public string FullName { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class IdeaDTO
    {

        public IdeaDTO()
        {

        }
        public IdeaDTO(Idea objRec)
        {
            this.content = objRec.Content;
            this.confidence = objRec.Confidence;
            this.ease = objRec.Ease;
            this.impact = objRec.Impact;
            this.Id = objRec.Id;
            this.created_at = objRec.CreationDate.Ticks;

        }

        public int Id { get; set; }

        public string content { get; set; }

        public int impact { get; set; }

        public int ease { get; set; }

        public int confidence { get; set; }

        public double average_score
        {
            get
            {
                return Math.Round((impact + ease + confidence) / 3.0, 2);
            }
        }

        public long created_at { get; set; }
    }


    public class ResponseDTO
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

}