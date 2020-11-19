using System;

namespace ReadyGamerOne.Network
{
    public interface INetworkConnection<T>
    {
        void Init(Action<T> onHandleNetPkg);
    }
}