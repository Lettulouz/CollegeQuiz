using System.Threading.Tasks;
using FluentFTP;

namespace CollegeQuizWeb.Sftp;

public interface IAsyncSftpConnector
{
    Task<AsyncFtpClient> Connect();
    Task Disconnect();
}