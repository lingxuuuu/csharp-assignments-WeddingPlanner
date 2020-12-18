using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; //let us use List<movie>

namespace WeddingPlanner.Models
{
    public class Wedding
    {
        [Key] //signifies that this is the unique identifier(example: Like a ssn number)
        public int WeddingId { get; set; }

        [Required(ErrorMessage = "Please provide this info to procced.")]
        [Display(Name = "Wedder One")]
        public string WedderOne { get; set; }

        [Required(ErrorMessage = "Please providethis info to procced.")]
        [Display(Name = "Wedder Two")]
        public string WedderTwo { get; set; }

        [Required(ErrorMessage = "Please providethis info to procced.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Please providethis info to procced.")]
        [Display(Name = "Wedding Address")]
        public string WeddingAddress { get; set; }

        public int UserId { get; set; }

        // the user object, get populated by .NET(not in the db originally)
        // Wedding does not have a User PlanedBy field. We need to use include() to connect the UserId with PostedBy
        //one wedding planedBy one user
        public User PlanedBy { get; set; }

        //many to many, add after creating Middle Model
        //one wedding have a list of Rsvps
        public List<Rsvp> Rsvps {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}