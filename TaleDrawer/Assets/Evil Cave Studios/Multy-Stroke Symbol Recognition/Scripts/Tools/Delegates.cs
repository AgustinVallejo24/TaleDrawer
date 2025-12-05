using UnityEngine;

namespace Delegates
{
    public delegate void DeleteDelegate<T>(T dato);

    public delegate void RecognitionAction(SpawnableObjectType symbol);
}
