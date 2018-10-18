using UnityEngine;
using UnityEngine.XR;

namespace FVTC.LearningInnovations.Unity
{
    public enum PlatformType
    {
        Unknown,
        Editor,
        Desktop,
        Mobile,
        Web,
        Console,
        AugmentedReality,
        VirtualReality
    }

    public static class PlatformHelper
    {
        public static PlatformType GetPlatformType()
        {
            switch (Application.platform)
            {

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.LinuxEditor:
                    return PlatformType.Editor;

                //case RuntimePlatform.MetroPlayerARM:
                //case RuntimePlatform.MetroPlayerX64:
                //case RuntimePlatform.MetroPlayerX86:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.WindowsPlayer:
                    return GetArVrPlatform() ?? PlatformType.Desktop;

                //case RuntimePlatform.BlackBerryPlayer:
                //case RuntimePlatform.TizenPlayer:
                //case RuntimePlatform.WP8Player:
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    return GetArVrPlatform() ?? PlatformType.Mobile;


                //case RuntimePlatform.FlashPlayer:
                case RuntimePlatform.WebGLPlayer:
                    return PlatformType.Web;

                //case RuntimePlatform.PS3:
                //case RuntimePlatform.PSM:
                //case RuntimePlatform.SamsungTVPlayer:
                //case RuntimePlatform.WiiU:
                //case RuntimePlatform.XBOX360:
                case RuntimePlatform.PS4:
                case RuntimePlatform.PSP2:
                case RuntimePlatform.Switch:
                case RuntimePlatform.XboxOne:
                    return PlatformType.Console;

                //case RuntimePlatform.NaCl:
                case RuntimePlatform.tvOS:
                default:
                    return PlatformType.Unknown;
            }
        }

        private static PlatformType? GetArVrPlatform()
        {
            if (XRDevice.isPresent)
            {
                return UnityEngine.XR.WSA.HolographicSettings.IsDisplayOpaque
                     ? PlatformType.VirtualReality
                     : PlatformType.AugmentedReality;

            }

            return null;
        }
    }
}
