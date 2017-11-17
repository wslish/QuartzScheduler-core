using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Quartz;

namespace QuartzScheduler.Infrastructure.Utils
{
    public static class JobUtil
    {
        public static Dictionary<string, Type> JobDic { get; set; }
        public static List<Type> GetJobTypes(string jobsDirectory)
        {
            var jobTypes = new List<Type>();

            var dinfo = new DirectoryInfo(jobsDirectory);
            var files = dinfo.GetFiles("*.dll", SearchOption.AllDirectories).Where(x => x.Name != "Quartz.dll").ToList();

            foreach (var file in files)
            {
                var assembly = Assembly.LoadFile(file.FullName);
                var jobs = assembly.GetTypes()
                    .Where(type => typeof(IJob).IsAssignableFrom(type) && type.IsClass).ToList();

                jobTypes.AddRange(jobs);
            }

            return jobTypes;
        }


        public static bool IsValidCronExpression(string expression)
        {
            return CronExpression.IsValidExpression(expression);
        }
    }
}
