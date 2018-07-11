using System.Collections.Generic;

namespace XHConfig
{
    public partial class VideoJson : JsonConfig<VideoJson>
    {
        protected override void Init()
        {
            base.Init();
            Info.Name = "VideoConfig";
        }

        public static VideoNode GetLink(string param)
        {
            return Config.resource.Find((i) => i.name == param);
        }

        public static List<VideoNode> GetResource()
        {
            return Config.resource;
        }

        public static string GetVersion()
        {
            return Config.version;
        }
    }
}
