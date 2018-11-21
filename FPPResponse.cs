using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Light_Emoting_Diode
{
    class FPPResponse
    {
        public string image_id { get; set; }
        public string request_id { get; set; }
        public int time_used { get; set; }
        public Face[] faces { get; set; }
    }

    public class Face
    {
        public Attributes attributes { get; set; }
        public Face_Rectangle face_rectangle { get; set; }
        public string face_token { get; set; }
    }

    public class Attributes
    {
        public Emotion emotion { get; set; }
    }

    public class Emotion
    {
        public int sadness { get; set; }
        public int neutral { get; set; }
        public int disgust { get; set; }
        public int anger { get; set; }
        public int surprise { get; set; }
        public int fear { get; set; }
        public int happiness { get; set; }
    }

    public class Face_Rectangle
    {
        public int width { get; set; }
        public int top { get; set; }
        public int left { get; set; }
        public int height { get; set; }
    }
}