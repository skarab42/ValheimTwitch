using System;
using UnityEngine;
using ValheimTwitch.Patches;
using ValheimTwitch.Twitch.PubSub.Messages;

namespace ValheimTwitch.Events
{
    internal class SpawnTrollAction : Action
    {
        public override void Run(Redemption redemption)
        {
            ConsoleUpdatePatch.AddAction(() => Spawn("Troll", 1));
        }

        private void Spawn(string type, int level = 1, float offset = 100)
        {
            try
            {
                var prefab = ZNetScene.instance.GetPrefab(type);

                if (!prefab)
                {
                    Log.Error("Missing prefab " + type);
                    return;
                }

                Log.Info("Spawning prefab " + type);

                if (!Player.m_localPlayer)
                {
                    Log.Error("Missing local player");
                    return;
                }

                Vector3 b = UnityEngine.Random.insideUnitSphere * offset;
                var position = Player.m_localPlayer.transform.position + Player.m_localPlayer.transform.forward * 2f + Vector3.up + b;

                Log.Info("Spawning position " + position.ToString());

                var instance = UnityEngine.Object.Instantiate(prefab, position, Quaternion.identity);

                //var renderer = instance.GetComponentInChildren<SkinnedMeshRenderer>();
                //renderer.material = ZNetScene.instance.GetPrefab("Blob").GetComponentInChildren<SkinnedMeshRenderer>().material;

                if (!instance)
                {
                    Log.Error("Missing instance " + type);
                    return;
                }

                var character = instance.GetComponent<Character>();

                if (!character)
                {
                    Log.Error("Missing component " + type);
                    return;
                }

                character.SetLevel(level);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}