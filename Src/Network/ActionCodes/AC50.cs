using System;
using System.Linq;
using Game;
using Game.Battle;
using Network;

namespace Network.ActionCodes
{
    public class AC50 : AC
    {
        public override int ID { get { return 50; } }

        public override void ProcessPkt(Player r, RecievePacket p)
        {
            switch (p.Unpack8())
            {
                case 1:
                    RecvAttack(r, p);
                    break;
                default:
                    log.Warn(p.A + "," + p.B + " Has not been coded");
                    break;
            }
        }

        void RecvAttack(Player r, RecievePacket p)
        {
            if (r == null || r.MyBattle == null || r.MyBattle.Owner == null)
                return;

            Battle owner = r.MyBattle.Owner;
            Fighter src = null;
            Fighter dst = null;

            try
            {
                byte srcX = p.Unpack8();
                byte srcY = p.Unpack8();
                byte dstX = p.Unpack8();
                byte dstY = p.Unpack8();

                src = owner.FindFighter(srcX, srcY);
                dst = owner.FindFighter(dstX, dstY);

            }
            catch
            {
                src = null;
                dst = null;
            }

            if (src == null)
                src = r;
            if (src != r)
                return;
            if (dst == null)
                dst = FindFirstEnemy(owner, r.BattlePosition);
            if (dst == null)
                return;

            owner.PLayer_BattleAction(new BattleAction { src = src, dst = dst });
        }

        Fighter FindFirstEnemy(Battle battle, BattleRole sourceRole)
        {
            BattleRole enemyRole = sourceRole == BattleRole.Attacking ? BattleRole.Defending : BattleRole.Attacking;
            return battle[enemyRole].FighterList.FirstOrDefault(c => c.CurHP > 0);
        }
    }
}
