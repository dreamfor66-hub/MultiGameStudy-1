using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rogue.Ingame.Data;
using UnityEngine;

namespace Rogue.Ingame.Bot
{
    [Serializable]
    public class BotPatternScoreVariables
    {
        public float ScoreBase = 10f;
        public float ScoreDamageHit = 5.0f;
        public float ScoreDamageHurt = -0.9f;
        public float ScoreModHurtWhileWait = 0.2f;

    }

    public class BotPatternRecord
    {
        public BotPhasePatternPoolData Pattern;
        public float DamageHit;
        public float DamageHurt;
        public int TotalFrame;
        public PatternPoolTag tag;
    }

    public class BotPatternScore
    {
        public BotPhasePatternPoolData Pattern;
        public float Score;
        public float ChanceDiff;
    }

    public class BotPatternScoreModel
    {
        private static BotPatternScoreVariables defaultVariables = new BotPatternScoreVariables
        {
            ScoreBase = 10f,
            ScoreDamageHit = 5.0f,
            ScoreDamageHurt = -0.9f,
            ScoreModHurtWhileWait = 0.2f,
        };

        public BotPatternRecord CurPattern;
        public int CurFrame;
        public List<BotPatternRecord> PatternRecords;
        public List<BotPatternScore> PatternScores;
        private readonly BotPatternScoreVariables variables = defaultVariables;

        public BotPatternScoreModel(BotPatternScoreVariables variables)
        {
            PatternRecords = new List<BotPatternRecord>();
            PatternScores = new List<BotPatternScore>();

            if (variables != null)
                this.variables = variables;
        }

        public void UpdateFrame()
        {
            if (CurPattern == null)
                return;
            CurFrame++;
            CurPattern.TotalFrame++;
        }

        public void Hit(int damage)
        {
            if (CurPattern == null)
                return;
            CurPattern.DamageHit += damage;
        }

        public void Hurt(int damage, BotAiStateType state)
        {
            if (CurPattern == null)
                return;
            var isWhileWait = state == BotAiStateType.Wait;
            CurPattern.DamageHurt += damage * (isWhileWait ? variables.ScoreModHurtWhileWait : 1f);
        }

        public void Change(BotPhasePatternPoolData newPattern)
        {
            CurFrame = 0;

            if (newPattern == null)
            {
                CurFrame = 0;
                CurPattern = null;
                return;
            }

            var tp = PatternRecords.Find(x => x.Pattern == newPattern);
            if (tp != null)
            {
                CurPattern = tp;
            }
            else
            {
                CurPattern = new BotPatternRecord();
                CurPattern.Pattern = newPattern;
                PatternRecords.Add(CurPattern);
            }
        }

        public void UpdateScore(BotSinglePhaseData phase)
        {
            if (!phase.UpdatePatternScoreOnStart)
                return;

            PatternScores.Clear();
            Change(null);
            UpdateScoreByTag(PatternPoolTag.Random);
            UpdateScoreByTag(PatternPoolTag.Counter);
            UpdateScoreByTag(PatternPoolTag.Evade);
            Debug.LogWarning(GetDebugString());
            PatternRecords.Clear();
        }

        public void UpdateScoreByTag(PatternPoolTag tag)
        {
            float totalScore = 0;
            var records = PatternRecords.Where(x => x.Pattern.Tag == tag).ToList();
            foreach (var record in records)
            {
                var score = PatternScores.Find(x => x.Pattern == record.Pattern);
                if (score == null)
                {
                    score = new BotPatternScore();
                    score.Pattern = record.Pattern;
                    PatternScores.Add(score);
                }
                score.Score = record.DamageHit * variables.ScoreDamageHit / record.TotalFrame +
                              record.DamageHurt * variables.ScoreDamageHurt / record.TotalFrame +
                              variables.ScoreBase;
                totalScore += score.Score;
            }

            var mean = totalScore / records.Count;

            foreach (var score in PatternScores)
            {
                if (score.Pattern.Tag == tag)
                    score.ChanceDiff = (score.Score - mean) * 100;
            }
        }

        public float GetChanceDiff(BotPhasePatternPoolData pattern)
        {
            var find = PatternScores.Find(x => x.Pattern == pattern);
            if (find == null)
                return 0f;

            return find.ChanceDiff;
        }

        public string GetDebugString()
        {
            var sb = new StringBuilder();
            foreach (var record in PatternRecords)
            {
                sb.Append($"{record.Pattern.Tag.ToString()} {record.Pattern.Pattern.name}: EstimatedScore:{record.DamageHit * variables.ScoreDamageHit / record.TotalFrame + record.DamageHurt * variables.ScoreDamageHurt / record.TotalFrame + variables.ScoreBase:0.000}\n");
            }
            sb.Append($"\n");
            foreach (var score in PatternScores)
            {
                sb.Append($"{score.Pattern.Pattern.name}: Diff:{score.ChanceDiff:0.000}\n");
            }
            return sb.ToString();
        }
    }
}