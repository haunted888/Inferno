using UnityEngine;

public class TalentTreeHost : MonoBehaviour
{
    public Transform mountPoint;

    private TalentTreeUIController activeTree;

    
    public void ShowFor(MapPartyMemberDefinition member)
    {
        if (member == null) return;

        // Replace instantiated tree if it's different
        if (activeTree == null || (member.talentTreePrefab != null &&
                                   activeTree.gameObject.name.Replace("(Clone)", "") != member.talentTreePrefab.gameObject.name))
        {
            if (activeTree != null) Destroy(activeTree.gameObject);

            if (member.talentTreePrefab != null)
            {
                activeTree = Instantiate(member.talentTreePrefab, mountPoint);
            }
        }

        if (activeTree != null)
            activeTree.SetCharacter(member);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
