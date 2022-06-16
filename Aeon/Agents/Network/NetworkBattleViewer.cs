using System.Collections.Generic;
using Aeon.Core;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;

namespace Aeon.Agents.Network;

public class NetworkBattleViewer : IBattleViewer, INetworkData
{
    public virtual void OnBattleStart(BattleStart model)
    {
        _doubles = new List<float>();
        _doubles.AddRange(new[] { StatConverters.Convert(Stat.Health, model.EnemyMaxHealth) });
    }

    public virtual void OnAttack(BattleAttack model) => _attacks.Add(model);

    public virtual void OnHeal(BattleHeal model) => _heals.Add(model);

    public virtual void OnBattleEnd(BattleEnd model)
    {
        for (int i = 0; i < SavedRounds; i++)
        {
            if (i < _attacks.Count)
            {
                var m = _attacks[i];
                _doubles.AddRange(new[]
                {
                    StatConverters.Convert(Stat.Health, m.GiveDamage), 
                    StatConverters.Convert(Stat.Health, m.TakeDamage)
                });
            }
            else _doubles.AddRange(new float[4]);
            if (i < _heals.Count)
            {
                var m = _heals[i];
                _doubles.AddRange(new[]
                {
                    StatConverters.Convert(Stat.Health, m.MineHeal), 
                    StatConverters.Convert(Stat.Health, m.EnemyHeal), 
                    StatConverters.Convert(Stat.Health, m.MineHealth), 
                    StatConverters.Convert(Stat.Health, m.EnemyHealth)
                });
            }
            else _doubles.AddRange(new float[4]);
        }
        for (int i = 1; i <= SavedRoundsEnd; i++)
        {
            if (i <= _attacks.Count)
            {
                var m = _attacks[^i];
                _doubles.AddRange(new[]
                {
                    StatConverters.Convert(Stat.Health, m.GiveDamage),
                    StatConverters.Convert(Stat.Health, m.TakeDamage)
                });
            }
            else _doubles.AddRange(new float[4]);
            if (i <= _heals.Count)
            {
                var m = _heals[^i];
                _doubles.AddRange(new[]
                {
                    StatConverters.Convert(Stat.Health, m.MineHeal), 
                    StatConverters.Convert(Stat.Health, m.EnemyHeal),
                    StatConverters.Convert(Stat.Health, m.MineHealth),
                    StatConverters.Convert(Stat.Health, m.EnemyHealth)
                });
            }
            else _doubles.AddRange(new float[4]);
        }
    }

    private readonly List<BattleAttack> _attacks = new();
    private readonly List<BattleHeal> _heals = new();

    protected List<float> _doubles = new(new float[25]);
    public virtual IEnumerable<float> Inputs => _doubles;
    public INetworkData State => new ArrayData(_doubles);
    public virtual int Size { get; } = 1 + (SavedRounds + SavedRoundsEnd) * 6;

    private const int SavedRounds = 2;
    private const int SavedRoundsEnd = 2;
}