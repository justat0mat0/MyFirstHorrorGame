using System;

namespace VICTORCom
{
    /// <summary>
    /// 解谜/环境交互运行时事件：由 <see cref="PuzzleInteractUIController"/> 派发，
    /// 场景脚本订阅 <see cref="EventRaised"/> 即可。
    /// </summary>
    public static class PuzzleInteractRuntimeEvents
    {
        /// <summary>
        /// lineIndex：整段开始/结束为 -1；某一句为 0..n-1。
        /// </summary>
        public static event Action<string, PuzzleInteractData, int> EventRaised;

        public static void Raise(string eventId, PuzzleInteractData context, int lineIndex)
        {
            if (string.IsNullOrEmpty(eventId))
                return;
            EventRaised?.Invoke(eventId, context, lineIndex);
        }
    }
}
