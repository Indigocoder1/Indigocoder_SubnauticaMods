using FMOD;
using Nautilus.Handlers;
using Nautilus.Utility;
using UnityEngine;

namespace Chameleon
{
    internal static class ChameleonAudio
    {
        public const MODE k3DSoundModes = MODE.DEFAULT | MODE._3D | MODE.ACCURATETIME | MODE._3D_LINEARSQUAREROLLOFF;
        public const MODE k2DSoundModes = MODE.DEFAULT | MODE._2D | MODE.ACCURATETIME;
        public const MODE kStreamSoundModes = k2DSoundModes | MODE.CREATESTREAM;

        internal static void RegisterAudio(AssetBundle bundle)
        {
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonWelcomeAboard"), "ChameleonWelcomeAboard");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonWelcomeNegative"), "ChameleonWelcomeAboardNegative");

            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonSlowSpeed"), "ChameleonSpeedSlow");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonStandardSpeed"), "ChameleonSpeedStandard");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonFlankSpeed"), "ChameleonSpeedFlank");

            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonEngineUp"), "ChameleonEngineOn");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonEngineDown"), "ChameleonEngineOff");

            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonCreatureAttack"), "ChameleonCreatureAttack");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonHealthLow"), "ChameleonHealthLow");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonHealthCritical"), "ChameleonHealthCritical");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonAbandonShip"), "ChameleonAbandonShip");

            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonNoPower"), "ChameleonNoPower");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonDamageNotification"), "ChameleonDamageNotification");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonCrushDepth"), "ChameleonCrushDepth");

            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonCloakEnable"), "ChameleonCloakEnable");
            AddSubVoiceLine(bundle.LoadAsset<AudioClip>("ChameleonCloakDisable"), "ChameleonCloakDisable");
        }

        public static void AddSubVoiceLine(AudioClip clip, string soundPath)
        {
            var sound = AudioUtils.CreateSound(clip, kStreamSoundModes);
            CustomSoundHandler.RegisterCustomSound(soundPath, sound, AudioUtils.BusPaths.VoiceOvers);
        }
    }
}
