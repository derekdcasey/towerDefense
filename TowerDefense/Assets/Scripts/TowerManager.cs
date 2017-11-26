using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

    public TowerBtn towerBtnPressed { get; set; }
    private SpriteRenderer spriteRenderer;
    private List<Tower> towerList = new List<Tower>();
    private List<Collider2D> buildList = new List<Collider2D>();
    private Collider2D buildTile;


    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
	}
	
    public void RegisterBuildSite(Collider2D buildTag)
    {
        buildList.Add(buildTag);
    }
    public void RegisterTower(Tower tower)
    {
        towerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach (Collider2D tag in buildList)
        {
            tag.tag = "BuildSite";

        }
        buildList.Clear();
    }

    public void DestroyAllTowers()
    {
        foreach (Tower tower in towerList)
        {
            Destroy(tower.gameObject);
        }
        towerList.Clear();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider.CompareTag("BuildSite"))
            {
                buildTile = hit.collider;
                buildTile.tag = "BuildSiteFull";
                RegisterBuildSite(buildTile);
                PlaceTower(hit);
            }


        }
        if (spriteRenderer.enabled)
        {
            followMouse();
        }
    }

    public void PlaceTower(RaycastHit2D hit)
    {
        if (!EventSystem.current.IsPointerOverGameObject() && towerBtnPressed != null)
        {
        Tower newTower = Instantiate(towerBtnPressed.TowerObject);
        newTower.transform.position = hit.point;
            BuyTower(towerBtnPressed.TowerPrice);
            RegisterTower(newTower);
            DisableDragSprite();
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.TowerBuilt);
        }
    }

 

    public void BuyTower(int price)
    {
        GameManager.Instance.SubtractMoney(price);
    }

    public void SelectedTower(TowerBtn towerSelected)
    {
        if(GameManager.Instance.TotalMoney >= towerSelected.TowerPrice) { 
        towerBtnPressed = towerSelected;
        EnableDragSprite(towerBtnPressed.DragSprite);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.ButtonPush);
        }
    }

    public void followMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }
    public void EnableDragSprite(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableDragSprite()
    {
        spriteRenderer.enabled = false;
        
    }
}
