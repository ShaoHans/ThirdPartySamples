using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CapRabbitMqDemo.Producer.Send.Data;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;

namespace CapRabbitMqDemo.Producer.Send.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICapPublisher _capPublisher;
        private readonly AppDbContext _dbContext;

        public AccountController(ICapPublisher capPublisher, AppDbContext dbContext)
        {
            _capPublisher = capPublisher;
            _dbContext = dbContext;
        }

        public IActionResult Regist()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Regist(User user)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            string result = string.Empty;
            try
            {
                using (var trans = await _dbContext.Database.BeginTransactionAsync())
                {
                    user.ReistDate = DateTime.Now;
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                    await _capPublisher.PublishAsync("caprabbitmqdemo.user.regist", user);
                    trans.Commit();
                    result = "恭喜你注册成功，激活链接已发送至您填写的邮箱";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            TempData["RegistResult"] = result;
            return RedirectToAction("RegistResult");
        }

        public IActionResult RegistResult()
        {
            return View();
        }
    }
}