using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzScheduler.Web.Models
{
    public class ApiResultModel<T>
    {
        /// <summary>
        /// 状态码 0:成功 非0:失败
        /// </summary>
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResultModel()
        {
            Code = 0;
            Message = "操作成功";
        }
    }
}
