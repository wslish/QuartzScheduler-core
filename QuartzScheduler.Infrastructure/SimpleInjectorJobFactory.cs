using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;
using Quartz;

namespace QuartzScheduler.Infrastructure
{
    public class SimpleInjectorJobFactory : IJobFactory
    {

        IServiceProvider serviceProvider;
        public SimpleInjectorJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)serviceProvider.GetService(bundle.JobDetail.JobType);
        }

        public virtual void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}
