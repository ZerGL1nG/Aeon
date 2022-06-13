using System.Collections.Generic;
using Aeon.Core.GameProcess;
using AI.NeuralNetwork;

namespace Aeon.Agents;

public class NetworkBattleViewer : IBattleViewer, INetworkInput
{
    public void OnBattleStart(BattleStart model) =>
        _doubles.AddRange(new[] { (float)model.MineMaxHealth, (float)model.EnemyMaxHealth });

    public void OnAttack(BattleAttack model) => _attacks.Add(model);

    public void OnHeal(BattleHeal model) => _heals.Add(model);

    public void OnBattleEnd(BattleEnd model)
    {
        for (int i = 0; i < SavedRounds; i++)
        {
            if (i < _attacks.Count)
            {
                var m = _attacks[i];
                _doubles.AddRange(new[]
                    { (float)m.MineHealth, (float)m.EnemyHealth, (float)m.GiveDamage, (float)m.TakeDamage });
            }
            else _doubles.AddRange(new float[4]);
            if (i < _heals.Count)
            {
                var m = _heals[i];
                _doubles.AddRange(new[]
                    { (float)m.MineHealth, (float)m.EnemyHealth, (float)m.MineHeal, (float)m.EnemyHeal });
            }
            else _doubles.AddRange(new float[4]);
        }
        for (int i = 1; i <= SavedRoundsEnd; i++)
        {
            if (i <= _attacks.Count)
            {
                var m = _attacks[^i];
                _doubles.AddRange(new[]
                    { (float)m.MineHealth, (float)m.EnemyHealth, (float)m.GiveDamage, (float)m.TakeDamage });
            }
            else _doubles.AddRange(new float[4]);
            if (i <= _heals.Count)
            {
                var m = _heals[^i];
                _doubles.AddRange(new[]
                    { (float)m.MineHealth, (float)m.EnemyHealth, (float)m.MineHeal, (float)m.EnemyHeal });
            }
            else _doubles.AddRange(new float[4]);
        }
        _doubles.AddRange(new float[] { model.TotalRounds, model.Winner });
    }

    private readonly List<BattleAttack> _attacks = new();
    private readonly List<BattleHeal> _heals = new();

    private readonly List<float> _doubles = new();
    public IEnumerable<float> Inputs => _doubles;
    public int Size { get; } = 2 + (SavedRounds + SavedRoundsEnd) * 2 * 4 + 2;

    private const int SavedRounds = 2;
    private const int SavedRoundsEnd = 2;
}