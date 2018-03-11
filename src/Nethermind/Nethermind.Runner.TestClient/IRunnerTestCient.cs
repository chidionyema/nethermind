using System.Threading.Tasks;

namespace Nethermind.Runner.TestClient
{
    public interface IRunnerTestCient
    {
        Task<string> SendEthProtocolVersion();
        string SendEthAccounts();
        string SendNetVersion();
        string SendWeb3ClientVersion();
        string SendWeb3Sha3(string content);
    }
}