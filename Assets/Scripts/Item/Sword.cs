using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    private bool isAttacking = false;

    private readonly List<Character> characters = new();

    public void Attack()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !isAttacking)
        {
            animator.SetTrigger(AnimationParametre.Attack.ToString());
            foreach (var character in characters)
            {
                character.Hit(GetComponentInParent<Character>().stat.damage);
                (character as Goblin).damageEffect.Play();
                if (character.knockback.isKnockback) continue;
                Vector2 direction = -(transform.position - character.transform.position).normalized;
                StartCoroutine(character.knockback.KnockbackCoroutine(character.GetComponent<Rigidbody2D>(), direction));
            }
        }
    }

    public void StartAttack() => isAttacking = true;

    public void EndAttack() => isAttacking = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Goblin))
        {
            characters.Add(other.GetComponent<Character>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Tag.Goblin) && characters.Contains(other.GetComponent<Character>()))
        {
            characters.Remove(other.GetComponent<Character>());
        }
    }
}