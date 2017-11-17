//using System.IO;
//using Quartz;
//using System.Threading.Tasks;

//namespace QuartzScheduler.Infrastructure
//{
//   public class JobListener : IJobListener
//    {

//        public string Name
//        {
//            get { return "MyJobListener"; }
//        }

//       public async Task JobExecutionVetoed(IJobExecutionContext context)
//        {
//            File.AppendAllText(@"C:\temp\test.txt", "Vetoed  ");
//        }

//        public Task JobToBeExecuted(IJobExecutionContext context)
//        {
//            File.AppendAllText(@"C:\temp\test.txt", "ToBeExecuted  ");
//        }

//        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
//        {
//            File.AppendAllText(@"C:\temp\test.txt", "Executed  ");
//        }
//    }
//}
