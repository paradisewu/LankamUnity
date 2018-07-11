using System;
using System.Collections.Generic;

namespace XHConfig
{
    public partial class GameJson
    {
        /// <summary>
        /// 
        /// </summary>
        public string version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<GameNode> resource { get; set; }
    }

    public class GameNode
    {
        /// <summary>
        /// 视频测试
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int size { get; set; }


        //public double price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string id { get; set; }


        //public string image { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
    }
}
