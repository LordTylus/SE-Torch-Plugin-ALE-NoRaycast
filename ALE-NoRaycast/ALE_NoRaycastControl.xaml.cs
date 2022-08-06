using System.Windows;
using System.Windows.Controls;

namespace ALE_NoRaycast {
    public partial class ALE_NoRaycastControl : UserControl {

        private ALE_NoRaycast Plugin { get; }

        private ALE_NoRaycastControl() {
            InitializeComponent();
        }

        public ALE_NoRaycastControl(ALE_NoRaycast plugin) : this() {
            Plugin = plugin;
        }
    }
}
