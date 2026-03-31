using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalentTreeUIController : MonoBehaviour
{
    public TMP_Text pointsLabel;

    // EXPLICIT list; populate in the prefab via Inspector
    [SerializeField] public List<TalentNodeUI> nodes = new List<TalentNodeUI>();

    private MapPartyMemberDefinition member;

    public void SetCharacter(MapPartyMemberDefinition m)
    {
        member = m;

        for (int i = 0; i < nodes.Count; i++)
        {
            var n = nodes[i];
            if (n != null) n.Bind(member, OnNodeToggle);
        }

        RefreshAll();
    }

    void OnNodeToggle(TalentDefinition t)
    {
        

        if (member == null || t == null) return;

        if (!member.HasTalent(t.id) && member.CanLearn(t))

            if(t.cost == 0)
            {
                //Only one free talent per character
                foreach (var node in nodes)
                {
                    if(node.talent.cost == 0 && member.HasTalent(node.talent.id))
                    {
                        member.UnlearnTalent(node.talent);
                    }
                }
            }

            member.LearnTalent(t);

        RefreshAll();
    }


    void RefreshAll()
    {
        if (pointsLabel && member != null)
            pointsLabel.text = $"Points: {member.talentPoints}";

        for (int i = 0; i < nodes.Count; i++)
            if (nodes[i] != null) nodes[i].Refresh();
    }

    // Optional utility
    public void RefundAll()
    {
        if (member == null) return;

        for (int i = 0; i < nodes.Count; i++)
        {
            var n = nodes[i];
            if (n != null && n.talent != null && member.HasTalent(n.talent.id))
                member.UnlearnTalent(n.talent);
        }
        member.InitializeTalents();
        RefreshAll();
    }
}
