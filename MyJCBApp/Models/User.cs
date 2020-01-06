using System;
using System.Collections.Generic;

namespace MyJCBApp.Models
{
    public class User
    {
        public string UserEmail { get; set; }
        public int RequestId { get; set; }
        public IEnumerable<Guid> OrganisationIds { get; set; }
    }
}