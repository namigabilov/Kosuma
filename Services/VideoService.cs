using System.Collections.Concurrent;
using System.Diagnostics;

namespace Kosuma.Services
{
    public static class VideoService
    {

        private static readonly string ffmpegPath = "/usr/bin/ffmpeg";
        private static readonly ConcurrentDictionary<string, string> m3u8Files = new ConcurrentDictionary<string, string>();

        public static bool InitializeStream(string streamId, string outputDirectory)
        {
            var directory = Path.Combine(outputDirectory, streamId);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var m3u8FilePath = Path.Combine(directory, $"{streamId}.m3u8");

            if (!File.Exists(m3u8FilePath))
            {
                // FFMPEG otomatik olarak m3u8 dosyasını oluşturacağı için burada boş bir dosya yazmanıza gerek yok
                m3u8Files[streamId] = m3u8FilePath;
                return true;
            }

            return false;
        }

        public static void RemoveEndlistTag(string m3u8FilePath)
        {
            var lines = File.ReadAllLines(m3u8FilePath);
            if (lines.Length > 0 && lines[lines.Length - 1] == "#EXT-X-ENDLIST")
            {
                File.WriteAllLines(m3u8FilePath, lines.Take(lines.Length - 1));
            }
        }


        public static void ConvertSegmentToTs(string streamId, string inputFile, string outputDirectory)
        {
            // FFMPEG tarafından otomatik olarak oluşturulacak dosya adı formatını belirliyoruz
            string ffmpegArgs;
            string masterFilePath = $"{outputDirectory}/stream.m3u8";
            string segmentFilePath = $"{outputDirectory}/data%02d.ts";

            if (!System.IO.File.Exists(masterFilePath))
            {
                // İlk seferde master .m3u8 dosyasını oluştur
                ffmpegArgs = $"-i \"{inputFile}\" " +
                             "-c:v libx264 -preset fast -crf 23 -c:a aac -b:a 192k -strict experimental " +
                             "-f hls " +
                             "-hls_time 2 " +
                             "-hls_list_size 5 " +
                             "-hls_flags delete_segments+independent_segments " +
                             "-hls_segment_type mpegts " +
                             $"-hls_segment_filename \"{segmentFilePath}\" " +
                             $"-var_stream_map \"v:0,a:0\" " +
                             $"\"{masterFilePath}\"";
            }
            else
            {
                // Master .m3u8 zaten varsa yeni segmentleri eklemek için dönüştür
                ffmpegArgs = $"-i \"{inputFile}\" " +
                             "-c:v libx264 -preset fast -crf 23 -c:a aac -b:a 192k -strict experimental " +
                             "-f hls " +
                             "-hls_time 2 " +
                             "-hls_list_size 5 " +
                             "-hls_flags append_list+independent_segments " +
                             "-hls_segment_type mpegts " +
                             $"-hls_segment_filename \"{segmentFilePath}\" " +
                             $"-var_stream_map \"v:0,a:0\" " +
                             $"\"{masterFilePath}\"";
            }

            using (var process = new Process())
            {
                process.StartInfo.FileName = ffmpegPath;
                process.StartInfo.Arguments = ffmpegArgs;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var errorOutput = process.StandardError.ReadToEnd();
                    var standardOutput = process.StandardOutput.ReadToEnd();

                    Console.WriteLine($"FFMPEG Error for Stream {streamId}: {errorOutput}");
                    Console.WriteLine($"FFMPEG Output for Stream {streamId}: {standardOutput}");
                }
                else
                {

                    var m3u8FilePath = Path.Combine(outputDirectory, "stream.m3u8");
                    RemoveEndlistTag(m3u8FilePath);

                    Console.WriteLine($"Segment başarıyla dönüştürüldü ve {streamId}.m3u8 dosyasına eklendi.");
                }
            }
        }

    }
}