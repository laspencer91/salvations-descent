using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Systems.Audio
{
    [CreateAssetMenu(fileName = "MaterialSoundDefinition", menuName = "Game/Create Footstep/Material Sound Definition")]
    public class MaterialToFootstepAudioEventConfiguration : SerializedScriptableObject
    {
        public AudioEvent fallbackFootstepAudioEvent;
        public Dictionary<string, AudioEvent> MatTypeToAudioEventDictionary = new Dictionary<string, AudioEvent>();

        public AudioEvent GetFootstepAudioEventForMaterial(Texture texture)
        {
            // Cycle through each material key of the sound definition file. Return the AudioEvent if the texture name begins with the key.
            foreach (var key in MatTypeToAudioEventDictionary.Keys) 
            {
                if (texture.name.StartsWith(key)) 
                    return MatTypeToAudioEventDictionary[key];
            }

            Debug.LogWarning("Material: " + texture.name + " is not set up in the Material Sound Definition Asset.");
            return fallbackFootstepAudioEvent;
        }
    }
}