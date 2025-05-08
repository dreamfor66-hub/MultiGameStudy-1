using System.Collections.Generic;
using System.Linq;

namespace Rogue.Ingame.Data.Buff
{
    public enum BuffTag
    {
        None = 0,
        Synergy = 1,
        Perk = 2,
        AbnormalStatus = 3,
        StartBuff = 4,

        ElementStack = 100,
        FireStack = 101,
        AcidStack = 102,

        ElementPrimary = 110,
        FireBurn = 111,
        AcidBurn = 112,
        //IceStun = 113,

        ElementSecondary = 120,
        FireDefDown = 121,
        AcidAtkDown = 122,
        //CrushSlow = 123,


        //특정 버프 저격용 태그는 여기서부터
        BuffEffect = 300,
        PlagueEffect = 301,
        HideEffect = 302,
        StunByHpEffect = 303,

        //플레이어블 캐릭터 전용
        /*
         * 10000번대부터 시작
         * 캐릭터끼리는 100번대 자릿수로 구분함
         */

        //피터
        PeterStack = 10001,
        PeterCharge = 10002,
        PeterChargeDamage = 10003,
        PeterChargeEnd = 10004,
        PeterChargeReleased = 10005,

        //로빈
        RobinBasicAttack = 10101,
        RobinCharge = 10102,
        RobinStack = 10103,
        RobinBasicEnd = 10104,
        RobinChargeReleased = 10105,
        RobinPerfectReleased = 10106,
        RobinSpecial = 10201,
        RobinSpecialCheck = 10202,
        RobinSpecialEnd = 10203,
        
        //Archer
        ArcherBasicRandomMotion = 10301,

        

        //몬스터 전용
        /*
         * 20000번대부터 시작
         * 캐릭터끼리는 100번대 자릿수로 구분함
         */
        GorilSummonHeal = 20001,
        GorilCageReady = 20002,
        GorilBoxReady = 20003,
        GorilQuakeReady = 20004,
        GorilCageStun = 20005,
        GorilPhase = 20006,

        GorilMinion = 20101,
        GorilMinionRun = 20102,
        GorilMinionDrop = 20103,

        //자폭스위치
        Self = 99999,
    }

    public class BuffTagHelper
    {
        public static bool SustainIfOwnerDead(List<BuffTag> tags)
        {
            return tags.Any(x =>
            {
                switch (x)
                {
                    case BuffTag.Perk:
                    case BuffTag.Synergy:
                    case BuffTag.StartBuff:
                        return true;
                    default:
                        return false;
                }
            });
        }
    }
}
