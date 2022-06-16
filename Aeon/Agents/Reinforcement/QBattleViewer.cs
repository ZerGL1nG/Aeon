using System.Collections.Generic;
using System.Linq;
using Aeon.Agents.Network;
using Aeon.Core.GameProcess;

namespace Aeon.Agents.Reinforcement;

public class QBattleViewer : NetworkBattleViewer
{
    public int ModelTotalBattles { get; private set; }
    public int ModelWinner { get; private set; } = 0;

    private const float RoundReward = 0; //-1f;
    private const float RoundWinReward = 0f;

    public float Reward { get; private set; }

    public bool WasBattle = false;

    public QBattleViewer()
    {
        new List<float>(new float[base.Size]);
    }

    public override void OnBattleEnd(BattleEnd model)
    {
        base.OnBattleEnd(model);
        ModelWinner = model.Winner;
        ModelTotalBattles += 1;
        WasBattle = true;

        Reward = RoundReward + model.Winner > 0 ? 1 * RoundWinReward : 0;
    }
}