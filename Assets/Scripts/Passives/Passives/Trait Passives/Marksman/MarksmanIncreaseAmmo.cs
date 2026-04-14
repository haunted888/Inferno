using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Passives/Trait Passives/Marksman/Marksman Increase Ammo")]
public class MarksmanIncreaseAmmo : PassivesDefinition
{
    public bool recoverAmmo = true;
    public int ammoIncrease = 0;

    public override void OnCreated(BattleCharacter self)
    {
        if(!self.traitTypes.Contains(CharacterTrait.Marksman))
        {
            self.RemovePassive(this);
            return; // Only applies if the character has the Marksman trait
        }
        int newMaxAmmo = self.MaxAmmo + ammoIncrease;
        int newCurrentAmmo = Math.Min(self.CurrentAmmo + (recoverAmmo ? ammoIncrease : 0), newMaxAmmo);
        self.SetAmmo(newMaxAmmo, newCurrentAmmo);
    }

    public override void OnDestroyed(BattleCharacter self)
    {
        int newMaxAmmo = self.MaxAmmo - ammoIncrease;
        int newCurrentAmmo = Math.Min(self.CurrentAmmo, newMaxAmmo);
        self.SetAmmo(newMaxAmmo, newCurrentAmmo);
    }
}
