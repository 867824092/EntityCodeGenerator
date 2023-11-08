using System.IO;
using System.Threading.Tasks;

namespace ECG.Avalonia.WPF.Utility; 

public class FileHepler {
    // 如果文件夹不存在则创建
    public static void CreateDirectoryIfNotExists(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }
    // 如果文件不存在则创建，如果存在则覆盖，异步方法
    public static async Task CreateFileIfNotExistsAsync(string path, string content) {
        await using var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        await using var writer = new StreamWriter(stream,System.Text.Encoding.UTF8);
        await writer.WriteAsync(content);
    }
}