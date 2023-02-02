using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

/// <summary>
/// ��ųʸ��� �ø������� �ȵȴ� ��� Ŀ�������� ���������Ѵ�.
/// ISerializationCallbackReceiver �� ��ӹ޾ƻ��
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

