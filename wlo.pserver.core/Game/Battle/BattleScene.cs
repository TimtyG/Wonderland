using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network;

namespace Game.Battle
{

    public delegate void BattleRoundInfo(List<Game.Battle.Fighter> fighters_on_my_side, List<Game.Battle.Fighter> fighters_on_other_side);
         
    /// <summary>
    /// Object that will house information pertaining to battle
    /// 
    /// this information is based on which side the player is on and who is on the same side
    /// </summary>
    public class BattleScene
    {
        readonly Battle _battleref;
        readonly BattleRole role;

        int round;

        List<Fighter> fighterlist; public IReadOnlyList<Fighter> FighterList { get { return fighterlist; } }

        public Battle Owner { get { return _battleref; } }
        public BattleRole Role { get { return role; } }
        public eBattleRoundState RoundState { get { return _battleref.RoundState; } }

        public event BattleRoundInfo onRoundStart;
        public event EventHandler onBattleOver;

        public BattleScene(BattleRole sideRole, Battle owner)
        {
            _battleref = owner;
            role = sideRole;

            round = 0;
            fighterlist = new List<Fighter>();
        }

        public int Total_Fighters { get { return fighterlist.Count; } }
        public int Total_Fighters_Alive { get { return fighterlist.Count(c => c.CurHP > 0); } }

        #region Methods
        /// <summary>
        /// Used to Poll if all fighters have finished what they need to do
        /// </summary>
        public bool EveryoneReady
        {
            get
            {
                if (fighterlist.Count(c => c.ActionDone) == fighterlist.Count)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool AvailableSlot(bool IsPlayer, bool IsPet, bool IsMob, out byte[] location)
        {
            byte x = 0;
            byte[] order = new byte[] { 2, 3, 1, 4, 0 };

            if (IsPlayer)
            {
                switch (role)
                {
                    case BattleRole.Defending: x = 1; break;
                    case BattleRole.Attacking: x = 4; break;
                    default:
                        location = new byte[0];
                        return false;
                }
            }
            else if (IsMob || IsPet)
            {
                switch (role)
                {
                    case BattleRole.Defending: x = 0; break;
                    case BattleRole.Attacking: x = 5; break;
                    default:
                        location = new byte[0];
                        return false;
                }
            }
            else
            {
                location = new byte[0];
                return false;
            }

            foreach (byte y in order)
            {
                location = new byte[] { x, y };
                if (!HasFighter(location))
                    return true;
            }

            location = new byte[0];
            return false;
        }

        /// <summary>
        /// Detmines if a a specific location has a fighter
        /// </summary>
        /// <param name="location">Location to check</param>
        /// <returns> true if theres a fighter</returns>
        bool HasFighter(byte[] location)
        {
            bool alive;
            return HasFighter(location, out alive);
        }
        /// <summary>
        /// Detmines if a a specific location has a fighter
        /// </summary>
        /// <param name="location">Location to check</param>
        /// <param name="IsAlive">is the fighter alive</param>
        /// <returns> true if theres a fighter</returns>
        bool HasFighter(byte[] location, out bool IsAlive)
        {
            IsAlive = false;
            if (location == null || location.Length < 2)
                return false;

            Fighter fighter = fighterlist.FirstOrDefault(c => c.GridX == location[0] && c.GridY == location[1]);
            if (fighter == null)
                return false;

            IsAlive = fighter.CurHP > 0;
            return true;
        }


       public void OnNewRound()
        {
            if (onRoundStart != null)
            {
                BattleRole rndptr = (role == BattleRole.Attacking) ? BattleRole.Defending : BattleRole.Attacking;
                onRoundStart(fighterlist, _battleref[rndptr].fighterlist);
            }
        }


        public bool AddFighter(Fighter src)
        {
            return OnNewFighter(src);
        }

        bool OnNewFighter(Fighter src)
        {
            if (src == null || fighterlist.Contains(src))
                return false;

            byte[] loc;
            bool added = false;

            if (src.TypeofFighter == eFighterType.player && AvailableSlot(true, false, false, out loc))
                added = true;
            else if (src.TypeofFighter == eFighterType.Npc_Mob && AvailableSlot(false, false, true, out loc))
                added = true;
            else if (src.TypeofFighter == eFighterType.Pet && AvailableSlot(false, true, false, out loc))
                added = true;

            if (!added)
                return false;

            src.BattlePosition = role;
            src.GridX = loc[0];
            src.GridY = loc[1];
            src.OnNewBattle(this);
            fighterlist.Add(src);
            return true;
        }

        public bool RemoveFighter(Fighter fighter)
        {
            if (fighter == null)
                return false;

            return fighterlist.Remove(fighter);
        }

        void FighterLeft(eBattleLeaveType exit, Fighter fighter)
        {
            RemoveFighter(fighter);
        }



        #endregion







        public Fighter FindFighter(byte x, byte y)
        {
            return fighterlist.FirstOrDefault(c => c.GridX == x && c.GridY == y);
        }

        public void ProcessSocket(RecievePacket g)
        {
        }

    }
}
