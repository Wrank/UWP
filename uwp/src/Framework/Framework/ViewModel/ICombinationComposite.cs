﻿namespace lindexi.uwp.Framework.ViewModel
{
    /// <summary>
    /// 组合 Composite 和 Message
    /// </summary>
    public interface ICombinationComposite
    {
        void Run(IViewModel source, IMessage message);
    }
}