using System.Threading.Tasks;
using WebJobs.Script.LanguageService.Eventing;

namespace WebJobs.Script.LanguageService
{
    public interface IOmniSharpService
    {
        Task SendEvent(LanguageServiceEvent message);

        void Start();
    }
}