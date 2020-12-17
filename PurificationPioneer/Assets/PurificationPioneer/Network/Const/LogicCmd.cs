namespace PurificationPioneer.Network.Const
{
    public static class LogicCmd
    {
        public const int LoginLogicReq = 1;
        public const int LoginLogicRes = 2;
        public const int UdpTestReq = 3;
        public const int UdpTestRes = 4;
        public const int StartMatchReq = 5;
        public const int StartMatchRes = 6;
        public const int AddMatcherTick = 7;
        public const int RemoveMatcherTick = 8;
        public const int FinishMatchTick = 9;
        public const int StopMatchReq = 10;
        public const int StopMatchRes = 11;
        public const int SelectHeroReq = 12;
        public const int SelectHeroRes = 13;
        public const int SubmitHeroReq = 14;
        public const int SubmitHeroRes = 15;
        public const int UpdateSelectTimer = 16;
        public const int ForceSelect = 17;
        public const int StartLoadGame = 18;
        public const int StartGameReq = 19;
        public const int StartGameRes = 20;
        public const int NextFrameInput = 21;
        public const int LogicFramesToSync = 22;
        public const int InitUdpReq = 23;
        public const int InitUdpRes = 24;
        public const int DebugUdpReq = 25;
    }
}