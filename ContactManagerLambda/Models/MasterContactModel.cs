using System;
using System.Collections.Generic;
using System.Text;

namespace ContactManagerLambda.Models
{
    public class MasterContactModel
    {
        public int Id { get; set;}
        public string LastName { get; set; }
        public string Firstname { get; set; }
        public string PrimaryEmail { get; set; }
    }
}
