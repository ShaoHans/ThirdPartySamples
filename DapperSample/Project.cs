using System;
using System.Collections.Generic;
using System.Text;

namespace DapperSample
{
    public class Project
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public decimal ProjectAmount { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public DateTime? ActualBeginTime { get; set; }

        public DateTime? ActualEndTime { get; set; }

        public DateTime CreateTime { get; set; }

        public User User { get; set; }

        public override string ToString()
        {
            return $"项目名称：{ProjectName}，金额：{ProjectAmount}，创建时间：{CreateTime.ToShortDateString()}，预计周期：{BeginTime.ToShortDateString()}-{EndTime.ToShortDateString()}";
        }

    }
}
