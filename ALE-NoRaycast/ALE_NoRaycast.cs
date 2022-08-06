using NLog;
using System;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.API.Managers;
using Torch.Session;
using VRage.Scripting;
using Sandbox.ModAPI.Ingame;

namespace ALE_NoRaycast {
    public class ALE_NoRaycast : TorchPluginBase, IWpfPlugin {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private ALE_NoRaycastControl _control;
        public UserControl GetControl() => _control ?? (_control = new ALE_NoRaycastControl(this));

        public static ALE_NoRaycast Instance;

        public ALE_NoRaycast() {
            Instance = this;
        }

        public override void Init(ITorchBase torch) {
            base.Init(torch);

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state) {

            switch (state) {

                case TorchSessionState.Loading:

                    using (var blacklist = MyScriptCompiler.Static.Whitelist.OpenIngameBlacklistBatch()) {

                        Type cameraBlock = typeof(IMyCameraBlock);

                        blacklist.AddMembers(cameraBlock, "Raycast");
                        blacklist.AddMembers(cameraBlock, "EnableRaycast");
                    }

                    Log.Info("Session is now Loading!");

                    break;
            }
        }
    }
}
