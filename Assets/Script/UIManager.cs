using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    bool _uiUp;
    public bool UiUp
    {
        get => _uiUp;
        set
        {
            _uiUp = value;
            OnUiUpChange?.Invoke(value);
        }
    }

    public Action<bool> OnUiUpChange;

    Dictionary<Type, UIBase> _uis = new Dictionary<Type, UIBase>();
    Stack<UIBase> _ui = new Stack<UIBase>();


    public void Register(UIBase ui)
    {
        if (!_uis.TryAdd(ui.GetType(), ui))
        {
            Debug.LogError($"{ui.gameObject.name}이 중복으로 등록되고 있습니다.");
        }
    }

    public T Get<T>()
        where T : UIBase
    {
        return (T)_uis[typeof(T)];
    }

    public void PushUI(UIBase ui)
    {
        _ui.Push(ui);
        ui.SortingOrder = _ui.Count;
        UiUp = _ui.Count != 0;
    }

    public void PopUI(UIBase ui)
    {
        if (_ui.Peek() != ui)
        {
            Debug.LogWarning("해당 UI가 최상단이 아닙니다.");
            return;
        }
        _ui.Pop();
        UiUp = _ui.Count != 0;
    }
}
