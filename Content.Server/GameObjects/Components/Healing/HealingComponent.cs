﻿using System;
using Content.Server.GameObjects.Components.Stack;
using Robust.Shared.GameObjects;
using Content.Server.GameObjects.EntitySystems;
using Robust.Shared.Interfaces.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.IoC;
using Robust.Server.GameObjects;
using Robust.Shared.Maths;
using Robust.Server.Interfaces.GameObjects;
using Robust.Shared.Interfaces.Timing;
using Robust.Shared.GameObjects.EntitySystemMessages;
using Robust.Shared.Serialization;
using Robust.Shared.Interfaces.GameObjects.Components;
using Content.Shared.GameObjects;

namespace Content.Server.GameObjects.Components.Weapon.Melee
{
    public class HealingComponent : Component, IAfterAttack, IUse
    {
        public override string Name => "Healing";

        public int Heal = 100;
        public DamageType Damage = DamageType.Brute;

        public override void ExposeData(ObjectSerializer serializer)
        {
            base.ExposeData(serializer);

            serializer.DataField(ref Heal, "heal", 100);
            serializer.DataField(ref Damage, "damage", DamageType.Brute);
        }

        void IAfterAttack.AfterAttack(AfterAttackEventArgs eventArgs)
        {
            if (eventArgs.Attacked == null)
            {
                return;
            }

            if (!eventArgs.Attacked.TryGetComponent(out DamageableComponent damagecomponent)) return;
            if (Owner.TryGetComponent(out StackComponent stackComponent))
            {
                if (!stackComponent.Use(1))
                {
                    Owner.Delete();
                    return;
                }

                damagecomponent.TakeHealing(Damage, Heal);
                return;
            }
            damagecomponent.TakeHealing(Damage, Heal);
            Owner.Delete();
        }

        bool IUse.UseEntity(UseEntityEventArgs eventArgs)
        {
            if (!eventArgs.User.TryGetComponent(out DamageableComponent damagecomponent)) return false;
            if (Owner.TryGetComponent(out StackComponent stackComponent))
            {
                if (!stackComponent.Use(1))
                {
                    Owner.Delete();
                    return false;
                }

                damagecomponent.TakeHealing(Damage, Heal);
                return false;
            }
            damagecomponent.TakeHealing(Damage, Heal);
            Owner.Delete();
            return false;
        }
    }
}
