using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapRabbitMqDemo.Consumer.Receive.Models;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace CapRabbitMqDemo.Consumer.Receive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [NonAction]
        [CapSubscribe("caprabbitmqdemo.user.regist")]
        public async Task SendEmail(User user)
        {
            Console.WriteLine($"向{user.Email}发送注册邮件");
        }
    }
}
