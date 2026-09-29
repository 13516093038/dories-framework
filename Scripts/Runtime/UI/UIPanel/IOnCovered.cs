using System;

public interface IOnCovered
{
    /// <summary>
    /// 当界面被覆盖时调用
    /// </summary>
    /// <param name="panelType">被哪个界面覆盖</param>
    void OnCovered(Type panelType);
}