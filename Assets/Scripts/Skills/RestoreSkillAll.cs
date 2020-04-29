using UnityEngine;

namespace TheBunniesOfVegetaria
{
    public class RestoreSkillAll : Skill
    {
        private int skillAmount;

        public RestoreSkillAll(int cost, int minimumLevel, string name, int skillAmount) : base(cost, minimumLevel, name)
        {
            this.skillAmount = skillAmount;
            Target = TargetType.Bunny;
            Description = $"Party SP +{skillAmount}";
        }

        public override void Use(BunnyActor user, IActor[] targets)
        {
            foreach (IActor target in targets)
            {
                if (target is BunnyActor && (BunnyActor)target != user)
                {
                    BunnyActor targetBunny = (BunnyActor)target;
                    targetBunny.RestoreSkill(skillAmount);
                }
            }
        }
    }
}