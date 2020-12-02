using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManagerLambda.Models
{
    public class SecondaryEmailModel
    {
        public int Id { get; set; }
        public int ContactId { get; set; }
        public string Email { get; set; }
        
    }
}
