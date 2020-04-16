using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialAtack : SpecialAttackInfo {

    public abstract bool canUseSpecial(PVE_Manager mg, GameObject specialOwner);

    public abstract bool canUseSpecial(Manager mg, GameObject specialOwner);

    public abstract IEnumerator callSpecial(PVE_Manager mg, GameObject specialOwner);

    public abstract IEnumerator callSpecial(Manager mg, GameObject specialOwner);
}
