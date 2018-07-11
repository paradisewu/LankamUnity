using System.Collections.Generic;

namespace XHConfig
{
    public partial class GameJson : JsonConfig<GameJson>
    {
        protected override void Init()
        {
            base.Init();
            Info.Name = "GameConfig";
        }

        public static GameNode GetLink(string param)
        {
            return Config.resource.Find((i) => i.name == param);
        }

        public static List<GameNode> GetResource()
        {
            return Config.resource;
        }

        public static string GetVersion()
        {
            return Config.version;
        }
    }
}
