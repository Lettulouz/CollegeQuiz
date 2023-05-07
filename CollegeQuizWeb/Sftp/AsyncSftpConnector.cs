using System.Threading.Tasks;
using CollegeQuizWeb.Config;
using FluentFTP;

namespace CollegeQuizWeb.Sftp;

public class AsyncSftpConnector : IAsyncSftpConnector
{
    private AsyncFtpClient _ftpClient;
    
    public async Task<AsyncFtpClient> Connect()
    {
        _ftpClient = new AsyncFtpClient(ConfigLoader.SftpHost, ConfigLoader.SftpUsername, ConfigLoader.SftpPassword);
        await _ftpClient.AutoConnect();
        return _ftpClient;
    }

    public async Task Disconnect()
    {
        await _ftpClient.Disconnect();
    }
}