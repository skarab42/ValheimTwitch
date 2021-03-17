using UnityEngine;
using ValheimTwitch.Patches;

namespace ValheimTwitch.GUI
{
    public class ValeimTwitchStartup : MonoBehaviour
    {
        public StartGui startGui;

        private void Awake()
        {
            startGui = gameObject.AddComponent<StartGui>();

            var mainMenu = GetComponentInChildren<FejdStartup>().m_mainMenu;
            var listener = mainMenu.gameObject.GetStateListener();

            listener.Enabled += () => SetActive(true);
            listener.Disabled += () => SetActive(false);
        }

        private void SetActive(bool active)
        {
            startGui.SetActive(active);
        }
    }
}
