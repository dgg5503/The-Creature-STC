/*
    CrawlRootAnim
    ------------------------
    AUTHS Douglas Gliner
    
*/

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CrawlRootAnim : MonoBehaviour {

    // Fields
    private Quaternion crawlAngle;
    private WaitForEndOfFrame waitEOF; // save in var so we dont create a new one in coroutine.

    [SerializeField]
    private float transitionTime = 0.05f;

    void Awake()
    {
        waitEOF = new WaitForEndOfFrame();
    }

    
    void OnAnimatorMove() { }

    public void SetCrawlingAngle(Quaternion angle)
    {
        crawlAngle = angle;
        StartCoroutine(TransitionAngle(transform.localRotation, crawlAngle, transitionTime));
    }

    private IEnumerator TransitionAngle(Quaternion start, Quaternion end, float time)
    {
        // lerp timer
        float currentLerpTime = 0;

        //increment timer once per frame
        while (currentLerpTime < time)
        {
            currentLerpTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(start, end, (currentLerpTime / time));
            yield return waitEOF;
        }
    }
}
