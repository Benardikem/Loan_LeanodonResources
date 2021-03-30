using API.Utilities;
using Quartz;
using System.Threading.Tasks;

namespace Web.Codes
{
    public class EmailJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => Messaging.SendPendingMails());
        }
    }
}
