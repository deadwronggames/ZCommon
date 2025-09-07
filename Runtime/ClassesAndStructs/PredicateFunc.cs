using System;
using DeadWrongGames.ZCommon.Interfaces;

namespace DeadWrongGames.ZCommon.ClassesAndStructs
{
    public class PredicateFunc : IPredicate
    {
        private readonly Func<bool> _func;

        public PredicateFunc(Func<bool> func)
        {
            _func = func;
        }
        
        public bool Evaluate() => _func.Invoke();
    }   
}