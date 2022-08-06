using NLog;
using Sandbox.Game;
using Sandbox.Game.Entities.Blocks;
using System;
using System.Collections.Generic;
using System.Reflection;
using Torch.Managers.PatchManager;
using Torch.Utils;
using Torch.Utils.Reflected;

namespace ALE_NoRaycast {
    
    [PatchShim]
    public static class ALE_MyProgrammableBlockPatch {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        internal static readonly MethodInfo _programableRecompile =
            typeof(MyProgrammableBlock).GetMethod("Compile", BindingFlags.Instance | BindingFlags.NonPublic) ??
            throw new Exception("Failed to find patch method");

        internal static readonly FieldInfo _compileErrorsField =
            typeof(MyProgrammableBlock).GetField("m_compilerErrors", BindingFlags.Instance | BindingFlags.NonPublic) ??
            throw new Exception("Failed to find patch Field");

        private static readonly MethodInfo _programableRecompilePatch =
            typeof(ALE_MyProgrammableBlockPatch).GetMethod(nameof(SuffixRecompilePb), BindingFlags.Static | BindingFlags.NonPublic) ??
            throw new Exception("Failed to find patch method");

        public static void Patch(PatchContext ctx) {

            try {

                ctx.GetPattern(_programableRecompile).Suffixes.Add(_programableRecompilePatch);

                Log.Info("Patching Successful MyProgrammableBlock!");

            } catch (Exception e) {
                Log.Error(e, "Patching failed!");
            }
        }

        private static void SuffixRecompilePb(MyProgrammableBlock __instance) {

            var value = (List<string>) _compileErrorsField.GetValue(__instance);

            if (value.Count == 0)
                return;

            if (__instance.OwnerId == 0)
                return;

            foreach(var error in value) {

                string serverName = ALE_NoRaycast.Instance.Torch.Config.ChatName;

                if (error.Contains("Raycast") && error.Contains("is prohibited"))
                    MyVisualScriptLogicProvider.SendChatMessageColored(
                        "Raycasting is prohibited on this server!",
                        VRageMath.Color.Violet, serverName, __instance.OwnerId);
            }
        }
    }
}
