using System;
using System.Linq;
using _Project.Scripts.General.ReusePatterns;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public abstract class PubTableState : MyGeneralState<IPubTableContext>
    {
        protected PubTableState(IPubTableContext context) : base(context)
        {
        }
    }
}