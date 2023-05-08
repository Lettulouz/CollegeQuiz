using System.Threading.Tasks;
using FluentFTP;

namespace CollegeQuizWeb.Sftp;

public interface IAsyncSftpConnector
{
    /// <summary>
    /// Method that connect user to server
    /// </summary>
    /// <returns>AsyncFtpClient</returns>
    Task<AsyncFtpClient> Connect();
    
    /// <summary>
    /// Method that disconnect user to server
    /// </summary>
    Task Disconnect();
}