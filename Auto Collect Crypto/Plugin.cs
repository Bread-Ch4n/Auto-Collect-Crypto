using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using IAmFuture.Data.Character.Statuses;
using IAmFuture.Gameplay.Cryptocurrency;

namespace Auto_Collect_Crypto;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    private static ManualLogSource _logger;

    private static Harmony _harmony;

    [HarmonyPatch(typeof(CryptocurrencyFarm))]
    class CryptocurrencyFarmPatch
    {
        [HarmonyPatch(typeof(CryptocurrencyFarm), "Construct")]
        static bool Prefix(CryptocurrencyFarm __instance)
        {
            __instance.OnCryptoCountChange += __instance.TransferCollectedCryptoIntoAccount;
            return true;
        }
    }

    [HarmonyPatch(typeof(CryptocurrencyServiceSFX))]
    class CryptocurrencyFarmSfxPatch
    {
        [HarmonyPatch(typeof(CryptocurrencyServiceSFX), "ResolveCryptoCurrencyAmountChanged")]
        static bool Prefix(CryptocurrencyServiceSFX __instance) { return false; }
    }

    [HarmonyPatch(typeof(GUI_CurrencyCounterSFX))]
    class CryptocurrencyCounterSfxPatch
    {
        [HarmonyPatch(typeof(GUI_CurrencyCounterSFX), "ResolveStartedAddingTween")]
        static bool Prefix(GUI_CurrencyCounterSFX __instance) { return false; }
    }

    private void Awake()
    {
        _logger = Logger;
        _logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        _harmony = new Harmony("auto_collect_crypto");
        _harmony.PatchAll();
    }

    private void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }
}