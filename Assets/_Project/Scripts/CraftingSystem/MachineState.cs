using System;
using _Project.Scripts.General.ReusePatterns;
using _Project.Scripts.Item;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.CraftingSystem
{
    [Serializable]
    public abstract class MachineState : MyGeneralState<IMachineContext>
    {
        protected MachineState(IMachineContext context) : base(context)
        {
        }
    }
}