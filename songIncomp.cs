using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoticonBot
{
    
    public class songIncomp
    {
        public string title { get; set; }
        public string length { get; set; }
        public string link { get; set; }
    }
    public class _0
    {
        public string dloadUrl { get; set; }
        public int bitrate { get; set; }
        public string mp3size { get; set; }
    }

    public class _1
    {
        public string dloadUrl { get; set; }
        public int bitrate { get; set; }
        public string mp3size { get; set; }
    }

    public class _2
    {
        public string dloadUrl { get; set; }
        public int bitrate { get; set; }
        public string mp3size { get; set; }
    }

    public class _3
    {
        public string dloadUrl { get; set; }
        public int bitrate { get; set; }
        public string mp3size { get; set; }
    }

    public class _4
    {
        public string dloadUrl { get; set; }
        public int bitrate { get; set; }
        public string mp3size { get; set; }
    }

    public class VidInfo
    {
        public _0 zero { get; set; }
        public _1 one { get; set; }
        public _2 two{ get; set; }
        public _3 three { get; set; }
        public _4 four { get; set; }
    }

    public class songJson
    {
        public string vidID { get; set; }
        public string vidTitle { get; set; }
        public VidInfo vidInfo { get; set; }
    }
}
