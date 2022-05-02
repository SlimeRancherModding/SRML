using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace SRML.SR.Patches
{
    [HarmonyPatch(typeof(ResourceBundle))]
    [HarmonyPatch("LoadFromText")]
    internal static class ResourceBundlePatch
    {
        [HarmonyPriority(Priority.Last)]
        static void Postfix(string path, Dictionary<string, string> __result, string text)
        {
            if (path == "ui")
            {
                MessageDirector.Lang lang = GameContext.Instance.MessageDirector.GetCultureLang();
                if (TranslationPatcher.srmlErrorMessages.ContainsKey(lang))
                {
                    __result["t.srml_error"] = TranslationPatcher.srmlErrorMessages[lang].Key;
                    __result["t.srml_error.abort"] = TranslationPatcher.srmlErrorMessages[lang].Value;
                }
                else
                {
                    switch (lang)
                    {
                        case MessageDirector.Lang.DE:
                            __result["t.srml_error"] = "SRML-FEHLER";
                            __result["t.srml_error.abort"] = $"Ladevorgang des mod wird abgebrochen...";
                            break;
                        case MessageDirector.Lang.ES:
                            __result["t.srml_error"] = "SRML ERROR";
                            __result["t.srml_error.abort"] = "Cargando mod abortado...";
                            break;
                        case MessageDirector.Lang.FR:
                            __result["t.srml_error"] = "SRML ERREUR";
                            __result["t.srml_error.abort"] = "Annulation du chargement du mod...";
                            break;
                        case MessageDirector.Lang.RU:
                            __result["t.srml_error"] = "SRML ОШИБКА";
                            __result["t.srml_error.abort"] = "Прерывается загрузка модификация...";
                            break;
                        case MessageDirector.Lang.SV:
                            __result["t.srml_error"] = "SRML FEL";
                            __result["t.srml_error.abort"] = "Avbrytande laddning av modifiering...";
                            break;
                        case MessageDirector.Lang.ZH:
                            __result["t.srml_error"] = "SRML 错误";
                            __result["t.srml_error.abort"] = "正在中止模组加载...";
                            break;
                        case MessageDirector.Lang.JA:
                            __result["t.srml_error"] = "SRMLのエラー";
                            __result["t.srml_error.abort"] = "MODのロードを中断しています...";
                            break;
                        case MessageDirector.Lang.PT:
                            __result["t.srml_error"] = "SRML ERROR";
                            __result["t.srml_error.abort"] = "Carregamento mod abortado...";
                            break;
                        case MessageDirector.Lang.KO:
                            __result["t.srml_error"] = "SRML 오류";
                            __result["t.srml_error.abort"] = "모드 로드 중단 중...";
                            break;
                        default:
                            __result["t.srml_error"] = "SRML ERROR";
                            __result["t.srml_error.abort"] = "Aborting mod loading...";
                            break;
                    }
                }
            }

            TranslationPatcher.doneDictionaries[path] = __result;
            if (!TranslationPatcher.patches.TryGetValue(path, out var dict)) return;    
            foreach (var entry in dict)
            {
                __result[entry.Key] = entry.Value;
            }
        }
    }
}
