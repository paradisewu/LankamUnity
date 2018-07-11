using System;
using System.Collections.Generic;

namespace XHConfig
{
    public partial class VideoJson
    {
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<VideoNode> resource { get; set; }
    }

    public class VideoNode
    {
        /// <summary>
        /// 视频测试
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }
}
