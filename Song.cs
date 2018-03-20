using System;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace EmoticonBot
{
    
    class Song
    {
        public string name;
        public bool isDone;
        public string duration;
        public string link;
        bool paused;
        public MemoryStream song = new MemoryStream();
        public string filename;
        public Song()
        {
            name = " ";
            paused = false;
        }
        
        public async Task<bool> setSong()
        {
            
            
           
            Console.WriteLine($"Trying to convert: {filename}");
            string path = "\"";
            path += filename + "\""; 

            var process = Process.Start(new ProcessStartInfo
            { // FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
                FileName = "ffmpeg",
                Arguments = $"-i {@path} " + // Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
                            "-f s16le -ar 48000 -ac 2 pipe:1", // Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout.
                UseShellExecute = false,
                RedirectStandardOutput = true,
                //RedirectStandardError = true// Capture the stdout of the process

            });

            //await outputBuffer.FlushAsync();
            await process.StandardOutput.BaseStream.FlushAsync();
            await process.StandardOutput.BaseStream.CopyToAsync(song);
            await song.FlushAsync();
            process.WaitForExit();
            song.Seek(0, SeekOrigin.Begin);
            return true;
        }

        public void pause()
        {
            paused = true;
        }
        public void unpause()
        {
            paused = false;
        }
        public void setFilename()
        {
            filename = "C:\\Users\\thoma\\Documents\\visual studio 2015\\Projects\\EmoticonBot\\EmoticonBot\\audio\\" + name + ".mp3";
        }
        public Song(string n, string d, string l, MemoryStream m)
        {
            name = n;
            isDone = false;
            duration = d;
            link = l;
            setFilename();
            song = m;
            paused = false;
        }
        public Song(string n, string d, string l)
        {
            name = n;
            isDone = false;
            duration = d;
            link = l;
            setFilename();
            paused = false;
        }
    }
}
