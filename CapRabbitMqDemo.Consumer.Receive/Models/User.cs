using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CapRabbitMqDemo.Consumer.Receive.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime ReistDate { get; set; }
    }
}
