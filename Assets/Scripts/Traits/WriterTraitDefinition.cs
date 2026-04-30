using UnityEngine;

[CreateAssetMenu(menuName = "Traits/Writer")]
public class WriterTraitDefinition : TraitDefinition
{
    
    public Skill writerStealSkill;

    void Awake()
    {
        traitType = CharacterTrait.Writer;
    }

    public override void OnInitialize(MapPartyMemberDefinition member)
    {
        member.skills.Add(writerStealSkill);
    }
}
