using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSample
{
    public class User
    {
        public string AccountName { get; set; }

        public string RealName { get; set; }

        public string Sex { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public override string ToString()
        {
            return $"用户名：{AccountName}，邮箱：{Email}，激活状态：{Active}";
        }
    }
}
