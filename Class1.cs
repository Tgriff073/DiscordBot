using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmoticonBot
{
    public class Result3
    {
        public long screen_id { get; set; }
        public string inspectLink { get; set; }
        public int inspect_t { get; set; }
        public string inspect_ms { get; set; }
        public string inspect_a { get; set; }
        public string inspect_d { get; set; }
    }

    public class req3
    {
        public string command { get; set; }
        public int forceOpskins { get; set; }
        public int debug_freeQueueLength { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string inspect_link { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public Result3 result { get; set; }
    }
    public class ResultModified
    {
        public long screen_id { get; set; }
        public int status { get; set; }
        public string time { get; set; }
        public string time_fin { get; set; }
        public int inspect_t { get; set; }
        public long inspect_ms { get; set; }
        public long inspect_a { get; set; }
        public string inspect_d { get; set; }
        public int user_client { get; set; }
        public int prem { get; set; }
        public int prem_logo { get; set; }
        public int prem_rotation_id { get; set; }
        public int mode { get; set; }
        public int resolution_x { get; set; }
        public int resolution_y { get; set; }
        public int item_defindex { get; set; }
        public int item_paintindex { get; set; }
        public int item_rarity { get; set; }
        public int item_quality { get; set; }
        public int item_paintwear { get; set; }
        public double item_floatvalue { get; set; }
        public int item_paintseed { get; set; }
        public long item_inventory { get; set; }
        public int item_origin { get; set; }
        public int item_questid { get; set; }
        public int item_dropreason { get; set; }
        public int item_stattrak { get; set; }
        public int screen_age { get; set; }
    }

    public class secReq
    {
        public string command { get; set; }
        public int forceOpskins { get; set; }
        public int debug_freeQueueLength { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string inspect_link { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public ResultModified result { get; set; }
    }
    public class Result
    {
        public long screen_id { get; set; }
        public int status { get; set; }
        public string time { get; set; }
        public string time_fin { get; set; }
        public int inspect_t { get; set; }
        public long inspect_ms { get; set; }
        public long inspect_a { get; set; }
        public string inspect_d { get; set; }
        public int user_client { get; set; }
        public int prem { get; set; }
        public int prem_logo { get; set; }
        public int prem_rotation_id { get; set; }
        public int mode { get; set; }
        public int resolution_x { get; set; }
        public int resolution_y { get; set; }
        public int item_defindex { get; set; }
        public int item_paintindex { get; set; }
        public int item_rarity { get; set; }
        public int item_quality { get; set; }
        public int item_paintwear { get; set; }
        public double item_floatvalue { get; set; }
        public int item_paintseed { get; set; }
        public long item_inventory { get; set; }
        public int item_origin { get; set; }
        public int item_questid { get; set; }
        public int item_dropreason { get; set; }
        public int item_stattrak { get; set; }
        public string image_url { get; set; }
        public string thumbnail_url { get; set; }
        public string inspectLink { get; set; }
    }
        public class r1
    {
        public long screen_id { get; set; }
        public int status { get; set; }
        public string time { get; set; }
        public string time_fin { get; set; }
        public int inspect_t { get; set; }
        public long inspect_ms { get; set; }
        public long inspect_a { get; set; }
        public long inspect_d { get; set; }
        public int user_client { get; set; }
        public int prem { get; set; }
        public int prem_logo { get; set; }
        public int prem_rotation_id { get; set; }
        public int mode { get; set; }
        public int resolution_x { get; set; }
        public int resolution_y { get; set; }
        public int item_defindex { get; set; }
        public int item_paintindex { get; set; }
        public int item_rarity { get; set; }
        public int item_quality { get; set; }
        public int item_paintwear { get; set; }
        public double item_floatvalue { get; set; }
        public int item_paintseed { get; set; }
        public long item_inventory { get; set; }
        public int item_origin { get; set; }
        public int item_questid { get; set; }
        public int item_dropreason { get; set; }
        public int item_stattrak { get; set; }
        public int screen_age { get; set; }
    }

    public class req
    {
        public string command { get; set; }
        public int forceOpskins { get; set; }
        public int debug_freeQueueLength { get; set; }
        public int imageWidth { get; set; }
        public int imageHeight { get; set; }
        public string inspect_link { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public r1 result { get; set; }
    }

    public class Csitem
        {
            public string command { get; set; }
            public bool success { get; set; }
            public Result result { get; set; }
        }
    
}
