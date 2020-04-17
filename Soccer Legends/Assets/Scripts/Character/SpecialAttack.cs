using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAttack : ScriptableObject {

    public abstract bool canUseSpecial(PVE_Manager mg, GameObject specialOwner, float energy);

    public abstract bool canUseSpecial(Manager mg, GameObject specialOwner, float energy);

    public abstract IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner, GameObject rival);

    public abstract IEnumerator callSpecial(Manager mg, GameObject specialOwner, GameObject rival);
}
