using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

/// <summary>
/// 딕셔너리는 시리얼라이즈가 안된다 고로 커스텀으로 만들어줘야한다.
/// ISerializationCallbackReceiver 를 상속받아사용
/// </summary>

public class JHDictionary<Tkey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField] List<Tkey> dominoType;
    [SerializeField] List<TValue> values;

    Dictionary<Tkey, TValue> target;

    public Dictionary<Tkey, TValue> ToDictionary()
    {
        return target;
    }

    public void Serialization(Dictionary<Tkey, TValue> _target)
    {
        target = _target;
    }

    public void OnBeforeSerialize()
    {
        dominoType = new List<Tkey>(target.Keys);
        values = new List<TValue>(target.Values);
    }

    public void OnAfterDeserialize()
    {
        var count = Mathf.Min(dominoType.Count, values.Count);
        target = new Dictionary<Tkey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(dominoType[i], values[i]);
        }
    }
}

