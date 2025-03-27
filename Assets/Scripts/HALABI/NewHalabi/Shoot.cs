using UnityEngine;

public class Shoot : MonoBehaviour
{   
    [SerializeField]
    GameObject arrowPrefab;
    [SerializeField]
    GameObject arrow;
    [SerializeField]
    int numberOfArrows = 10;
    [SerializeField]
    GameObject bow;
    bool arrowSlotted = false;
    float pullAmount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnArrow();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void SpawnArrow()
    {
        if(numberOfArrows > 0)
        {
            arrowSlotted = true;
            arrow = Instantiate(arrowPrefab, transform.position, transform.rotation) as GameObject;
            arrow.transform.parent = transform;
        }

    }
    void ShootLogic()
    {
        if (numberOfArrows > 0)
        {
            if (pullAmount <100)
            {
                pullAmount =100;
                SkinnedMeshRenderer _bowSkin = bow.transform.GetComponent<SkinnedMeshRenderer>();
            }
        }
    }

}
