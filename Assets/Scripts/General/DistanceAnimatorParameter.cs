﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceAnimatorParameter : MonoBehaviour
{
    GameObject player;
    Transform cameraT;

    [SerializeField]
    [Tooltip("The transforms with which distance will be checked. Only the closest one will be used.")]
    private List<Transform> targets;

    [SerializeField]
    [Tooltip("The animator where we will update the parameter.")]
    private Animator animatorComponent;

    [SerializeField]
    [Tooltip("The name of the parameter in the animator that will receive the distance value..")]
    private string parameterName = "Distance";

    private Coroutine updateCoroutine;
    //for other scriptps to reference
    [HideInInspector]
    public float distanceFromPlayer;

    //needs to work with Trip sequence as well
    // just add Guardian Rider transform to targets on awake if its a trip obj

    private void Awake()
    {
        animatorComponent = GetComponent<Animator>();
        
        player = GameObject.FindGameObjectWithTag("Player");

        cameraT = Camera.main.transform;

        ClearEmptyTargets();

        if(player)
            targets.Add(player.transform);

        if (cameraT)
            targets.Add(cameraT);
    }

    void ClearEmptyTargets()
    {
        for(int i = 0; i < targets.Count; i++)
        {
            if(targets[i] == null)
            {
                targets.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnValidate()
    {
        //if (targets.Count == 0 || targets.TrueForAll(t => t == null))
        //{
        //    Debug.LogWarningFormat(gameObject, "No target specified in DistanceAnimatorParameter of {0}", name);
        //}

        //if (animatorComponent == null)
        //{
        //    Debug.LogWarningFormat(gameObject, "No animator component specified in DistanceAnimatorParameter of {0}", name);
        //}
    }

    private void OnEnable()
    {
        //only run the coroutine if we find the parameter
        if(HasParameter(parameterName, animatorComponent))
        {
            updateCoroutine = StartCoroutine(UpdateDistanceCoroutine());
        }
    }

    private void OnDisable()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
    }

    IEnumerator UpdateDistanceCoroutine()
    {
        Transform myTransform = transform;
        int parameter = Animator.StringToHash(parameterName);
        while (true)
        {
            float closestDistance = Mathf.Infinity;

            foreach (Transform target in targets)
            {
                float distance = Vector3.Distance(target.position, myTransform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                }
            }

            animatorComponent.SetFloat(parameter, closestDistance);

            yield return null;
        }
    }

    public static bool HasParameter(string paramName, Animator animator)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}