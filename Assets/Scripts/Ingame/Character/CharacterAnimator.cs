using System;
using Rogue.Ingame.Attack;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Character
{
    public class CharacterAnimator
    {
        private readonly CharacterData characterData;
        private readonly Animator animator;

        public CharacterAnimator(CharacterData characterData, Animator animator)
        {
            this.characterData = characterData;
            this.animator = animator;
        }

        public void UpdateAnimator(CharacterStateUpdateInfo update)
        {
            var state = update.Cur.StateType;
            animator.speed = 0f;


            var height = 0f;
            switch (update.Cur.StateType)
            {
                case CharacterStateType.Idle:
                    if (update.Prev.StateType == CharacterStateType.Run && characterData.ExtraRunAnimation)
                        animator.Play("Run_End");
                    else if (update.Prev.StateType == CharacterStateType.WalkAround && characterData.ExtraWalkAnimation)
                        animator.Play("Walk_End");
                    else if (update.Prev.StateType != CharacterStateType.Idle)
                        animator.Play("Idle");
                    UpdateTime(update.DeltaTime);
                    break;
                case CharacterStateType.Run:
                    {
                        if (characterData.ExtraRunAnimation)
                        {
                            if (update.Prev.StateType != CharacterStateType.Run)
                                animator.Play("Run_Start");
                        }
                        else
                        {
                            animator.Play("Run");
                        }
                        UpdateTime(update.DeltaTime);
                        break;
                    }
                case CharacterStateType.WalkAround:
                    {
                        if (characterData.ExtraWalkAnimation)
                        {
                            if (update.Prev.StateType != CharacterStateType.WalkAround && update.Prev.StateType != CharacterStateType.Run)
                                animator.Play("Walk_Start");
                        }
                        else
                        {
                            animator.Play("Walk");
                        }

                        var angle = Vector3.SignedAngle(animator.transform.forward, update.Cur.Direction, Vector3.up);
                        if (angle < 0)
                            angle += 360;
                        animator.SetFloat("Angle", angle);
                        UpdateTime(update.DeltaTime);
                        break;
                    }
                case CharacterStateType.Action:
                    {
                        var actionData = update.Cur.ActionData;
                        var convertedFrame = actionData.AnimationAdjustCurve.Evaluate(update.Cur.Frame);
                        var normalizedTime = convertedFrame / actionData.AnimationOriginalFrame;

                        height = actionData.MoveCurveY.Evaluate(update.Cur.Frame / CommonVariables.GameFrame);
                        animator.Play(actionData.ActionKey, 0, normalizedTime);
                    }
                    break;
                case CharacterStateType.Hurt:
                case CharacterStateType.Dead:
                    {
                        var curFrame = update.Cur.Frame;
                        var prevFrame = curFrame - update.DeltaFrame;
                        var distance = update.Cur.KnockbackDistance;
                        var totalKnockbackFrame = KnockbackCalculator.TotalFrame(distance);

                        if (curFrame < totalKnockbackFrame)
                        {
                            var animName = StrengthToAnimName(update.Cur.KnockbackStrength);
                            var showHeight = StrengthToShowHeight(update.Cur.KnockbackStrength);
                            animator.Play(animName, 0, curFrame / totalKnockbackFrame);
                            if (showHeight)
                            {
                                height = KnockbackCalculator.FrameToHeight(distance, curFrame);
                            }
                        }
                        else
                        {
                            if (state == CharacterStateType.Dead)
                            {
                                animator.Play("Dead");
                            }
                            else
                            {
                                var afterTotalFrame = KnockbackAfterFrame(update.Cur);
                                var afterCurFrame = curFrame - totalKnockbackFrame;
                                var afterAnim = KnockbackAfterAnimName(update.Cur.KnockbackStrength);
                                animator.Play(afterAnim, 0, afterCurFrame / afterTotalFrame);
                            }
                            UpdateTime(update.DeltaTime);
                        }
                    }
                    break;
                case CharacterStateType.Stun:
                    animator.Play("Stun");
                    UpdateTime(update.DeltaTime);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            animator.transform.localPosition = new Vector3(0f, height, 0f);
        }

        private void UpdateTime(float deltaTime)
        {
            animator.speed = 1f;
            animator.Update(deltaTime);
            animator.speed = 0f;
        }

        private string StrengthToAnimName(KnockbackStrength strength)
        {
            switch (strength)
            {
                case KnockbackStrength.Low:
                    return "KnockbackLow";
                case KnockbackStrength.Mid:
                    return "KnockbackMid";
                case KnockbackStrength.High:
                    return "KnockbackHigh";
                case KnockbackStrength.JustDamage: // JustDamage 로 죽으면 들어올 수 있다.
                    return "KnockbackLow";
                default:
                    throw new ArgumentOutOfRangeException(nameof(strength), strength, null);
            }
        }

        private bool StrengthToShowHeight(KnockbackStrength strength)
        {
            switch (strength)
            {
                case KnockbackStrength.Low:
                    return false;
                case KnockbackStrength.Mid:
                    return characterData.KnockbackMidUseHeight;
                case KnockbackStrength.High:
                    return characterData.KnockbackHighUseHeight;
                case KnockbackStrength.JustDamage:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(strength), strength, null);
            }
        }

        private int KnockbackAfterFrame(CharacterStateInfo stateInfo)
        {
            switch (stateInfo.KnockbackStrength)
            {
                case KnockbackStrength.Low:
                    return stateInfo.KnockStopFrame;
                case KnockbackStrength.Mid:
                    return characterData.KnockbackMidGetUpFrame;
                case KnockbackStrength.High:
                    return characterData.KnockbackHighGetUpFrame;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string KnockbackAfterAnimName(KnockbackStrength strength)
        {
            switch (strength)
            {
                case KnockbackStrength.Low:
                    return "KnockStop";
                case KnockbackStrength.Mid:
                    return "GetUpMid";
                case KnockbackStrength.High:
                    return "GetUpHigh";
                default:
                    throw new ArgumentOutOfRangeException(nameof(strength), strength, null);
            }
        }
    }
}
