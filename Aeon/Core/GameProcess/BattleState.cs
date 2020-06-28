namespace Aeon.Core.GameProcess
{
    public class BattleState
    {
        public double MyMaxHp { get; set; }
        public double EnemyMaxHp { get; set; }

        public double MyCurHp { get; set; }
        public double EnemyCurHp { get; set; }

        public double MyRecDmg { get; set; }
        public double EnemyRecDmg { get; set; }

        public double MyRegen { get; set; }
        public double EnemyRegen { get; set; }


        public BattleState Reverse() => new BattleState(
            EnemyMaxHp,
            MyMaxHp,
            EnemyCurHp,
            MyCurHp,
            EnemyRecDmg,
            MyRecDmg,
            EnemyRegen,
            MyRegen
            );
        
        public BattleState(double myMaxHp = 0,
            double enemyMaxHp = 0,
            double myCurHp = 0,
            double enemyCurHp = 0, 
            double myRecDmg = 0,
            double enemyRecDmg = 0,
            double myRegen = 0,
            double enemyRegen = 0)
        {
            MyMaxHp = myMaxHp;
            EnemyMaxHp = enemyMaxHp;
            MyCurHp = myCurHp;
            EnemyCurHp = enemyCurHp;
            MyRecDmg = myRecDmg;
            EnemyRecDmg = enemyRecDmg;
            MyRegen = myRegen;
            EnemyRegen = enemyRegen;
        }
    }
}