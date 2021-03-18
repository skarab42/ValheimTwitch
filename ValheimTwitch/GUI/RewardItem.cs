using UnityEngine;
using UnityEngine.UI;

namespace ValheimTwitch.GUI
{
    class RewardItem : MonoBehaviour
    {
        private GameObject goRewardItem;

        private void Awake()
        {
            goRewardItem = new GameObject("RewardItem");
            goRewardItem.transform.SetParent(transform);

            var image = goRewardItem.AddComponent<Image>();
            image.color = new Color32(255, 0, 0, 200);
        }
    }
}
