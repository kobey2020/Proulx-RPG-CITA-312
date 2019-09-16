using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat

{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = .05f;
        [SerializeField] float weaponDamage = 5f;

        Health target;
        float timesSinceLastAttack = 0;

        private void Update()
        {
            timesSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if (timesSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit() event
                GetComponent<Animator>().SetTrigger("attack");
                timesSinceLastAttack = 0;
            }

        }

        //Animation Event
        void Hit()
        {
            target.takeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public bool CanAttack(CombatTarget combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(CombatTarget combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("stopAttack");
            target = null;
        }
    }
}