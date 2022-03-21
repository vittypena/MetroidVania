using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    [SerializeField] int damagePoints = 10;

    [SerializeField] TagId targetTag;//SerializeField significa que se puede cambiar el valor de la variable en unity
    private Collider2D collider2D;


    private void Awake()//Metodo para tocar los parametros del colisionador del arma
    {
        collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false; //Deshabilitamos en un principio el daño

    }

    public void Attack(float attackDuration)
    {
        collider2D.enabled = true;
        StartCoroutine(_Attack(attackDuration));
    }

    private IEnumerator _Attack(float attackDuration)//Corutina del ataque para calcular el tiempo que pasa atacando y deshabilitar el colisionador en ese periodo
    {
        yield return new WaitForSeconds(attackDuration);
        collider2D.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)//Detecta cuando ocurre la colision del gameObject (espada en este caso) y realiza el daño a la salud
    {
        if (collision.gameObject.tag.Equals(targetTag.ToString()))
        {
            var component = collision.gameObject.GetComponent<ITargetCombat>();//Comprueba la colision

            if(component != null)//Si hay colision, haz daño
            {
                component.TakeDamage(damagePoints);
            }
        }
    }

}
