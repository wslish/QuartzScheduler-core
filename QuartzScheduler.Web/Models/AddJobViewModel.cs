//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace QuartzScheduler.Web.Models
//{
//    public class AddJobViewModel
//    {
//        [Required]
//        [StringLength(100)]
//        public string JobName { get; set; }
//        [Required]
//        [StringLength(100)]
//        public string JobGroup { get; set; }
        
//        [Required]
//        [StringLength(100)]
//        public string CronExpression { get; set; }
//        [Required]
//        public bool IsDurable { get; set; }
//        [Required]
//        [StringLength(200)]
//        public string NameSpace { get; set; }
//        [Required]
//        [StringLength(100)]
//        public string ClassName { get; set; }
//        public List<string> TriggerProperties { get; set; }
//    }
//}