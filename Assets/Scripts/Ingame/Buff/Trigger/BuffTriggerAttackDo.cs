using Rogue.Ingame.Character;
using Rogue.Ingame.Data.Buff;
using Rogue.Ingame.Entity;

namespace Rogue.Ingame.Buff.Trigger
{
    public class BuffTriggerAttackDo : BuffTriggerBase
    {
        private readonly ActionTypeMask typeMask;
        private readonly CharacterBehaviour character;
        private CharacterStateInfo prevInfo;

        public BuffTriggerAttackDo(IEntity me, BuffTriggerData triggerData) : base(me, triggerData)
        {
            typeMask = triggerData.AttackType;
            character = me.GameObject.GetComponent<CharacterBehaviour>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnUpdate()
        {
            if (character == null)
                return;
            var curInfo = character.TotalInfo.StateInfo;
            if (prevInfo.Id != curInfo.Id)
            {
                if (curInfo.StateType == CharacterStateType.Action)
                {
                    if (ActionTypeHelper.CheckType(typeMask, curInfo.ActionData.AttackType))
                        Invoke(Me);
                }
            }

            prevInfo = curInfo;
        }

        protected override void OnEnd()
        {

        }
    }
}