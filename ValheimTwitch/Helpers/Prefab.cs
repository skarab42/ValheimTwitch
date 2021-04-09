using System;
using UnityEngine;
using ValheimTwitch.Patches;

namespace ValheimTwitch.Helpers
{
    public static class Prefab
    {
        public static Tameable SetTameable(ZNetView znview, GameObject go)
        {
            if (go.GetComponent<MonsterAI>() == null)
            {
                Log.Warning(go.name + " is not tamable");

                return null;
            }

            Tameable tame;

            if (!go.TryGetComponent(out tame))
            {
                tame = go.AddComponent<Tameable>();
            }

            var humanoid = go.GetComponent<Humanoid>();
            humanoid.m_faction = Character.Faction.Players;

            var tameable = ZNetScene.instance.GetPrefab("Wolf").GetComponent<Tameable>();

            tame.m_petEffect = tameable.m_petEffect;
            tame.m_tamingTime = tameable.m_tamingTime;
            tame.m_commandable = tameable.m_commandable;
            tame.m_fedDuration = tameable.m_fedDuration;
            tame.m_tamedEffect = tameable.m_tamedEffect;
            tame.m_sootheEffect = tameable.m_sootheEffect;

            return tame;
        }

        public static string GetTamedName(string name, bool tamed)
        {
           return tamed ? $"{name} <color=magenta><3</color>" : name;
        }

        static void SetName(ZNetView znview, Character character, string name, bool tamed)
        {
            character.m_name = GetTamedName(name, tamed);
            znview.GetZDO().Set($"{Plugin.GUID}-name", name);
        }

        public static void Spawn(string type, int level = 1, float offset = 100, bool tamed = false, string name = null)
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
                    Log.Error("Missing prefab instance");
                    return;
                }

                var character = instance.GetComponent<Character>();

                if (character == null)
                    return;

                ZNetView znview = character.GetComponent<ZNetView>();
                Tameable component = SetTameable(znview, instance);

                if (name != null)
                    SetName(znview, character, name, tamed);

                if (tamed && component != null)
                {
                    component.Tame();
                    CharacterAwakePatch.tamedCharacters.Add(character);
                    znview.GetZDO().Set($"{Plugin.GUID}-tamed", true);
                }

                if (level > 0)
                    character.SetLevel(level);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}