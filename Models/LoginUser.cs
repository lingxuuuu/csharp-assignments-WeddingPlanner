using System;
using System.ComponentModel.DataAnnotations;//let us use DataType
using System.ComponentModel.DataAnnotations.Schema; //let us use [NotMapped] on PSconfirmation


namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        // No other fields! Only need this model for login
        public string LoginEmail {get; set;}

        [DataType(DataType.Password)]
        public string LoginPassword { get; set; }
    }
}