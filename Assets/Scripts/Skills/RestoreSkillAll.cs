using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class RestoreSkillAll : Skill
    {
        private int skillAmount;

        public RestoreSkillAll(int cost, int minimumLevel, string name, int skillAmount) : base(cost, minimumLevel, name)
        {
            this.skillAmount = skillAmount;
            type = TargetType.Bunny;
            Description = $"Party SP +{skillAmount}";
        }

        public override void Use(IActor user, IActor[] targets)
        {
            foreach (IActor target in targets)
            {
                if (target != user && target is BunnyActor)
                {
                    BunnyActor targetBunny = (BunnyActor)target;
                    targetBunny.RestoreSkill(skillAmount);
                }
            }
        }
    }
}