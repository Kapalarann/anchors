using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public Spell[] spells = new Spell[4];
    public KeyCode[] spellKeys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R };

    private Spell selectedSpell;
    private GameObject spellIndicator;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        
        for (int i = 0; i < spells.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]) && spells[i] != null)
            {
                SelectSpell(spells[i]);
            }
        }

      
        if (selectedSpell != null)
        {
            UpdateSpellIndicator();

           
            if (Input.GetMouseButtonDown(0))
            {
                CastSelectedSpell();
            }
        }
    }

    private void SelectSpell(Spell spell)
    {
        selectedSpell = spell;

        
        if (spellIndicator != null) Destroy(spellIndicator);

        
        if (selectedSpell.spellPrefab != null)
        {
            spellIndicator = Instantiate(selectedSpell.spellPrefab);
            spellIndicator.GetComponent<Collider>().enabled = false; 
        }
    }

    private void UpdateSpellIndicator()
    {
        if (spellIndicator != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                spellIndicator.transform.position = hit.point; 
            }
        }
    }

    private void CastSelectedSpell()
    {
        if (selectedSpell == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject spellObj = Instantiate(selectedSpell.spellPrefab);
            spellObj.transform.position = (selectedSpell is QSpell) ? transform.position : hit.point;

            Spell spellInstance = spellObj.GetComponent<Spell>();
            if (spellInstance != null)
            {
                spellInstance.Cast(hit.point);
            }
            else
            {
                Debug.LogError("The instantiated spell does not have a Spell component!");
            }
        }

        if (spellIndicator != null) Destroy(spellIndicator);
        selectedSpell = null;
    }


      
}
