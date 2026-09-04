using UnityEngine;

namespace Project.Assets._Project._Scripts.DI
{
    public class TypedInstance<T>
    {
        private readonly T _value;
        protected TypedInstance(T value) => _value = value;
        public static implicit operator T(TypedInstance<T> typedInstance) => typedInstance._value;
    }

    public class GameplayCamera : TypedInstance<Camera>
    {
        public GameplayCamera(Camera value) : base(value) { }
    }
}
