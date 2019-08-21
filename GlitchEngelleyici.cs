using System.Collections;
using System.Linq;
using DaeGlitchEngelleyici.Enumlar;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace DaeGlitchEngelleyici
{
    public class GlitchEngelleyici : RocketPlugin<GlitchEngelleyiciYapılandırma>
    {
        private Eylem _eylem;

        protected override void Load()
        {
            switch (Configuration.Instance.Eylem.ToLower())
            {
                default:
                    _eylem = Eylem.Yok;
                    break;
                case "at":
                    _eylem = Eylem.At;
                    break;
                case "yasakla":
                    _eylem = Eylem.Yasakla;
                    break;
            }

            StartCoroutine(Kontrol());
        }

        protected override void Unload()
        {
            StopAllCoroutines();
        }

        private IEnumerator Kontrol()
        {
            var maskeler = RayMasks.RESOURCE | RayMasks.SMALL | RayMasks.MEDIUM | RayMasks.LARGE | RayMasks.GROUND | RayMasks.GROUND2;

            while (true)
            {
                yield return new WaitForSeconds(1);

                foreach (var steamOyuncu in Provider.clients.ToList())
                {
                    var oyuncu = UnturnedPlayer.FromSteamPlayer(steamOyuncu);

                    if (Physics.Raycast(oyuncu.Position, Vector3.down, 2048f, maskeler))
                    {
                        continue;
                    }

                    Physics.Raycast(oyuncu.Position, Vector3.up, out var yeryüzü, 2048f, maskeler);
                    
                    oyuncu.Teleport(yeryüzü.point, oyuncu.Rotation);

                    if (_eylem == Eylem.Yok)
                    {
                        Logger.Log(Translate("EylemYok", oyuncu.CharacterName));
                    }
                    else if (_eylem == Eylem.At)
                    {
                        Logger.Log(Translate("EylemAt", oyuncu.CharacterName));
                        oyuncu.Kick(Translate("AtılmaMesajı"));
                    }
                    else
                    {
                        Logger.Log(Translate("EylemYasakla", oyuncu.CharacterName, Configuration.Instance.YasaklamaSüresi));
                        oyuncu.Ban(Translate("YasaklanmaMesajı"), Configuration.Instance.YasaklamaSüresi);
                    }
                }
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
            { "EylemYok", "Açık kullanımı tespit edildi. Eylem: Yok | Oyuncu: {0}" },
            { "EylemAt", "Açık kullanımı tespit edildi. Eylem: At | Oyuncu: {0}" },
            { "AtılmaMesajı", "Açık kullanımı tespit edildi. Bu bir hata ise kurucuya bildir." },
            { "EylemYasakla", "Açık kullanımı tespit edildi. Eylem: Yasakla | Süre: {1} | Oyuncu: {0}" },
            { "YasaklanmaMesajı", "Açık kullanımı tespit edildi. Bu bir hata ise kurucuya bildir." }
        };
    }
}