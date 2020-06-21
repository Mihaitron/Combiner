using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D;

public class Material : MonoBehaviour
{
    public int tier = 0;
    public bool isCombined = false;
    public UnityEvent combining;
    public SpriteAtlas spriteAtlas;

    private int coinsToGive;
    private Vector3 positionOnScreen;
    private Sprite[] textures;

    private void Awake()
    {
        this.textures = new Sprite[this.spriteAtlas.spriteCount];
        for (int i = 0; i < this.spriteAtlas.spriteCount; i++)
            this.textures[i] = this.spriteAtlas.GetSprite("Materials" + (i + 1).ToString());
    }

    private void Start()
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = this.textures[tier];
        this.coinsToGive = this.tier * 2 + 1;

        StartCoroutine(GiveCoins());
    }

    private void OnMouseDown()
    {
        this.positionOnScreen = Camera.main.WorldToScreenPoint(this.transform.position);
    }

    private void OnMouseDrag()
    {
        Vector3 current_screen_point = new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.positionOnScreen.z);
        Vector3 current_position = Camera.main.ScreenToWorldPoint(current_screen_point);

        this.transform.position = current_position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Material" && collision.gameObject.GetComponent<Material>().tier == this.tier)
        {
            this.isCombined = true;

            combining.Invoke();
        }
    }

    private IEnumerator GiveCoins()
    {
        yield return new WaitForSeconds(1.0f);

        World.coinsCount += this.coinsToGive;

        StartCoroutine(GiveCoins());
    }
}
