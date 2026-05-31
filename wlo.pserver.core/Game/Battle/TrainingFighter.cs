using System;
using Game;

namespace Game.Battle
{
    public class TrainingFighter : Fighter
    {
        readonly uint id;
        readonly string name;
        BattleRole position;
        ushort clickId;
        uint ownerId;
        byte gridX;
        byte gridY;
        int curHp;
        int curSp;
        DateTime roundEndTime;

        public TrainingFighter(uint id, string name, byte level, int hp, int sp, int attack, int defense, int speed)
        {
            this.id = id;
            this.name = name;
            Level = level;
            MaxHP = hp;
            MaxSP = (short)Math.Max(0, Math.Min(short.MaxValue, sp));
            curHp = hp;
            curSp = sp;
            FullAtk = attack;
            FullDef = defense;
            FullMatk = attack;
            FullMdef = defense;
            FullSpd = speed;
            Element = Affinity.Normal;
            Job = RebornJob.none;
        }

        public uint ID { get { return id; } }
        public string Name { get { return name; } }
        public BattleRole BattlePosition { get { return position; } set { position = value; } }
        public eFighterType TypeofFighter { get { return eFighterType.Npc_Mob; } }
        public FighterState BattleState { get { return curHp > 0 ? FighterState.Alive : FighterState.Dead; } }
        public UInt16 ClickID { get { return clickId; } set { clickId = value; } }
        public UInt32 OwnerID { get { return ownerId; } set { ownerId = value; } }
        public byte Level { get; private set; }
        public byte GridX { get { return gridX; } set { gridX = value; } }
        public byte GridY { get { return gridY; } set { gridY = value; } }
        public bool ActionDone { get { return true; } }
        public Int32 CurHP { get { return curHp; } set { curHp = value; } }
        public Int32 CurSP { get { return curSp; } set { curSp = value; } }
        public DateTime RdEndTime { set { roundEndTime = value; } }
        public Int32 MaxHP { get; private set; }
        public Int16 MaxSP { get; private set; }
        public Affinity Element { get; private set; }
        public RebornJob Job { get; private set; }
        public bool Reborn { get { return false; } }
        public Int32 FullMatk { get; private set; }
        public Int32 FullAtk { get; private set; }
        public Int32 FullDef { get; private set; }
        public Int32 FullMdef { get; private set; }
        public Int32 FullSpd { get; private set; }

        public void OnNewBattle(BattleScene battle)
        {
            roundEndTime = DateTime.Now.AddSeconds(30);
        }
    }
}
