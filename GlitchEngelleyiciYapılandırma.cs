using Rocket.API;

namespace DaeGlitchEngelleyici
{
    public class GlitchEngelleyiciYapılandırma : IRocketPluginConfiguration
    {
        public string Eylem { get; set; }

        public uint YasaklamaSüresi { get; set; }

        public void LoadDefaults()
        {
            Eylem = "Yok";

            YasaklamaSüresi = 43200;
        }
    }
}