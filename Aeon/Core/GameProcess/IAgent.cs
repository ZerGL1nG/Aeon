using System.Collections.Generic;
using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess;

public interface IAgent
{
    public IBattleViewer BattleView { get; }
    public IShopViewer ShopView { get; }
    public bool IsBot { get; }
        
    public Command ShopDecision();

    public HeroClasses ChooseClass();
}
    
public interface IBattleViewer
{
    public void OnBattleStart(BattleStart model);
    public void OnAttack(BattleAttack model);
    public void OnHeal(BattleHeal model);
    public void OnBattleEnd(BattleEnd model);
}

public interface IShopViewer
{
    public void OnShopUpdate(Hero customer);
}