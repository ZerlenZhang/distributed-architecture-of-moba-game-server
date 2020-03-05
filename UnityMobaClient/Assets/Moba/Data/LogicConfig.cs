namespace Moba.Data
{
    public class TowerConfig
    {
        public int hp;
        public int attackRadis;
        public int shoot_logic_fps;
    }

    public class BulletConfig
    {
        public int speed;
        public int maxDis;
        public int attack;
    }

    public class HeroConfig
    {
        public int defence;
        public int attack;
        public int maxHp;
        public int addHp;
        public int maxExp;
    }
    
    public static class LogicConfig
    {
        public static readonly int LogicFrameTime = 50;
        public static readonly int PlayerCount = 3;

        public static readonly HeroConfig[] NormalHero =
        {
            new HeroConfig
            {
                defence = 1,
                attack = 10,
                maxHp = 100,
                maxExp = 100,
                addHp = 10,
            },
            new HeroConfig
            {
                defence = 1,
                attack = 10,
                maxHp = 100,
                maxExp = 100,
                addHp = 10,
            },
            new HeroConfig
            {
                defence = 1,
                attack = 10,
                maxHp = 100,
                maxExp = 100,
                addHp = 10,
            },
        };

        public static readonly TowerConfig MainTower
            = new TowerConfig
            {
                hp = 10,
                attackRadis = 10,
                shoot_logic_fps = 3,
            };

        public static readonly TowerConfig NormalTower
            = new TowerConfig
            {
                hp = 10,
                attackRadis = 10,
                shoot_logic_fps = 5,
            };

        public static readonly BulletConfig MainBullet
            = new BulletConfig
            {
                attack = 10,
                speed = 20,
                maxDis = 20,
            };
        public static readonly BulletConfig NormalBullet
            = new BulletConfig
            {
                attack = 10,
                speed = 20,
                maxDis = 20,
            };
    }
}