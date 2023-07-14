using System.Collections.Generic;
using UnityEngine;
using BlazeAISpace;

public class BlazeAIDistanceCulling : MonoBehaviour
{
    [Tooltip("Automatically get the game camera.")]
    public bool autoCatchCamera = true;

    [Tooltip("The player or camera to calculate the distance between it and the AIs.")]
    public Transform playerOrCamera;

    [Min(0), Tooltip("If an AI distance is more than this set value then the it will get culled.")]
    public float distanceToCull = 30;
    
    [Range(0, 30), Tooltip("Run the cycle every set frames. The bigger the number, the better it is for performance but less accurate.")]
    public int cycleFrames = 7; 

    [Tooltip("If set to true, the culling will be disabling Blaze only and playing the idle animation of the normal state. When within range again, Blaze will re-enable.")]
    public bool disableBlazeOnly;


    int framesPassed = 0;
    List<BlazeAI> agentsList = new List<BlazeAI>();


    public static BlazeAIDistanceCulling instance;


    void Awake()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }


        if (autoCatchCamera) {
            playerOrCamera = Camera.main.transform;
            return;
        }


        if (playerOrCamera == null) {
            Debug.LogWarning("No camera has been set in the camera property in the Blaze AI Distance Culling component.");
        }
    }


    void Update()
    {
        // prevent continuation if no camera set
        if (playerOrCamera == null) {
            return;
        }


        // increment the frames for the cycle
        if (framesPassed < cycleFrames) {
            framesPassed++;
            return;
        }

        framesPassed = 0;


        // loop the AIs and disable/enable based on distance
        for (int i=0; i<agentsList.Count; i++) {
            BlazeAI blaze = agentsList[i];

            // calculate the distance using sqr magnitude since it's faster than Vector3.Distance()
            float agentDistance = (blaze.transform.position - playerOrCamera.position).sqrMagnitude;

            // if distance is larger than set -> cull the AI
            if (agentDistance > distanceToCull * distanceToCull) {
                if (disableBlazeOnly) {
                    if (!blaze.enabled) {
                        continue;
                    }

                    blaze.enabled = false;
                    
                    if (blaze.state != BlazeAI.State.death) {
                        PlayCullAnim(blaze);
                    }

                    continue;
                }


                if (!blaze.gameObject.activeSelf) continue;


                blaze.gameObject.SetActive(false);
                continue;
            }
            

            // reaching this point means distance is less than set -> re-enable AI
            
            
            if (disableBlazeOnly) {
                if (!blaze.enabled) {
                    blaze.enabled = true;
                    continue;
                }
            }


            if (!blaze.gameObject.activeSelf) {
                blaze.gameObject.SetActive(true);
                PlayCullAnim(blaze);
            }
        }
    }


    // add agent to the list of culling
    public void AddAgent(BlazeAI agent)
    {
        if (agentsList.Contains(agent)) {
            return;
        }

        agentsList.Add(agent);
    }


    // remove agent from the list of culling
    public void RemoveAgent(BlazeAI agent)
    {
        if (!agentsList.Contains(agent)) {
            return;
        }

        agentsList.Remove(agent);
    }


    // check if passed agent is in the list of culling
    public bool CheckAgent(BlazeAI agent)
    {
        if (agentsList.Contains(agent)) {
            return true;
        }

        return false;
    }


    // play the cull animation
    void PlayCullAnim(BlazeAI blaze)
    {
        if (blaze.animToPlayOnCull.Length > 0 && blaze.animToPlayOnCull != null) {
            blaze.animManager.Play(blaze.animToPlayOnCull, 0.25f);
        }
    }
}

