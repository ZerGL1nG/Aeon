﻿using Aeon.Core.Heroes;

namespace Aeon.Core.GameProcess
{
    public class Battle
    {


        private const int maxBattleIt = 100;
        private Hero First { get;}
        private Hero Second { get; }

        private Viewer Viewer { get; }

        public Battle(Viewer viewer, Hero first, Hero second)
        {
            Viewer = viewer;
            First = first;
            Second = second;
        }


        public void StartBattle()
        {
            First.StartBattle();
            Second.StartBattle();
            for(var it = 0; it < maxBattleIt; it++){
                
                var att1 = First.MakeAttack();
                var att2 = Second.MakeAttack();
                var rec1 = First.ReceiveAttack(att2);
                var rec2 = Second.ReceiveAttack(att1);

                var dead1 = First.CheckDead();
                var dead2 = Second.CheckDead();
                if (dead1 || dead2)
                {
                    First.EndBattle(!dead1);
                    Second.EndBattle(!dead2);
                    break;
                }
                if(rec1.Damage > 0) First.TryRegen();
                if(rec2.Damage > 0) Second.TryRegen();
            }
        }
        
    }
}