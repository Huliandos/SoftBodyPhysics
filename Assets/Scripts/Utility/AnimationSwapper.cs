using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSwapper : MonoBehaviour
{
    [SerializeField]
    int numOfAnimations = 3;

    int animationCounter;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            animationCounter++;
            animationCounter %= numOfAnimations;

            animator.SetInteger("AnimationCounter", animationCounter);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (animationCounter == 0) animationCounter = numOfAnimations-1;
            else animationCounter--;

            animator.SetInteger("AnimationCounter", animationCounter);
        }
    }
}
