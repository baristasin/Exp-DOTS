using Unity.Entities;
using Unity.Mathematics;

namespace Game.Scripts
{
    public struct GraveyardRandom : IComponentData
    {
        public Random Value;
    }
}