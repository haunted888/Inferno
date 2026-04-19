using UnityEngine;

public enum CharacterTrait
{
    Hero,
    Villain,
    Entertainer,
    Expeditioner,
    Fighter,
    Marksman,
    Rogue,
    Strategist,
    Expeditionist,
    Alchemist,
}

public abstract class TraitDefinition : ScriptableObject
{
    public string traitName;
    [TextArea] public string description;
    public CharacterTrait traitType = CharacterTrait.Hero;

    // Called once when a BattleCharacter is built from a map member
    public virtual void SetupForBattle(MapPartyMemberDefinition source, BattleCharacter battleChar) { }

    // Called for every *damaging* skill just before damage is applied
    public virtual void OnModifySkillDamage(
        BattleCharacter user,
        Skill skill,
        BattleCharacter target,
        ref int damage)
    { }

    public virtual void OnPassiveApplied(
        BattleCharacter user,
        PassivesDefinition passive,
        BattleCharacter target)
    { }

    public virtual void OnBattleStart(BattleCharacter user) { }

    public virtual void OnDeath(BattleCharacter user) { }

}
