using UnityEngine;

public class PartySlotController : MonoBehaviour
{
    public BattleSlots battleSlots;
    private BattleCharacter[] partyMembers;
    private Transform[] partyHomes;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        partyMembers = GetComponentsInChildren<BattleCharacter>();
        partyHomes = battleSlots.GetSlots(true, partyMembers, 1);
        GoToHome();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToHome()
    {

        for (int i = 0; i < partyMembers.Length; i++)
        {
            partyMembers[i].transform.position = partyHomes[i].position;
        }
    }
}
