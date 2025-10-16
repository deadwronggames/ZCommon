using System;
using DeadWrongGames.ZCommon.Interfaces;

namespace DeadWrongGames.ZCommon.ClassesAndStructs
{
    /// <summary>
    /// Simple IPredicate implementation that wraps a <see cref="bool"/> Func.
    /// Useful for runtime-constructed conditions or filters.
    /// </summary>
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