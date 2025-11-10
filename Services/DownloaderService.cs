using System.Diagnostics;

namespace MusicDownloaderAPI.Services
{
    public class DownloaderService
    {
        private readonly string _downloadPath;
        const string YTDLP_PATH = "/usr/local/bin/yt-dlp";

        public DownloaderService()
        {
            _downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            Directory.CreateDirectory(_downloadPath);
        }

        public async Task<string?> DownloadAndConvertToMp3Async(string url)
        {
            try
            {
                // Archivo temporal de salida
                string outputTemplate = Path.Combine(_downloadPath, "%(title)s.%(ext)s");

                // 1️⃣ Descargar audio con yt-dlp (y convertir a mp3)
                var ytDlProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = YTDLP_PATH ,
                        Arguments = $"-f bestaudio --extract-audio --audio-format mp3 -o \"{outputTemplate}\" \"{url}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                ytDlProcess.Start();
                string output = await ytDlProcess.StandardOutput.ReadToEndAsync();
                string error = await ytDlProcess.StandardError.ReadToEndAsync();
                ytDlProcess.WaitForExit();

                if (ytDlProcess.ExitCode != 0)
                {
                    Console.WriteLine($"yt-dlp error: {error}");
                    return null;
                }

                // Buscar el archivo más reciente
                var latestFile = Directory.GetFiles(_downloadPath)
                    .OrderByDescending(File.GetCreationTime)
                    .FirstOrDefault();

                return latestFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error general: {ex.Message}");
                return null;
            }
        }
    }
}

