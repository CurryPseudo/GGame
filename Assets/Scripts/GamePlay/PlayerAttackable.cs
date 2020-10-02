﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerAttackable
{
    // Start is called before the first frame update
    bool OnAttack();
    bool ValidBox(BoxPhysics box);
}
