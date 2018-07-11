namespace XHConfig
{
    public partial class MeachineConfig : XmlConfig<MeachineConfig>
    {
        protected override void Init()
        {
            base.Init();
            Info.Name = "MeachineConfig";
        }


        public static MeachineItem GetMeachine()
        {
            return Config.Items[0];
        }
    }
}
