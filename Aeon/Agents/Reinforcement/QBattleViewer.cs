using System.Collections.Generic;
using Aeon.Agents.Network;
using Aeon.Core.GameProcess;

namespace Aeon.Agents.Reinforcement;

public class QBattleViewer: NetworkBattleViewer
{
    private const float RoundReward = -0.25f; //-1f;
    private const float RoundWinReward = 3f;

    public bool WasBattle;

    public QBattleViewer() : this(2, 2) { }
    public QBattleViewer(int start, int end): base(start, end) { }

    public int ModelTotalBattles { get; private set; }
    public int ModelWinner { get; private set; }

    public float Reward { get; private set; }

    public override void OnBattleEnd(BattleEnd model)
    {
        base.OnBattleEnd(model);
        ModelWinner       =  model.Winner;
        ModelTotalBattles += 1;
        WasBattle         =  true;

        Reward = RoundReward+model.Winner > 0? 1*RoundWinReward : 0;
    }
}