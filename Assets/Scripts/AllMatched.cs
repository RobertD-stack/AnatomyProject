using UnityEngine;
/* *** SUMMARY *** 
A tracker to determine whether all objects are matched or not
*/


public class AllMatched : MonoBehaviour
{
    public bool checkAllMatched = false;

    public void setAllMatched(bool match) {
        checkAllMatched = match;
    }
}
