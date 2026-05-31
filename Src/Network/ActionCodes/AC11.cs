using System;
using System.Linq;
using Game;
using Game.Battle;
using Network;

namespace Network.ActionCodes
{
    public class AC11 : AC
    {
        public override int ID { get { return 11; } }

        public override void ProcessPkt(Player r, RecievePacket p)
        {
            switch (p.Unpack8())
            {
                case 1:
                    RecvLeave(r, p);
                    break;
                case 2:
                    RecvPk(r, p);
                    break;
                default:
                    log.Warn(p.A + "," + p.B + " Has not been coded");
                    break;
            }
        }

        void RecvLeave(Player r, RecievePacket p)
        {
            byte leaveType = 0;
            try { leaveType = p.Unpack8(); }
            catch { }

            if (r != null && r.MyBattle != null)
                r.MyBattle.Owner.RemFighter(leaveType == 3 ? eBattleLeaveType.RunAway : eBattleLeaveType.BattleFinished, r);
        }

        void RecvPk(Player r, RecievePacket p)
        {
            if (r == null || r.MyBattle != null)
                return;

            byte pkType = p.Unpack8();
            UInt32 targetID = p.Unpack32();
            UInt16 clickID = p.Unpack16();

            if (pkType == 2)
                StartPlayerBattle(r, targetID, clickID);
            else if (pkType == 3)
                StartTrainingBattle(r, targetID, clickID);
        }

        void StartPlayerBattle(Player attacker, uint targetID, ushort clickID)
        {
            GameMap map = attacker.CurMap as GameMap;
            if (map == null)
                return;

            Player defender = map.Players.FirstOrDefault(c => c.CharID == targetID);
            if (defender == null || defender == attacker || defender.MyBattle != null)
                return;

            Battle battle = new Battle(0, Environment.TickCount);
            battle.TypeofBattle = eBattleType.pk;
            battle[BattleRole.Attacking].AddFighter(attacker);
            defender.ClickID = clickID;
            battle[BattleRole.Defending].AddFighter(defender);
            battle.StartBattle();
        }

        void StartTrainingBattle(Player attacker, uint targetID, ushort clickID)
        {
            Battle battle = new Battle(0, Environment.TickCount);
            battle.TypeofBattle = eBattleType.normal;
            battle[BattleRole.Defending].AddFighter(attacker);
            battle[BattleRole.Attacking].AddFighter(new TrainingFighter(targetID == 0 ? 900001U : targetID, "Training NPC", 1, 100, 30, 15, 5, 8) { ClickID = clickID });
            battle.StartBattle();
        }
    }
}
