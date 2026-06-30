using UnityEngine;

public enum BodyPartGroupType
{
    Skeleton,
    Muscles,
    Veins
}

public class BodyGroup : MonoBehaviour
{
    public BodyPartGroupType selectedGroup = BodyPartGroupType.Skeleton;
    public GameObject[] skeletonGroup;
    public GameObject[] musclesGroup;
    public GameObject[] veinsGroup;

    void Awake() {
        if (selectedGroup == BodyPartGroupType.Skeleton) {
            foreach (GameObject obj in skeletonGroup) {
                obj.SetActive(true);
            }
            foreach (GameObject obj in musclesGroup) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in veinsGroup) {
                obj.SetActive(false);
            }
        }
        else if (selectedGroup == BodyPartGroupType.Muscles) {
            foreach (GameObject obj in skeletonGroup) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in musclesGroup) {
                obj.SetActive(true);
            }
            foreach (GameObject obj in veinsGroup) {
                obj.SetActive(false);
            }
        }
        else if (selectedGroup == BodyPartGroupType.Veins) {
            foreach (GameObject obj in skeletonGroup) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in musclesGroup) {
                obj.SetActive(false);
            }
            foreach (GameObject obj in veinsGroup) {
                obj.SetActive(true);
            }
        }
    }
}
