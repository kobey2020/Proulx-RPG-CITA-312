﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RPG.Movement;
using RPG.Core;
using System;

namespace RPG.Combat

{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = .05f;
        [SerializeField] float weaponDamage = 5f;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Transform handTransform = null;

        Health target;
        float timesSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            SpawnWeapon();
        }

        private void Update()
        {
            timesSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        private void SpawnWeapon()
        {
            Instantiate(weaponPrefab, handTransform);
        }

        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if (timesSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit() event
                TriggerAttack();
                timesSinceLastAttack = 0;
            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation Event
        void Hit()
        {
            if(target == null)
            {
                return;
            }
            target.takeDamage(weaponDamage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }
    }
}