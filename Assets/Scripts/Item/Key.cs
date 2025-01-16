using UnityEngine;
using UnityEngine.UI;

public class Key : MonoBehaviour
{
    private Image keyImage;
    private void Start()
    {
        keyImage = GameObject.FindGameObjectWithTag(Tag.KeyImage).GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tag.Player))
        {
            collision.gameObject.GetComponent<Player>().SetKey();
            keyImage.color = Color.white;
            Destroy(gameObject);
        }
    }
}
