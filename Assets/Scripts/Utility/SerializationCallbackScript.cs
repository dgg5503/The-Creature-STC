using UnityEngine;
using System.Collections.Generic;
using System;

public class SerializationCallbackScript : MonoBehaviour, ISerializationCallbackReceiver
{
    public List<int> _keys;
    public List<string> _values;

    //Unity doesn't know how to serialize a Dictionary
    public Dictionary<int, string> _myDictionary = new Dictionary<int, string>
        {
            {3, "I"},
            {4, "Love"},
            {5, "Unity"}
        };

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();
        foreach (var kvp in _myDictionary)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        _myDictionary.Clear();
        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            _myDictionary.Add(_keys[i], _values[i]);
    }
}