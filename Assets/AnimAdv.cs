using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimAdv : MonoBehaviour
{
    [System.Serializable]
    public class AnimSet
    {
        public Sprite[] anims;
    }

    public SpriteRenderer SR;
    public AnimSet[] anims;
    public float speed;

    private int animIndex;
    private float timeC;

    public int AnimSetIndex;

    private void Update()
    {
        timeC += Time.deltaTime;
        if (timeC > speed)
        {
            timeC = 0;
            animIndex++;
            if (animIndex == anims[AnimSetIndex].anims.Length)
            {
                animIndex = 0;
            }
            SR.sprite = anims[AnimSetIndex].anims[animIndex];
        }
    }

    public void SetAnimSetIndex(int index)
    {
        if (AnimSetIndex == index)
        {
            return;
        }
        AnimSetIndex = index;
        animIndex = 0;
    }


}
