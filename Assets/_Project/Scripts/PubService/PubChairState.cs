using System;
using System.Linq;
using _Project.Scripts.General.ReusePatterns;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Project.Scripts.PubService
{
    [Serializable]
    public abstract class PubChairState : MyGeneralState<IPubChairContext>
    {
        protected PubChairState(IPubChairContext context) : base(context)
        {
        }
    }
}