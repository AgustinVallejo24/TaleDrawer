using UnityEngine;
using System.Collections;
using DG.Tweening;
public class LavaCollider : MonoBehaviour
{
    public int waterCount;
    public int ObsidianThreshold;

    public MetaballManager waterManager;
    public MetaballManager lavaManager;

    public Material obsidianMat;

    public SpriteRenderer _myRenderer;

    public GameObject particleSteamPrefab;
    public ParticleSystem surfaceSteam;
    public bool isSolidifying;
    public BoxCollider2D boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        //lavaManager.spriteRenderer.material.SetFloat("_LiquidMinY", boxCollider.bounds.min.y);
        //lavaManager.spriteRenderer.material.SetFloat("_LiquidMaxY", boxCollider.bounds.max.y);

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Water")
        {
            var part = Instantiate(particleSteamPrefab);
            part.transform.position = collision.transform.position;
            Destroy(collision.gameObject);
            waterCount++;
            waterManager.DestroyParticle(collision.gameObject);
            surfaceSteam.Play();
            if (!isSolidifying)
            {
                isSolidifying = true;
                //  _myRenderer.material.DOFloat(2.3f, "_FillAmount", .7f).SetUpdate(true);

                lavaManager.spriteRenderer.material.DOFloat(.9f, "_OverlayFill", .4f).SetEase(Ease.Linear).SetUpdate(true);
                lavaManager.spriteRenderer.material.SetFloat("_NoiseStrength", 0);
                lavaManager.spriteRenderer.material.SetFloat("_WaveStrength", 0);
                lavaManager.spriteRenderer.material.SetFloat("_FlattenStrength", 0);
            }
            if (waterCount >= ObsidianThreshold)
            {
                boxCollider.isTrigger = false;
              //  GetComponent<SpriteRenderer>().enabled = true;

                //lavaManager.material = obsidianMat;


                waterManager.DestroyAllParticles();
              //  lavaManager.DestroyAllParticles();
               
            }
        }

        if(collision.TryGetComponent(out Character character))
        {
            character.Death();
        }
    }

}
