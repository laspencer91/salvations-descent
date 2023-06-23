using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Systems.Audio
{
    [CreateAssetMenu(fileName = "MaterialSoundDefinition", menuName = "Game/Create Footstep/Material Sound Definition")]
    public class MaterialToFootstepAudioEventConfiguration : SerializedScriptableObject
    {
        public AudioEvent fallbackFootstepAudioEvent;
        public Dictionary<Texture, AudioEvent> MatTypeToAudioEventDictionary = new Dictionary<Texture, AudioEvent>();

        public AudioEvent GetFootstepAudioEventForMaterial(Texture texture)
        {
            if (MatTypeToAudioEventDictionary.ContainsKey(texture)) 
            {
                return MatTypeToAudioEventDictionary[texture];
            }
            Debug.LogWarning("Material: " + texture.name + " is not set up in the Material Sound Definition Asset.");
            return fallbackFootstepAudioEvent;
        }
    }
}